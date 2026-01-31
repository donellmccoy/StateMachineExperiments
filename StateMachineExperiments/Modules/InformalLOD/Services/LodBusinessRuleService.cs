using StateMachineExperiments.Modules.InformalLOD.Models;
using System;
using System.Linq;

namespace StateMachineExperiments.Modules.InformalLOD.Services
{
    public class LodBusinessRuleService : ILodBusinessRuleService
    {
        // Business rule: Legal review required if injury severity > 5 OR estimated cost > $50,000
        public bool RequiresLegalReview(InformalLineOfDuty lodCase)
        {
            return (lodCase.InjurySeverity.HasValue && lodCase.InjurySeverity.Value > 5) ||
                   (lodCase.EstimatedCost.HasValue && lodCase.EstimatedCost.Value > 50000);
        }

        // Business rule: Wing review required if injury severity > 7 OR estimated cost > $100,000
        public bool RequiresWingReview(InformalLineOfDuty lodCase)
        {
            return (lodCase.InjurySeverity.HasValue && lodCase.InjurySeverity.Value > 7) ||
                   (lodCase.EstimatedCost.HasValue && lodCase.EstimatedCost.Value > 100000);
        }

        // Business rule: Appeal must be filed within 30 days of notification
        public bool IsAppealEligible(InformalLineOfDuty lodCase, DateTime appealDate)
        {
            // Find the notification date from transition history
            var notificationTransition = lodCase.TransitionHistory
                .FirstOrDefault(t => t.ToState == nameof(LodState.Notification));

            if (notificationTransition == null)
            {
                return false;
            }

            var daysSinceNotification = (appealDate - notificationTransition.Timestamp).TotalDays;
            return daysSinceNotification <= 30;
        }

        // Apply all business rules to determine required reviews
        public void ApplyBusinessRules(InformalLineOfDuty lodCase)
        {
            lodCase.RequiresLegalReview = RequiresLegalReview(lodCase);
            lodCase.RequiresWingReview = RequiresWingReview(lodCase);
        }
    }
}
