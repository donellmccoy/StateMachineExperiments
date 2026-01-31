using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.FormalLOD.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineExperiments.Modules.FormalLOD.Services
{
    public class FormalLodTransitionValidator : IFormalLodTransitionValidator
    {
        private readonly IFormalLodBusinessRuleService _businessRules;

        public FormalLodTransitionValidator(IFormalLodBusinessRuleService businessRules)
        {
            _businessRules = businessRules;
        }

        public Task<ValidationResult> ValidateTransitionAsync(FormalLineOfDuty lodCase, FormalLodTrigger trigger)
        {
            var errors = new List<string>();

            // Validate based on trigger and current state
            var currentState = System.Enum.Parse<FormalLodState>(lodCase.CurrentState);

            switch (trigger)
            {
                case FormalLodTrigger.ProcessInitiated:
                    if (currentState != FormalLodState.Start)
                        errors.Add("Process can only be initiated from Start state");
                    break;

                case FormalLodTrigger.ConditionReported:
                    if (string.IsNullOrEmpty(lodCase.MemberId))
                        errors.Add("Member ID is required before reporting condition");
                    break;

                case FormalLodTrigger.QuestionableDetected:
                    // Validate that questionable circumstances have been documented
                    break;

                case FormalLodTrigger.OfficerAppointed:
                    if (string.IsNullOrEmpty(lodCase.InvestigatingOfficerId))
                        errors.Add("Investigating officer must be assigned before proceeding");
                    break;

                case FormalLodTrigger.InvestigationComplete:
                    if (!_businessRules.CanProceedFromInvestigation(lodCase))
                        errors.Add("Cannot complete investigation: Toxicology reports are required but not yet received");
                    break;

                case FormalLodTrigger.AppealRequested:
                    if (!_businessRules.IsAppealEligible(lodCase, System.DateTime.UtcNow))
                    {
                        var deadline = lodCase.IsDeathCase ? "180 days" : "30 days";
                        errors.Add($"Appeal deadline has passed (must be filed within {deadline} of notification)");
                    }
                    break;

                case FormalLodTrigger.NoAppealRequested:
                    if (currentState != FormalLodState.Notification)
                        errors.Add("Can only close without appeal from Notification state");
                    break;
            }

            return Task.FromResult(errors.Any() 
                ? ValidationResult.Failure(errors.ToArray()) 
                : ValidationResult.Success());
        }
    }
}
