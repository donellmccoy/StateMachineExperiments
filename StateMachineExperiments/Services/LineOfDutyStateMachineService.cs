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
        private readonly ILineOfDutyStateMachineFactory _stateMachineFactory;
        private readonly INotificationService _notificationService;
        private readonly ILogger<LineOfDutyStateMachineService> _logger;
        private readonly Dictionary<LineOfDutyState, LineOfDutyAuthority> _stateToAuthorityMap;

        public LineOfDutyStateMachineService(
            ILineOfDutyDataService dataService,
            ILineOfDutyBusinessRuleService businessRules,
            ILineOfDutyTransitionValidator validator,
            ILineOfDutyStateMachineFactory stateMachineFactory,
            INotificationService notificationService,
            ILogger<LineOfDutyStateMachineService> logger)
        {
            _dataService = dataService;
            _businessRules = businessRules;
            _validator = validator;
            _stateMachineFactory = stateMachineFactory;
            _notificationService = notificationService;
            _logger = logger;

            _stateToAuthorityMap = new Dictionary<LineOfDutyState, LineOfDutyAuthority>
            {
                // Common states
                { LineOfDutyState.Start, LineOfDutyAuthority.None },
                { LineOfDutyState.MemberReports, LineOfDutyAuthority.Member },
                { LineOfDutyState.BoardAdjudication, LineOfDutyAuthority.ReviewingBoard },
                { LineOfDutyState.Determination, LineOfDutyAuthority.ApprovingAuthority },
                { LineOfDutyState.Notification, LineOfDutyAuthority.LodPm },
                { LineOfDutyState.Appeal, LineOfDutyAuthority.AppellateAuthority },
                { LineOfDutyState.End, LineOfDutyAuthority.None },
                
                // Informal-specific states
                { LineOfDutyState.LodInitiation, LineOfDutyAuthority.LodMfp },
                { LineOfDutyState.MedicalAssessment, LineOfDutyAuthority.MedicalProvider },
                { LineOfDutyState.CommanderReview, LineOfDutyAuthority.ImmediateCommander },
                { LineOfDutyState.OptionalLegal, LineOfDutyAuthority.LegalAdvisor },
                { LineOfDutyState.OptionalWing, LineOfDutyAuthority.WingCommander },
                
                // Formal-specific states
                { LineOfDutyState.FormalInitiation, LineOfDutyAuthority.LodMfp },
                { LineOfDutyState.AppointingOfficer, LineOfDutyAuthority.AppointingAuthority },
                { LineOfDutyState.Investigation, LineOfDutyAuthority.InvestigatingOfficer },
                { LineOfDutyState.WingLegalReview, LineOfDutyAuthority.LegalAdvisor },
                { LineOfDutyState.WingCommanderReview, LineOfDutyAuthority.WingCommander }
            };
        }

        public async Task<LineOfDutyCase> CreateNewCaseAsync(LineOfDutyCaseType caseType, string caseNumber, int memberId, bool isDeathCase = false)
        {
            _logger.LogInformation("Creating new {CaseType} LOD case: {CaseNumber} for member ID {MemberId}", 
                caseType, caseNumber, memberId);

            var lodCase = await _dataService.CreateNewCaseAsync(caseType, caseNumber, memberId, isDeathCase);

            // Fetch member details for notification
            var member = await _dataService.GetMemberAsync(memberId);
            var memberName = member?.Name ?? "Unknown";
            var memberCardId = member?.CardId ?? "N/A";

            // Send notification to stakeholders for formal cases
            if (caseType == LineOfDutyCaseType.Formal)
            {
                await _notificationService.AlertStakeholdersAsync(new StakeholderAlertRequest
                {
                    CaseNumber = caseNumber,
                    AlertType = "New LOD Case Created",
                    Message = $"New {(isDeathCase ? "DEATH" : "formal")} LOD case {caseNumber} created for {memberName} ({memberCardId})",
                    Stakeholders = new[] { "LOD Manager", "Wing Commander" }
                });
            }

            _logger.LogInformation("LOD case created successfully: {CaseNumber} with ID {CaseId}", lodCase.CaseNumber, lodCase.Id);

            return lodCase;
        }

        public async Task<LineOfDutyCase?> GetCaseAsync(int caseId)
        {
            return await _dataService.GetCaseAsync(caseId);
        }

        public async Task<LineOfDutyCase?> GetCaseByCaseNumberAsync(string caseNumber)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(caseNumber);
            
            return await _dataService.GetCaseByCaseNumberAsync(caseNumber);
        }

        public async Task FireTriggerAsync(int caseId, LineOfDutyTrigger trigger, string? notes = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(caseId);

            _logger.LogDebug("Attempting to fire trigger {Trigger} for case ID {CaseId}", trigger, caseId);

            var lodCase = await GetCaseAsync(caseId) ?? throw new CaseNotFoundException(caseId);

            await FireTriggerAsync(lodCase, trigger, notes);
        }

        public async Task FireTriggerAsync(LineOfDutyCase lodCase, LineOfDutyTrigger trigger, string? notes = null)
        {
            ArgumentNullException.ThrowIfNull(lodCase);

            _logger.LogDebug("Attempting to fire trigger {Trigger} for case {CaseNumber} (Type: {CaseType})", 
                trigger, lodCase.CaseNumber, lodCase.LineOfDutyType);

            var stateMachine = _stateMachineFactory.CreateStateMachine(lodCase);

            var fromState = lodCase.LineOfDutyState;

            await stateMachine.FireAsync(trigger);

            _logger.LogInformation("State transition successful for case {CaseNumber}: {FromState} -> {ToState} via {Trigger}", 
                lodCase.CaseNumber, fromState, stateMachine.State, trigger);

            // Update case state
            lodCase.LineOfDutyState = stateMachine.State;
            lodCase.LastModifiedDate = DateTime.UtcNow;
            await _dataService.UpdateCaseAsync(lodCase);

            // Record transition
            var authority = GetCurrentAuthority(stateMachine.State);

            await _dataService.AddTransitionHistoryAsync(new LineOfDutyStateTransitionHistory
            {
                LineOfDutyCaseId = lodCase.Id,
                FromState = fromState,
                ToState = stateMachine.State,
                Trigger = trigger,
                Timestamp = DateTime.UtcNow,
                PerformedByAuthority = authority,
                Description = notes
            });
        }

        public async Task<List<LineOfDutyStateTransitionHistory>> GetCaseHistoryAsync(int caseId)
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

        public async Task<bool> CanFireAsync(int caseId, LineOfDutyTrigger trigger)
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

        public async Task<ValidationResult> ValidateTransitionAsync(int caseId, LineOfDutyTrigger trigger)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(caseId);

            var lodCase = await GetCaseAsync(caseId) ?? throw new CaseNotFoundException(caseId);

            return await _validator.ValidateTransitionAsync(lodCase, trigger);
        }

        public LineOfDutyAuthority GetCurrentAuthority(LineOfDutyState state)
        {
            return _stateToAuthorityMap.GetValueOrDefault(state, LineOfDutyAuthority.None);
        }
    }
}
