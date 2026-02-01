using Stateless;
using StateMachineExperiments.Modules.InformalLOD.Models;

namespace StateMachineExperiments.Modules.InformalLOD.Services
{
    public interface ILodStateMachineFactory
    {
        StateMachine<LodState, LodTrigger> CreateStateMachine(InformalLineOfDuty lodCase);
    }
}
