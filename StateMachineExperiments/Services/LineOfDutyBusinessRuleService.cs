using StateMachineExperiments.Models;
using StateMachineExperiments.Enums;
using StateMachineExperiments.Infrastructure;
using System;
using System.Linq;

namespace StateMachineExperiments.Services
{
    public class LineOfDutyBusinessRuleService : ILineOfDutyBusinessRuleService
    {
        private readonly BusinessRulesSettings _settings;

        public LineOfDutyBusinessRuleService(BusinessRulesSettings settings)
        {
            _settings = settings;
        }

        // ========== INFORMAL LOD BUSINESS RULES ==========
        
        /// <summary>
        /// Business rule for Informal LOD: Legal review required if injury severity > threshold OR estimated cost > threshold
        /// </summary>
        public bool RequiresLegalReview(LineOfDutyCase lodCase)
        {
            if (lodCase.LineOfDutyType != LineOfDutyCaseType.Informal)
                return false;

            return (lodCase.InjurySeverity.HasValue && lodCase.InjurySeverity.Value > _settings.LegalReview.InjurySeverityThreshold) ||
                   (lodCase.EstimatedCost.HasValue && lodCase.EstimatedCost.Value > _settings.LegalReview.CostThreshold);
        }

        /// <summary>
        /// Business rule for Informal LOD: Wing review required if injury severity > threshold OR estimated cost > threshold
        /// </summary>
        public bool RequiresWingReview(LineOfDutyCase lodCase)
        {
            if (lodCase.LineOfDutyType != LineOfDutyCaseType.Informal)
                return false;

            return (lodCase.InjurySeverity.HasValue && lodCase.InjurySeverity.Value > _settings.WingReview.InjurySeverityThreshold) ||
                   (lodCase.EstimatedCost.HasValue && lodCase.EstimatedCost.Value > _settings.WingReview.CostThreshold);
        }

        // ========== FORMAL LOD BUSINESS RULES ==========
        
        /// <summary>
        /// Business rule for Formal LOD: Toxicology required for suspicious circumstances or death cases
        /// </summary>
        public bool RequiresToxicology(LineOfDutyCase lodCase)
        {
            if (lodCase.LineOfDutyType != LineOfDutyCaseType.Formal)
                return false;

            // This would be determined by the investigating officer based on case specifics
            // For now, use the flag set on the case
            return lodCase.ToxicologyRequired;
        }

        /// <summary>
        /// Business rule for Formal LOD: Can proceed from investigation only if toxicology is complete (if required)
        /// </summary>
        public bool CanProceedFromInvestigation(LineOfDutyCase lodCase)
        {
            if (lodCase.LineOfDutyType != LineOfDutyCaseType.Formal)
                return true;

            if (!lodCase.ToxicologyRequired)
            {
                return true; // No toxicology required, can proceed
            }

            return lodCase.ToxicologyComplete; // Can only proceed if toxicology is complete
        }

        // ========== COMMON BUSINESS RULES ==========
        
        /// <summary>
        /// Business rule: Appeal eligibility based on case type
        /// Informal cases: Configurable deadline (default 30 days)
        /// Formal regular cases: 30 days from notification
        /// Formal death cases (next of kin): 180 days from notification
        /// </summary>
        public bool IsAppealEligible(LineOfDutyCase lodCase, DateTime appealDate)
        {
            // Find the notification date from transition history
            var notificationTransition = lodCase.TransitionHistory
                .FirstOrDefault(t => t.ToState == LineOfDutyState.Notification);

            if (notificationTransition == null)
            {
                return false;
            }

            var daysSinceNotification = (appealDate - notificationTransition.Timestamp).TotalDays;
            
            int appealDeadlineDays;
            if (lodCase.LineOfDutyType == LineOfDutyCaseType.Informal)
            {
                appealDeadlineDays = _settings.Appeal.DeadlineDays;
            }
            else // Formal
            {
                appealDeadlineDays = lodCase.IsDeathCase ? 180 : 30;
            }

            return daysSinceNotification <= appealDeadlineDays;
        }

        /// <summary>
        /// Apply all business rules to determine required reviews based on case type
        /// </summary>
        public void ApplyBusinessRules(LineOfDutyCase lodCase)
        {
            if (lodCase.LineOfDutyType == LineOfDutyCaseType.Informal)
            {
                lodCase.RequiresLegalReview = RequiresLegalReview(lodCase);
                lodCase.RequiresWingReview = RequiresWingReview(lodCase);
            }
            // For Formal LOD, most rules are determined during the investigation
        }
    }
}
