using StateMachineExperiments.Enums;
using StateMachineExperiments.Infrastructure;
using StateMachineExperiments.Models;
using System.Threading.Tasks;

namespace StateMachineExperiments.Services
{
    public interface ILineOfDutyTransitionValidator
    {
        Task<ValidationResult> ValidateTransitionAsync(LineOfDuty lodCase, LodTrigger trigger);
    }
}
