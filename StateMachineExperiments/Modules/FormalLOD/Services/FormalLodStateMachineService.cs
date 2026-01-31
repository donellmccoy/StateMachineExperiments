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
        private readonly IEventPublisher _eventPublisher;
        private readonly Dictionary<FormalLodState, FormalLodAuthority> _stateToAuthorityMap;

        public FormalLodStateMachineService(
            IFormalLodDataService dataService,
            IFormalLodBusinessRuleService businessRules,
            IFormalLodTransitionValidator validator,
            IFormalLodStateMachineFactory stateMachineFactory,
            IEventPublisher eventPublisher)
        {
            _dataService = dataService;
            _businessRules = businessRules;
            _validator = validator;
            _stateMachineFactory = stateMachineFactory;
            _eventPublisher = eventPublisher;

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

            // Publish domain event
            _eventPublisher.Publish(new FormalLodCaseCreatedEvent
            {
                CaseId = lodCase.Id,
                CaseNumber = lodCase.CaseNumber,
                MemberId = lodCase.MemberId,
                MemberName = lodCase.MemberName,
                IsDeathCase = lodCase.IsDeathCase
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

            var currentState = Enum.Parse<FormalLodState>(lodCase.CurrentState);
            var stateMachine = _stateMachineFactory.CreateStateMachine(lodCase, _eventPublisher);

            if (!stateMachine.CanFire(trigger))
            {
                var permitted = (await stateMachine.PermittedTriggersAsync).Select(t => t.ToString()).ToArray();
                throw new InvalidStateTransitionException(currentState.ToString(), trigger.ToString(), permitted);
            }

            var fromState = lodCase.CurrentState;
            await stateMachine.FireAsync(trigger);
            var toState = stateMachine.State.ToString();

            // Update case state
            lodCase.CurrentState = toState;
            lodCase.LastModifiedDate = DateTime.UtcNow;
            await _dataService.UpdateCaseAsync(lodCase);

            // Record transition
            var authority = GetCurrentAuthority(Enum.Parse<FormalLodState>(toState));
            await _dataService.AddTransitionHistoryAsync(new FormalStateTransitionHistory
            {
                FormalLodCaseId = caseId,
                FromState = fromState,
                ToState = toState,
                Trigger = trigger.ToString(),
                Timestamp = DateTime.UtcNow,
                PerformedByAuthority = authority,
                Notes = notes
            });

            // Publish state changed event
            _eventPublisher.Publish(new FormalLodStateChangedEvent
            {
                CaseId = caseId,
                CaseNumber = lodCase.CaseNumber,
                FromState = fromState,
                ToState = toState,
                Trigger = trigger.ToString(),
                Authority = authority,
                Notes = notes
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

            var stateMachine = _stateMachineFactory.CreateStateMachine(lodCase, _eventPublisher);
            return [.. (await stateMachine.PermittedTriggersAsync).Select(t => t.ToString())];
        }

        public async Task<ValidationResult> ValidateTransitionAsync(int caseId, FormalLodTrigger trigger)
        {
            var lodCase = await GetCaseAsync(caseId) ?? throw new CaseNotFoundException(caseId);
            return await _validator.ValidateTransitionAsync(lodCase, trigger);
        }

        public string GetCurrentAuthority(FormalLodState state)
        {
            return _stateToAuthorityMap.GetValueOrDefault(state, FormalLodAuthority.None).ToString();
        }
    }
}
