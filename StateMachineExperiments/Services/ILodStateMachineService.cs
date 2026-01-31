using StateMachineExperiments.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachineExperiments.Services
{
    public interface ILodStateMachineService
    {
        Task<InformalLineOfDuty> CreateNewCaseAsync(string caseNumber, string? memberId = null, string? memberName = null);
        Task<InformalLineOfDuty?> GetCaseAsync(int caseId);
        Task<InformalLineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber);
        Task FireTriggerAsync(int caseId, LodTrigger trigger, string? notes = null);
        Task<List<StateTransitionHistory>> GetCaseHistoryAsync(int caseId);
        Task<List<string>> GetPermittedTriggersAsync(int caseId);
        string GetCurrentAuthority(LodState state);
    }
}
