using StateMachineExperiments.Modules.FormalLOD.Models;
using System;
using System.Linq;

namespace StateMachineExperiments.Modules.FormalLOD.Services
{
    public class FormalLodBusinessRuleService : IFormalLodBusinessRuleService
    {
        // Business rule: Toxicology required for suspicious circumstances or death cases
        public bool RequiresToxicology(FormalLineOfDuty lodCase)
        {
            // This would be determined by the investigating officer based on case specifics
            // For now, use the flag set on the case
            return lodCase.ToxicologyRequired;
        }

        // Business rule: Can proceed from investigation only if toxicology is complete (if required)
        public bool CanProceedFromInvestigation(FormalLineOfDuty lodCase)
        {
            if (!lodCase.ToxicologyRequired)
            {
                return true; // No toxicology required, can proceed
            }

            return lodCase.ToxicologyComplete; // Can only proceed if toxicology is complete
        }

        // Business rule: Appeal eligibility
        // Regular cases: 30 days from notification
        // Death cases (next of kin): 180 days from notification
        public bool IsAppealEligible(FormalLineOfDuty lodCase, DateTime appealDate)
        {
            // Find the notification date from transition history
            var notificationTransition = lodCase.TransitionHistory
                .FirstOrDefault(t => t.ToState == FormalLodState.Notification);

            if (notificationTransition == null)
            {
                return false;
            }

            var daysSinceNotification = (appealDate - notificationTransition.Timestamp).TotalDays;
            var appealDeadlineDays = lodCase.IsDeathCase ? 180 : 30;

            return daysSinceNotification <= appealDeadlineDays;
        }

        // Apply all business rules to determine requirements
        public void ApplyBusinessRules(FormalLineOfDuty lodCase)
        {
            // Business rules can be applied here as needed
            // For Formal LOD, most rules are determined during the investigation
        }
    }
}
