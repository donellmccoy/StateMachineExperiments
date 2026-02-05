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
        Task<LineOfDutyCase> CreateNewCaseAsync(LineOfDutyType caseType, string caseNumber, int memberId, bool isDeathCase = false);
        Task<LineOfDutyCase?> GetCaseAsync(int caseId);
        Task<LineOfDutyCase?> GetCaseByCaseNumberAsync(string caseNumber);
        Task<IEnumerable<LineOfDutyCase>> GetAllCasesAsync();
        Task<IEnumerable<LineOfDutyCase>> GetCasesByTypeAsync(LineOfDutyType caseType);
        Task UpdateCaseAsync(LineOfDutyCase lodCase);
        Task AddTransitionHistoryAsync(LineOfDutyStateTransitionHistory history);
        Task<List<LineOfDutyStateTransitionHistory>> GetTransitionHistoryAsync(int caseId);
        
        // Member management methods
        Task<Member> CreateMemberAsync(string cardId, string name, string? rank = null, string? unit = null, string? email = null, string? phone = null);
        Task<Member?> GetMemberAsync(int memberId);
        Task<Member?> GetMemberByCardIdAsync(string cardId);
        Task<IEnumerable<Member>> GetAllMembersAsync();
        Task UpdateMemberAsync(Member member);
    }
}
