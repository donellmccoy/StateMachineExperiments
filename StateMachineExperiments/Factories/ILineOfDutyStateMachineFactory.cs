using Stateless;
using StateMachineExperiments.Enums;
using StateMachineExperiments.Models;

namespace StateMachineExperiments.Factories
{
    /// <summary>
    /// Unified interface for creating state machines for both Informal and Formal Line of Duty cases.
    /// </summary>
    public interface ILineOfDutyStateMachineFactory
    {
        StateMachine<LineOfDutyState, LineOfDutyTrigger> CreateStateMachine(LineOfDutyCase lodCase);
    }
}
