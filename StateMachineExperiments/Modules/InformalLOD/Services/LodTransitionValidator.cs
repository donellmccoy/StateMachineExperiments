using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.InformalLOD.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachineExperiments.Modules.InformalLOD.Services
{
    public interface ILodTransitionValidator
    {
        Task<ValidationResult> ValidateTransitionAsync(InformalLineOfDuty lodCase, LodTrigger trigger);
    }

    public class LodTransitionValidator : ILodTransitionValidator
    {
        private readonly ILodBusinessRuleService _businessRules;

        public LodTransitionValidator(ILodBusinessRuleService businessRules)
        {
            _businessRules = businessRules;
        }

        public Task<ValidationResult> ValidateTransitionAsync(InformalLineOfDuty lodCase, LodTrigger trigger)
        {
            var errors = new List<string>();

            // Validate based on trigger and current state
            var currentState = System.Enum.Parse<LodState>(lodCase.CurrentState);

            switch (trigger)
            {
                case LodTrigger.ProcessInitiated:
                    if (currentState != LodState.Start)
                        errors.Add("Process can only be initiated from Start state");
                    break;

                case LodTrigger.ConditionReported:
                    if (string.IsNullOrEmpty(lodCase.MemberId))
                        errors.Add("Member ID is required before reporting condition");
                    break;

                case LodTrigger.InitiationComplete:
                    // Could validate that required documentation is attached
                    break;

                case LodTrigger.ReviewFinished:
                    if (currentState == LodState.CommanderReview)
                    {
                        // Validate that commander has provided their review
                    }
                    break;

                case LodTrigger.AppealFiled:
                    if (!_businessRules.IsAppealEligible(lodCase, System.DateTime.UtcNow))
                        errors.Add("Appeal deadline has passed (must be filed within 30 days of notification)");
                    break;

                case LodTrigger.NotificationComplete:
                    if (currentState != LodState.Notification)
                        errors.Add("Can only complete notification from Notification state");
                    break;
            }

            return Task.FromResult(errors.Any() 
                ? ValidationResult.Failure(errors.ToArray()) 
                : ValidationResult.Success());
        }
    }
}

