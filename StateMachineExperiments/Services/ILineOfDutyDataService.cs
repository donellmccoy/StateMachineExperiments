using StateMachineExperiments.Models;
using StateMachineExperiments.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachineExperiments.Services
{
    /// <summary>
    /// Data access service for Line of Duty cases (both Informal and Formal).
    /// Provides CRUD operations and persistence for LOD entities.
    /// </summary>
    public interface ILineOfDutyDataService
    {
        Task<LineOfDuty> CreateNewCaseAsync(LodType caseType, string caseNumber, string? memberId = null, string? memberName = null, bool isDeathCase = false);
        Task<LineOfDuty?> GetCaseAsync(int caseId);
        Task<LineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber);
        Task<IEnumerable<LineOfDuty>> GetAllCasesAsync();
        Task<IEnumerable<LineOfDuty>> GetCasesByTypeAsync(LodType caseType);
        Task UpdateCaseAsync(LineOfDuty lodCase);
        Task AddTransitionHistoryAsync(LodStateTransitionHistory history);
        Task<List<LodStateTransitionHistory>> GetTransitionHistoryAsync(int caseId);
    }
}
