using StateMachineExperiments.Common.Infrastructure;
using Stateless;
using StateMachineExperiments.Modules.InformalLOD.Events;
using StateMachineExperiments.Common.Exceptions;
using StateMachineExperiments.Modules.InformalLOD.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineExperiments.Modules.InformalLOD.Services
{
    public class LodStateMachineService : ILodStateMachineService
    {
        private readonly IInformalLineOfDutyDataService _dataService;
        private readonly ILodBusinessRuleService _businessRules;
        private readonly ILodTransitionValidator _validator;
        private readonly ILodStateMachineFactory _stateMachineFactory;
        private readonly INotificationService _notificationService;
        private readonly ILogger<LodStateMachineService> _logger;
        private readonly Dictionary<LodState, LodAuthority> _stateToAuthorityMap;

        public LodStateMachineService(
            IInformalLineOfDutyDataService dataService,
            ILodBusinessRuleService businessRules,
            ILodTransitionValidator validator,
            ILodStateMachineFactory stateMachineFactory,
            INotificationService notificationService,
            ILogger<LodStateMachineService> logger)
        {
            _dataService = dataService;
            _businessRules = businessRules;
            _validator = validator;
            _stateMachineFactory = stateMachineFactory;
            _notificationService = notificationService;
            _logger = logger;

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
            _logger.LogInformation("Creating new LOD case: {CaseNumber} for member {MemberId} - {MemberName}", 
                caseNumber, memberId ?? "N/A", memberName ?? "Unknown");

            var lodCase = await _dataService.CreateNewCaseAsync(caseNumber, memberId, memberName);

            _logger.LogInformation("LOD case created successfully: {CaseNumber} with ID {CaseId}", 
                lodCase.CaseNumber, lodCase.Id);

            return lodCase;
        }

        public async Task<InformalLineOfDuty?> GetCaseAsync(int caseId)
        {
            return await _dataService.GetCaseAsync(caseId);
        }

        public async Task<InformalLineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(caseNumber);
            
            return await _dataService.GetCaseByCaseNumberAsync(caseNumber);
        }

        public async Task FireTriggerAsync(int caseId, LodTrigger trigger, string? notes = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(caseId);

            _logger.LogDebug("Attempting to fire trigger {Trigger} for case ID {CaseId}", trigger, caseId);

            var lodCase = await GetCaseAsync(caseId) ?? throw new CaseNotFoundException(caseId);

            await FireTriggerAsync(lodCase, trigger, notes);
        }

        public async Task FireTriggerAsync(InformalLineOfDuty lodCase, LodTrigger trigger, string? notes = null)
        {
            ArgumentNullException.ThrowIfNull(lodCase);

            _logger.LogDebug("Attempting to fire trigger {Trigger} for case {CaseNumber}", trigger, lodCase.CaseNumber);

            var stateMachine = _stateMachineFactory.CreateStateMachine(lodCase);

            if (!stateMachine.CanFire(trigger))
            {
                var permitted = (await stateMachine.PermittedTriggersAsync).Select(t => t.ToString()).ToArray();
                
                _logger.LogWarning("Invalid state transition for case {CaseNumber}. Current state: {CurrentState}, Trigger: {Trigger}, Permitted: {Permitted}", 
                    lodCase.CaseNumber, lodCase.CurrentState, trigger, string.Join(", ", permitted));

                throw new InvalidStateTransitionException(lodCase.CurrentState.ToString(), trigger.ToString(), permitted);
            }

            var fromState = lodCase.CurrentState;

            await stateMachine.FireAsync(trigger);

            _logger.LogInformation("State transition successful for case {CaseNumber}: {FromState} -> {ToState} via {Trigger}", 
                lodCase.CaseNumber, fromState, stateMachine.State, trigger);

            // Update case state
            lodCase.CurrentState = stateMachine.State;
            lodCase.LastModifiedDate = DateTime.UtcNow;
            await _dataService.UpdateCaseAsync(lodCase);

            // Record transition
            var authority = GetCurrentAuthority(stateMachine.State);

            await _dataService.AddTransitionHistoryAsync(new StateTransitionHistory
            {
                CaseId = lodCase.Id,
                FromState = fromState,
                ToState = stateMachine.State,
                Trigger = trigger,
                Timestamp = DateTime.UtcNow,
                PerformedByAuthority = authority,
                Notes = notes
            });
        }

        public async Task<List<StateTransitionHistory>> GetCaseHistoryAsync(int caseId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(caseId);
            
            return await _dataService.GetTransitionHistoryAsync(caseId);
        }

        public async Task<List<string>> GetPermittedTriggersAsync(int caseId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(caseId);

            var lodCase = await GetCaseAsync(caseId);

            if (lodCase == null)
            {
                return [];
            }

            var stateMachine = _stateMachineFactory.CreateStateMachine(lodCase);

            return [.. (await stateMachine.PermittedTriggersAsync).Select(t => t.ToString())];
        }

        public async Task<bool> CanFireAsync(int caseId, LodTrigger trigger)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(caseId);

            var lodCase = await GetCaseAsync(caseId);

            if (lodCase == null)
            {
                return false;
            }

            var stateMachine = _stateMachineFactory.CreateStateMachine(lodCase);

            return stateMachine.CanFire(trigger);
        }

        public async Task<ValidationResult> ValidateTransitionAsync(int caseId, LodTrigger trigger)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(caseId);

            var lodCase = await GetCaseAsync(caseId) ?? throw new CaseNotFoundException(caseId);

            return await _validator.ValidateTransitionAsync(lodCase, trigger);
        }

        public LodAuthority GetCurrentAuthority(LodState state)
        {
            return _stateToAuthorityMap.GetValueOrDefault(state, LodAuthority.None);
        }
    }
}

