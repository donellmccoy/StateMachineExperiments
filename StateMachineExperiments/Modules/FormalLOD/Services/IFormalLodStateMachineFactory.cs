using Stateless;
using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.FormalLOD.Models;

namespace StateMachineExperiments.Modules.FormalLOD.Services
{
    public interface IFormalLodStateMachineFactory
    {
        StateMachine<FormalLodState, FormalLodTrigger> CreateStateMachine(FormalLineOfDuty lodCase);
    }
}
