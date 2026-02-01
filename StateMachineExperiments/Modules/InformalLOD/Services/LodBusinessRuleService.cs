using StateMachineExperiments.Modules.InformalLOD.Models;
using StateMachineExperiments.Common.Infrastructure;
using System;
using System.Linq;

namespace StateMachineExperiments.Modules.InformalLOD.Services
{
    public class LodBusinessRuleService : ILodBusinessRuleService
    {
        private readonly BusinessRulesSettings _settings;

        public LodBusinessRuleService(BusinessRulesSettings settings)
        {
            _settings = settings;
        }

        // Business rule: Legal review required if injury severity > threshold OR estimated cost > threshold
        public bool RequiresLegalReview(InformalLineOfDuty lodCase)
        {
            return (lodCase.InjurySeverity.HasValue && lodCase.InjurySeverity.Value > _settings.LegalReview.InjurySeverityThreshold) ||
                   (lodCase.EstimatedCost.HasValue && lodCase.EstimatedCost.Value > _settings.LegalReview.CostThreshold);
        }

        // Business rule: Wing review required if injury severity > threshold OR estimated cost > threshold
        public bool RequiresWingReview(InformalLineOfDuty lodCase)
        {
            return (lodCase.InjurySeverity.HasValue && lodCase.InjurySeverity.Value > _settings.WingReview.InjurySeverityThreshold) ||
                   (lodCase.EstimatedCost.HasValue && lodCase.EstimatedCost.Value > _settings.WingReview.CostThreshold);
        }

        // Business rule: Appeal must be filed within configured deadline days of notification
        public bool IsAppealEligible(InformalLineOfDuty lodCase, DateTime appealDate)
        {
            // Find the notification date from transition history
            var notificationTransition = lodCase.TransitionHistory
                .FirstOrDefault(t => t.ToState == LodState.Notification);

            if (notificationTransition == null)
            {
                return false;
            }

            var daysSinceNotification = (appealDate - notificationTransition.Timestamp).TotalDays;
            return daysSinceNotification <= _settings.Appeal.DeadlineDays;
        }

        // Apply all business rules to determine required reviews
        public void ApplyBusinessRules(InformalLineOfDuty lodCase)
        {
            lodCase.RequiresLegalReview = RequiresLegalReview(lodCase);
            lodCase.RequiresWingReview = RequiresWingReview(lodCase);
        }
    }
}
