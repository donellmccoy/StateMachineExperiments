using StateMachineExperiments.Modules.FormalLOD.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachineExperiments.Modules.FormalLOD.Services
{
    /// <summary>
    /// Data access service for Formal Line of Duty cases.
    /// Provides CRUD operations and persistence for Formal LOD entities.
    /// </summary>
    public interface IFormalLodDataService
    {
        Task<FormalLineOfDuty> CreateNewCaseAsync(string caseNumber, string? memberId = null, string? memberName = null, bool isDeathCase = false);
        Task<FormalLineOfDuty?> GetCaseAsync(int caseId);
        Task<FormalLineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber);
        Task<IEnumerable<FormalLineOfDuty>> GetAllCasesAsync();
        Task UpdateCaseAsync(FormalLineOfDuty lodCase);
        Task AddTransitionHistoryAsync(FormalStateTransitionHistory history);
        Task<List<FormalStateTransitionHistory>> GetCaseHistoryAsync(int caseId);
    }
}
