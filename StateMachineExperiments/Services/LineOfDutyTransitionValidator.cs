using StateMachineExperiments.Enums;
using StateMachineExperiments.Infrastructure;
using StateMachineExperiments.Models;
using System.Threading.Tasks;

namespace StateMachineExperiments.Services
{
    public class LineOfDutyTransitionValidator : ILineOfDutyTransitionValidator
    {
        private readonly ILineOfDutyBusinessRuleService _businessRules;

        public LineOfDutyTransitionValidator(ILineOfDutyBusinessRuleService businessRules)
        {
            _businessRules = businessRules;
        }

        public Task<ValidationResult> ValidateTransitionAsync(LineOfDuty lodCase, LodTrigger trigger)
        {
            var result = new ValidationResult { IsValid = true };

            // Case type-specific validation
            if (lodCase.CaseType == LodType.Formal)
            {
                return ValidateFormalTransition(lodCase, trigger);
            }
            else
            {
                return ValidateInformalTransition(lodCase, trigger);
            }
        }

        private Task<ValidationResult> ValidateInformalTransition(LineOfDuty lodCase, LodTrigger trigger)
        {
            // Add Informal-specific validation rules here
            // For example, checking if legal review is required before proceeding
            if (trigger == LodTrigger.SkipToAdjudication && lodCase.RequiresLegalReview)
            {
                return Task.FromResult(ValidationResult.Failure("Cannot skip to adjudication when legal review is required."));
            }

            return Task.FromResult(ValidationResult.Success());
        }

        private Task<ValidationResult> ValidateFormalTransition(LineOfDuty lodCase, LodTrigger trigger)
        {
            // Add Formal-specific validation rules here
            // For example, checking if toxicology is complete before proceeding from investigation
            if (trigger == LodTrigger.InvestigationComplete && !_businessRules.CanProceedFromInvestigation(lodCase))
            {
                return Task.FromResult(ValidationResult.Failure("Cannot complete investigation until required toxicology reports are received."));
            }

            return Task.FromResult(ValidationResult.Success());
        }
    }
}
