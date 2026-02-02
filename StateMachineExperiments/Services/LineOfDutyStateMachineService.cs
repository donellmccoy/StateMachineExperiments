using StateMachineExperiments.Enums;
using StateMachineExperiments.Infrastructure;
using Stateless;
using StateMachineExperiments.Exceptions;
using StateMachineExperiments.Models;
using StateMachineExperiments.Factories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineExperiments.Services
{
    public class LineOfDutyStateMachineService : ILineOfDutyStateMachineService
    {
        private readonly ILineOfDutyDataService _dataService;
        private readonly ILineOfDutyBusinessRuleService _businessRules;
        private readonly ILineOfDutyTransitionValidator _validator;
        private readonly ILodStateMachineFactory _stateMachineFactory;
        private readonly INotificationService _notificationService;
        private readonly ILogger<LineOfDutyStateMachineService> _logger;
        private readonly Dictionary<LodState, LodAuthority> _stateToAuthorityMap;

        public LineOfDutyStateMachineService(
            ILineOfDutyDataService dataService,
            ILineOfDutyBusinessRuleService businessRules,
            ILineOfDutyTransitionValidator validator,
            ILodStateMachineFactory stateMachineFactory,
            INotificationService notificationService,
            ILogger<LineOfDutyStateMachineService> logger)
        {
            _dataService = dataService;
            _businessRules = businessRules;
            _validator = validator;
            _stateMachineFactory = stateMachineFactory;
            _notificationService = notificationService;
            _logger = logger;

            _stateToAuthorityMap = new Dictionary<LodState, LodAuthority>
            {
                // Common states
                { LodState.Start, LodAuthority.None },
                { LodState.MemberReports, LodAuthority.Member },
                { LodState.BoardAdjudication, LodAuthority.ReviewingBoard },
                { LodState.Determination, LodAuthority.ApprovingAuthority },
                { LodState.Notification, LodAuthority.LodPm },
                { LodState.Appeal, LodAuthority.AppellateAuthority },
                { LodState.End, LodAuthority.None },
                
                // Informal-specific states
                { LodState.LodInitiation, LodAuthority.LodMfp },
                { LodState.MedicalAssessment, LodAuthority.MedicalProvider },
                { LodState.CommanderReview, LodAuthority.ImmediateCommander },
                { LodState.OptionalLegal, LodAuthority.LegalAdvisor },
                { LodState.OptionalWing, LodAuthority.WingCommander },
                
                // Formal-specific states
                { LodState.FormalInitiation, LodAuthority.LodMfp },
                { LodState.AppointingOfficer, LodAuthority.AppointingAuthority },
                { LodState.Investigation, LodAuthority.InvestigatingOfficer },
                { LodState.WingLegalReview, LodAuthority.LegalAdvisor },
                { LodState.WingCommanderReview, LodAuthority.WingCommander }
            };
        }

        public async Task<LineOfDuty> CreateNewCaseAsync(LodType caseType, string caseNumber, string? memberId = null, string? memberName = null, bool isDeathCase = false)
        {
            _logger.LogInformation("Creating new {CaseType} LOD case: {CaseNumber} for member {MemberId} - {MemberName}", 
                caseType, caseNumber, memberId ?? "N/A", memberName ?? "Unknown");

            var lodCase = await _dataService.CreateNewCaseAsync(caseType, caseNumber, memberId, memberName, isDeathCase);

            // Send notification to stakeholders for formal cases
            if (caseType == LodType.Formal)
            {
                await _notificationService.AlertStakeholdersAsync(new StakeholderAlertRequest
                {
                    CaseNumber = caseNumber,
                    AlertType = "New LOD Case Created",
                    Message = $"New {(isDeathCase ? "DEATH" : "formal")} LOD case {caseNumber} created for {memberName ?? "member"} ({memberId ?? "N/A"})",
                    Stakeholders = new[] { "LOD Manager", "Wing Commander" }
                });
            }

            _logger.LogInformation("LOD case created successfully: {CaseNumber} with ID {CaseId}", lodCase.CaseNumber, lodCase.Id);

            return lodCase;
        }

        public async Task<LineOfDuty?> GetCaseAsync(int caseId)
        {
            return await _dataService.GetCaseAsync(caseId);
        }

        public async Task<LineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber)
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

        public async Task FireTriggerAsync(LineOfDuty lodCase, LodTrigger trigger, string? notes = null)
        {
            ArgumentNullException.ThrowIfNull(lodCase);

            _logger.LogDebug("Attempting to fire trigger {Trigger} for case {CaseNumber} (Type: {CaseType})", 
                trigger, lodCase.CaseNumber, lodCase.CaseType);

            var stateMachine = _stateMachineFactory.CreateStateMachine(lodCase);

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

            await _dataService.AddTransitionHistoryAsync(new LodStateTransitionHistory
            {
                LineOfDutyCaseId = lodCase.Id,
                FromState = fromState,
                ToState = stateMachine.State,
                Trigger = trigger,
                Timestamp = DateTime.UtcNow,
                PerformedByAuthority = authority,
                Notes = notes
            });
        }

        public async Task<List<LodStateTransitionHistory>> GetCaseHistoryAsync(int caseId)
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
