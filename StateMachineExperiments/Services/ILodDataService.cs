using StateMachineExperiments.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachineExperiments.Services
{
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
