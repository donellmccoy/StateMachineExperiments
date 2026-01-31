using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.FormalLOD.Models;
using System.Threading.Tasks;

namespace StateMachineExperiments.Modules.FormalLOD.Services
{
    public interface IFormalLodTransitionValidator
    {
        Task<ValidationResult> ValidateTransitionAsync(FormalLineOfDuty lodCase, FormalLodTrigger trigger);
    }
}
