using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.FormalLOD.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachineExperiments.Modules.FormalLOD.Services
{
    /// <summary>
    /// Main service interface for managing Formal Line of Duty cases and state transitions.
    /// </summary>
    public interface IFormalLodStateMachineService
    {
        /// <summary>
        /// Creates a new Formal LOD case with the specified details.
        /// </summary>
        /// <param name="caseNumber">Unique case number identifier.</param>
        /// <param name="memberId">Optional member ID.</param>
        /// <param name="memberName">Optional member name.</param>
        /// <param name="isDeathCase">Whether this is a death case requiring expedited processing.</param>
        /// <returns>The newly created Formal LOD case.</returns>
        Task<FormalLineOfDuty> CreateNewCaseAsync(string caseNumber, string? memberId = null, string? memberName = null, bool isDeathCase = false);
        
        /// <summary>
        /// Retrieves a case by its database ID.
        /// </summary>
        /// <param name="caseId">The database ID of the case.</param>
        /// <returns>The case if found, otherwise null.</returns>
        Task<FormalLineOfDuty?> GetCaseAsync(int caseId);
        
        /// <summary>
        /// Retrieves a case by its case number.
        /// </summary>
        /// <param name="caseNumber">The unique case number.</param>
        /// <returns>The case if found, otherwise null.</returns>
        Task<FormalLineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber);
        
        /// <summary>
        /// Fires a trigger to transition the case to a new state.
        /// </summary>
        /// <param name="caseId">The ID of the case to transition.</param>
        /// <param name="trigger">The trigger to fire.</param>
        /// <param name="notes">Optional notes about the transition.</param>
        Task FireTriggerAsync(int caseId, FormalLodTrigger trigger, string? notes = null);
        
        /// <summary>
        /// Retrieves the complete transition history for a case.
        /// </summary>
        /// <param name="caseId">The ID of the case.</param>
        /// <returns>List of all state transitions for the case.</returns>
        Task<List<FormalStateTransitionHistory>> GetCaseHistoryAsync(int caseId);
        
        /// <summary>
        /// Gets the list of permitted triggers for the case in its current state.
        /// </summary>
        /// <param name="caseId">The ID of the case.</param>
        /// <returns>List of trigger names that can be fired.</returns>
        Task<List<string>> GetPermittedTriggersAsync(int caseId);
        
        /// <summary>
        /// Validates whether a trigger can be fired on a case before attempting the transition.
        /// </summary>
        /// <param name="caseId">The ID of the case.</param>
        /// <param name="trigger">The trigger to validate.</param>
        /// <returns>Validation result indicating success or failure with error messages.</returns>
        Task<ValidationResult> ValidateTransitionAsync(int caseId, FormalLodTrigger trigger);
        
        /// <summary>
        /// Determines the authority level responsible for a given state.
        /// </summary>
        /// <param name="state">The Formal LOD state.</param>
        /// <returns>String representation of the responsible authority.</returns>
        string GetCurrentAuthority(FormalLodState state);
    }
}
