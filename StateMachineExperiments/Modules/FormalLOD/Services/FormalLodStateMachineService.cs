using StateMachineExperiments.Common.Infrastructure;
using Stateless;
using StateMachineExperiments.Modules.FormalLOD.Events;
using StateMachineExperiments.Common.Exceptions;
using StateMachineExperiments.Modules.FormalLOD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineExperiments.Modules.FormalLOD.Services
{
    public class FormalLodStateMachineService : IFormalLodStateMachineService
    {
        private readonly IFormalLodDataService _dataService;
        private readonly IFormalLodBusinessRuleService _businessRules;
        private readonly IFormalLodTransitionValidator _validator;
        private readonly IFormalLodStateMachineFactory _stateMachineFactory;
        private readonly INotificationService _notificationService;
        private readonly Dictionary<FormalLodState, FormalLodAuthority> _stateToAuthorityMap;

        public FormalLodStateMachineService(
            IFormalLodDataService dataService,
            IFormalLodBusinessRuleService businessRules,
            IFormalLodTransitionValidator validator,
            IFormalLodStateMachineFactory stateMachineFactory,
            INotificationService notificationService)
        {
            _dataService = dataService;
            _businessRules = businessRules;
            _validator = validator;
            _stateMachineFactory = stateMachineFactory;
            _notificationService = notificationService;

            _stateToAuthorityMap = new Dictionary<FormalLodState, FormalLodAuthority>
            {
                { FormalLodState.Start, FormalLodAuthority.None },
                { FormalLodState.MemberReports, FormalLodAuthority.Member },
                { FormalLodState.FormalInitiation, FormalLodAuthority.LodMfp },
                { FormalLodState.AppointingOfficer, FormalLodAuthority.AppointingAuthority },
                { FormalLodState.Investigation, FormalLodAuthority.InvestigatingOfficer },
                { FormalLodState.WingLegalReview, FormalLodAuthority.LegalAdvisor },
                { FormalLodState.WingCommanderReview, FormalLodAuthority.WingCommander },
                { FormalLodState.BoardAdjudication, FormalLodAuthority.ReviewingBoard },
                { FormalLodState.Determination, FormalLodAuthority.ApprovingAuthority },
                { FormalLodState.Notification, FormalLodAuthority.LodPm },
                { FormalLodState.Appeal, FormalLodAuthority.AppellateAuthority },
                { FormalLodState.End, FormalLodAuthority.None }
            };
        }

        public async Task<FormalLineOfDuty> CreateNewCaseAsync(string caseNumber, string? memberId = null, string? memberName = null, bool isDeathCase = false)
        {
            var lodCase = await _dataService.CreateNewCaseAsync(caseNumber, memberId, memberName, isDeathCase);

            // Send notification to stakeholders
            await _notificationService.AlertStakeholdersAsync(new StakeholderAlertRequest
            {
                CaseNumber = caseNumber,
                AlertType = "New LOD Case Created",
                Message = $"New {(isDeathCase ? "DEATH" : "formal")} LOD case {caseNumber} created for {memberName ?? "member"} ({memberId ?? "N/A"})",
                Stakeholders = new[] { "LOD Manager", "Wing Commander" }
            });

            return lodCase;
        }

        public async Task<FormalLineOfDuty?> GetCaseAsync(int caseId)
        {
            return await _dataService.GetCaseAsync(caseId);
        }

        public async Task<FormalLineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber)
        {
            return await _dataService.GetCaseByCaseNumberAsync(caseNumber);
        }

        public async Task FireTriggerAsync(int caseId, FormalLodTrigger trigger, string? notes = null)
        {
            var lodCase = await GetCaseAsync(caseId) ?? throw new CaseNotFoundException(caseId);

            // Validate the transition
            var validationResult = await _validator.ValidateTransitionAsync(lodCase, trigger);
            if (!validationResult.IsValid)
            {
                throw new TransitionValidationException(string.Join("; ", validationResult.Errors));
            }

            var stateMachine = _stateMachineFactory.CreateStateMachine(lodCase);

            if (!stateMachine.CanFire(trigger))
            {
                var permitted = (await stateMachine.PermittedTriggersAsync).Select(t => t.ToString()).ToArray();
                throw new InvalidStateTransitionException(lodCase.CurrentState.ToString(), trigger.ToString(), permitted);
            }

            var fromState = lodCase.CurrentState;
            await stateMachine.FireAsync(trigger);
            var toState = stateMachine.State;

            // Update case state
            lodCase.CurrentState = toState;
            lodCase.LastModifiedDate = DateTime.UtcNow;
            await _dataService.UpdateCaseAsync(lodCase);

            // Record transition
            var authority = GetCurrentAuthority(toState);
            await _dataService.AddTransitionHistoryAsync(new FormalStateTransitionHistory
            {
                FormalLodCaseId = caseId,
                FromState = fromState,
                ToState = toState,
                Trigger = trigger,
                Timestamp = DateTime.UtcNow,
                PerformedByAuthority = authority,
                Notes = notes
            });

            // Send notification about state change
            await _notificationService.NotifyAsync(new NotificationRequest
            {
                Recipient = "LOD Manager",
                Subject = $"State Transition - Case {lodCase.CaseNumber}",
                Message = $"Case {lodCase.CaseNumber} transitioned from {fromState} to {toState} via trigger {trigger}. Authority: {authority}",
                NotificationType = "System"
            });

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Transition: {fromState} -> {toState} | Trigger: {trigger}");
        }

        public async Task<List<FormalStateTransitionHistory>> GetCaseHistoryAsync(int caseId)
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

            var stateMachine = _stateMachineFactory.CreateStateMachine(lodCase);
            return [.. (await stateMachine.PermittedTriggersAsync).Select(t => t.ToString())];
        }

        public async Task<ValidationResult> ValidateTransitionAsync(int caseId, FormalLodTrigger trigger)
        {
            var lodCase = await GetCaseAsync(caseId) ?? throw new CaseNotFoundException(caseId);
            return await _validator.ValidateTransitionAsync(lodCase, trigger);
        }

        public FormalLodAuthority GetCurrentAuthority(FormalLodState state)
        {
            return _stateToAuthorityMap.GetValueOrDefault(state, FormalLodAuthority.None);
        }
    }
}
