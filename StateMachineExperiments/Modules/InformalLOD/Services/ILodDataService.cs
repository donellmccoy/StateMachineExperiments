using StateMachineExperiments.Modules.InformalLOD.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachineExperiments.Modules.InformalLOD.Services
{
    /// <summary>
    /// Data access service for Informal Line of Duty cases.
    /// Provides CRUD operations and persistence for LOD entities.
    /// </summary>
    public interface ILodDataService
    {
        Task<InformalLineOfDuty> CreateNewCaseAsync(string caseNumber, string? memberId = null, string? memberName = null);
        Task<InformalLineOfDuty?> GetCaseAsync(int caseId);
        Task<InformalLineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber);
        Task UpdateCaseAsync(InformalLineOfDuty lodCase);
        Task AddTransitionHistoryAsync(StateTransitionHistory history);
        Task<List<StateTransitionHistory>> GetCaseHistoryAsync(int caseId);
    }
}
