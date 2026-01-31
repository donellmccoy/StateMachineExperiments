using Microsoft.EntityFrameworkCore;
using Stateless;
using StateMachineExperiments.Data;
using StateMachineExperiments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineExperiments.Services
{
    public class LodStateMachineService : ILodStateMachineService
    {
        private readonly LodDbContext _context;
        private readonly Dictionary<LodState, LodAuthority> _stateToAuthorityMap;

        public LodStateMachineService(LodDbContext context)
        {
            _context = context;

            _stateToAuthorityMap = new Dictionary<LodState, LodAuthority>
            {
                { LodState.Start, LodAuthority.None },
                { LodState.MemberReports, LodAuthority.Member },
                { LodState.LodInitiation, LodAuthority.LodMfp },
                { LodState.MedicalAssessment, LodAuthority.MedicalProvider },
                { LodState.CommanderReview, LodAuthority.ImmediateCommander },
                { LodState.OptionalLegal, LodAuthority.LegalAdvisor },
                { LodState.OptionalWing, LodAuthority.WingCommander },
                { LodState.BoardAdjudication, LodAuthority.ReviewingBoard },
                { LodState.Determination, LodAuthority.ApprovingAuthority },
                { LodState.Notification, LodAuthority.LodPm },
                { LodState.Appeal, LodAuthority.AppellateAuthority },
                { LodState.End, LodAuthority.None }
            };
        }

        public async Task<InformalLineOfDuty> CreateNewCaseAsync(string caseNumber, string? memberId = null, string? memberName = null)
        {
            var entity =_context.LodCases.Add(new InformalLineOfDuty
            {
                CaseNumber = caseNumber,
                MemberId = memberId,
                MemberName = memberName,
                CurrentState = nameof(LodState.Start),
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return new InformalLineOfDuty
            {
                CaseNumber = caseNumber,
                MemberId = memberId,
                MemberName = memberName,
                CurrentState = nameof(LodState.Start),
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
        }

        public async Task<InformalLineOfDuty?> GetCaseAsync(int caseId)
        {
            return await _context.LodCases.Include(c => c.TransitionHistory).FirstOrDefaultAsync(c => c.Id == caseId);
        }

        public async Task<InformalLineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber)
        {
            return await _context.LodCases.Include(c => c.TransitionHistory).FirstOrDefaultAsync(c => c.CaseNumber == caseNumber);
        }

        public async Task FireTriggerAsync(int caseId, LodTrigger trigger, bool condition = true, string? notes = null)
        {
            var lodCase = await GetCaseAsync(caseId) ?? throw new InvalidOperationException($"Case with ID {caseId} not found.");

            var currentState = Enum.Parse<LodState>(lodCase.CurrentState);
            var stateMachine = ConfigureStateMachine(currentState, condition);

            if (!stateMachine.CanFire(trigger))
            {
                var permitted = string.Join(", ", await stateMachine.PermittedTriggersAsync ?? []);

                throw new InvalidOperationException($"Cannot fire trigger '{trigger}' in state '{currentState}'. Permitted triggers: {permitted}");
            }

            var fromState = lodCase.CurrentState;
            await stateMachine.FireAsync(trigger);
            var toState = stateMachine.State.ToString();

            // Update case state
            lodCase.CurrentState = toState;
            lodCase.LastModifiedDate = DateTime.UtcNow;

            // Record transition
            _context.TransitionHistory.Add(new StateTransitionHistory
            {
                LodCaseId = caseId,
                FromState = fromState,
                ToState = toState,
                Trigger = trigger.ToString(),
                Timestamp = DateTime.UtcNow,
                PerformedByAuthority = GetCurrentAuthority(Enum.Parse<LodState>(toState)),
                Notes = notes
            });

            await _context.SaveChangesAsync();

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Transition: {fromState} -> {toState} | Trigger: {trigger}");
        }

        public async Task<List<StateTransitionHistory>> GetCaseHistoryAsync(int caseId)
        {
            return await _context.TransitionHistory
                .Where(h => h.LodCaseId == caseId)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();
        }

        public async Task<List<string>> GetPermittedTriggersAsync(int caseId)
        {
            var lodCase = await GetCaseAsync(caseId);

            if (lodCase == null)
            {
                return [];
            }

            var currentState = Enum.Parse<LodState>(lodCase.CurrentState);
            var stateMachine = ConfigureStateMachine(currentState, true);

            return [.. (await stateMachine.PermittedTriggersAsync).Select(t => t.ToString())];
        }

        public string GetCurrentAuthority(LodState state)
        {
            return _stateToAuthorityMap.GetValueOrDefault(state, LodAuthority.None).ToString();
        }

        private static StateMachine<LodState, LodTrigger> ConfigureStateMachine(LodState initialState, bool conditionFlag)
        {
            var stateMachine = new StateMachine<LodState, LodTrigger>(initialState);

            // Configure Start state
            stateMachine.Configure(LodState.Start)
                .Permit(LodTrigger.ProcessInitiated, LodState.MemberReports);

            // Configure MemberReports state
            stateMachine.Configure(LodState.MemberReports)
                .Permit(LodTrigger.ConditionReported, LodState.LodInitiation);

            // Configure LodInitiation state with conditional guard
            stateMachine.Configure(LodState.LodInitiation)
                .PermitIf(LodTrigger.InitiationComplete, LodState.MedicalAssessment, () => conditionFlag);

            // Configure MedicalAssessment state
            stateMachine.Configure(LodState.MedicalAssessment)
                .Permit(LodTrigger.AssessmentDone, LodState.CommanderReview);

            // Configure CommanderReview state with conditional branching
            stateMachine.Configure(LodState.CommanderReview)
                .PermitIf(LodTrigger.ReviewFinished, LodState.OptionalLegal, () => conditionFlag)
                .PermitIf(LodTrigger.ReviewFinished, LodState.BoardAdjudication, () => !conditionFlag);

            // Configure OptionalLegal state with conditional branching
            stateMachine.Configure(LodState.OptionalLegal)
                .PermitIf(LodTrigger.LegalDone, LodState.OptionalWing, () => conditionFlag)
                .PermitIf(LodTrigger.LegalDone, LodState.BoardAdjudication, () => !conditionFlag);

            // Configure OptionalWing state
            stateMachine.Configure(LodState.OptionalWing)
                .Permit(LodTrigger.WingDone, LodState.BoardAdjudication);

            // Configure BoardAdjudication state
            stateMachine.Configure(LodState.BoardAdjudication)
                .Permit(LodTrigger.AdjudicationComplete, LodState.Determination);

            // Configure Determination state
            stateMachine.Configure(LodState.Determination)
                .Permit(LodTrigger.DeterminationFinalized, LodState.Notification);

            // Configure Notification state with conditional branching for appeal
            stateMachine.Configure(LodState.Notification)
                .PermitIf(LodTrigger.AppealRequested, LodState.Appeal, () => conditionFlag)
                .PermitIf(LodTrigger.NoAppealRequested, LodState.End, () => !conditionFlag);

            // Configure Appeal state
            stateMachine.Configure(LodState.Appeal)
                .Permit(LodTrigger.AppealResolved, LodState.End);

            return stateMachine;
        }
    }
}
