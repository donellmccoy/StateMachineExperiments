using Stateless;
using StateMachineExperiments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineExperiments.Services
{
    public class LodStateMachineService : ILodStateMachineService
    {
        private readonly ILodDataService _dataService;
        private readonly Dictionary<LodState, LodAuthority> _stateToAuthorityMap;

        public LodStateMachineService(ILodDataService dataService)
        {
            _dataService = dataService;

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
            return await _dataService.CreateNewCaseAsync(caseNumber, memberId, memberName);
        }

        public async Task<InformalLineOfDuty?> GetCaseAsync(int caseId)
        {
            return await _dataService.GetCaseAsync(caseId);
        }

        public async Task<InformalLineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber)
        {
            return await _dataService.GetCaseByCaseNumberAsync(caseNumber);
        }

        public async Task FireTriggerAsync(int caseId, LodTrigger trigger, string? notes = null)
        {
            var lodCase = await GetCaseAsync(caseId) ?? throw new InvalidOperationException($"Case with ID {caseId} not found.");

            var currentState = Enum.Parse<LodState>(lodCase.CurrentState);
            var stateMachine = ConfigureStateMachine(currentState, lodCase);

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
            await _dataService.UpdateCaseAsync(lodCase);

            // Record transition
            await _dataService.AddTransitionHistoryAsync(new StateTransitionHistory
            {
                LodCaseId = caseId,
                FromState = fromState,
                ToState = toState,
                Trigger = trigger.ToString(),
                Timestamp = DateTime.UtcNow,
                PerformedByAuthority = GetCurrentAuthority(Enum.Parse<LodState>(toState)),
                Notes = notes
            });

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Transition: {fromState} -> {toState} | Trigger: {trigger}");
        }

        public async Task<List<StateTransitionHistory>> GetCaseHistoryAsync(int caseId)
        {
            return await _dataService.GetCaseHistoryAsync(caseId);
        }

        public async Task<List<string>> GetPermittedTriggersAsync(int caseId)
        {
            var lodCase = await GetCaseAsync(caseId);

            if (lodCase == null)
            {
                return [];
            }

            var currentState = Enum.Parse<LodState>(lodCase.CurrentState);
            var stateMachine = ConfigureStateMachine(currentState, lodCase);

            return [.. (await stateMachine.PermittedTriggersAsync).Select(t => t.ToString())];
        }

        public string GetCurrentAuthority(LodState state)
        {
            return _stateToAuthorityMap.GetValueOrDefault(state, LodAuthority.None).ToString();
        }

        private static StateMachine<LodState, LodTrigger> ConfigureStateMachine(LodState initialState, InformalLineOfDuty lodCase)
        {
            var stateMachine = new StateMachine<LodState, LodTrigger>(initialState);

            // Configure Start state
            stateMachine.Configure(LodState.Start)
                .Permit(LodTrigger.ProcessInitiated, LodState.MemberReports);

            // Configure MemberReports state
            stateMachine.Configure(LodState.MemberReports)
                .Permit(LodTrigger.ConditionReported, LodState.LodInitiation);

            // Configure LodInitiation state - always proceeds to medical assessment
            stateMachine.Configure(LodState.LodInitiation)
                .Permit(LodTrigger.InitiationComplete, LodState.MedicalAssessment);

            // Configure MedicalAssessment state
            stateMachine.Configure(LodState.MedicalAssessment)
                .Permit(LodTrigger.AssessmentDone, LodState.CommanderReview);

            // Configure CommanderReview state - dynamic routing based on case requirements
            stateMachine.Configure(LodState.CommanderReview)
                .PermitIf(LodTrigger.ReviewFinished, LodState.OptionalLegal, () => lodCase.RequiresLegalReview)
                .Permit(LodTrigger.SkipToAdjudication, LodState.BoardAdjudication);

            // Configure OptionalLegal state - dynamic routing based on wing review requirement
            stateMachine.Configure(LodState.OptionalLegal)
                .PermitIf(LodTrigger.LegalDone, LodState.OptionalWing, () => lodCase.RequiresWingReview)
                .Permit(LodTrigger.SkipWingReview, LodState.BoardAdjudication);

            // Configure OptionalWing state
            stateMachine.Configure(LodState.OptionalWing)
                .Permit(LodTrigger.WingDone, LodState.BoardAdjudication);

            // Configure BoardAdjudication state
            stateMachine.Configure(LodState.BoardAdjudication)
                .Permit(LodTrigger.AdjudicationComplete, LodState.Determination);

            // Configure Determination state
            stateMachine.Configure(LodState.Determination)
                .Permit(LodTrigger.DeterminationFinalized, LodState.Notification);

            // Configure Notification state - separate triggers for appeal vs no appeal
            stateMachine.Configure(LodState.Notification)
                .Permit(LodTrigger.AppealFiled, LodState.Appeal)
                .Permit(LodTrigger.NotificationComplete, LodState.End);

            // Configure Appeal state
            stateMachine.Configure(LodState.Appeal)
                .Permit(LodTrigger.AppealResolved, LodState.End);

            return stateMachine;
        }
    }
}
