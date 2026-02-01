using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.InformalLOD.Models;

namespace StateMachineExperiments.Modules.InformalLOD.Services
{
    public interface ILodTransitionValidator
    {
        Task<ValidationResult> ValidateTransitionAsync(InformalLineOfDuty lodCase, LodTrigger trigger);
    }
}

