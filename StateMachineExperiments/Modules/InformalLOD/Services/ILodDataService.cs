using StateMachineExperiments.Modules.InformalLOD.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachineExperiments.Modules.InformalLOD.Services
{
    /// <summary>
    /// Data access service for Informal Line of Duty cases.
    /// Provides CRUD operations and persistence for LOD entities.
    /// </summary>
    public interface IInformalLineOfDutyDataService
    {
        Task<InformalLineOfDuty> CreateNewCaseAsync(string caseNumber, string memberId, string? memberName = null);
        Task<InformalLineOfDuty?> GetCaseAsync(int caseId);
        Task<InformalLineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber);
        Task UpdateCaseAsync(InformalLineOfDuty lodCase);
        Task AddTransitionHistoryAsync(StateTransitionHistory history);
        Task<List<StateTransitionHistory>> GetTransitionHistoryAsync(int caseId);
    }
}
