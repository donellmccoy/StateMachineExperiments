using StateMachineExperiments.Enums;
using StateMachineExperiments.Infrastructure;
using StateMachineExperiments.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateMachineExperiments.Services
{
    /// <summary>
    /// Main service interface for managing Line of Duty cases and state transitions (both Informal and Formal).
    /// </summary>
    public interface ILineOfDutyStateMachineService
    {
        /// <summary>
        /// Creates a new LOD case with the specified details.
        /// </summary>
        /// <param name="caseType">The type of case (Informal or Formal).</param>
        /// <param name="caseNumber">Unique case number identifier.</param>
        /// <param name="memberId">Optional member ID.</param>
        /// <param name="memberName">Optional member name.</param>
        /// <param name="isDeathCase">Whether this is a death case (Formal cases only).</param>
        /// <returns>The newly created LOD case.</returns>
        Task<LineOfDuty> CreateNewCaseAsync(LodType caseType, string caseNumber, string? memberId = null, string? memberName = null, bool isDeathCase = false);
        
        /// <summary>
        /// Retrieves a case by its database ID.
        /// </summary>
        /// <param name="caseId">The database ID of the case.</param>
        /// <returns>The case if found, otherwise null.</returns>
        Task<LineOfDuty?> GetCaseAsync(int caseId);
        
        /// <summary>
        /// Retrieves a case by its case number.
        /// </summary>
        /// <param name="caseNumber">The unique case number.</param>
        /// <returns>The case if found, otherwise null.</returns>
        Task<LineOfDuty?> GetCaseByCaseNumberAsync(string caseNumber);
        
        /// <summary>
        /// Fires a trigger to transition the case to a new state.
        /// </summary>
        /// <param name="caseId">The ID of the case to transition.</param>
        /// <param name="trigger">The trigger to fire.</param>
        /// <param name="notes">Optional notes about the transition.</param>
        Task FireTriggerAsync(int caseId, LodTrigger trigger, string? notes = null);
        
        /// <summary>
        /// Fires a trigger to transition the case to a new state.
        /// </summary>
        /// <param name="lodCase">The LOD case instance to transition.</param>
        /// <param name="trigger">The trigger to fire.</param>
        /// <param name="notes">Optional notes about the transition.</param>
        Task FireTriggerAsync(LineOfDuty lodCase, LodTrigger trigger, string? notes = null);
        
        /// <summary>
        /// Retrieves the complete transition history for a case.
        /// </summary>
        /// <param name="caseId">The ID of the case.</param>
        /// <returns>List of all state transitions for the case.</returns>
        Task<List<LodStateTransitionHistory>> GetCaseHistoryAsync(int caseId);
        
        /// <summary>
        /// Gets the list of permitted triggers for the case in its current state.
        /// </summary>
        /// <param name="caseId">The ID of the case.</param>
        /// <returns>List of trigger names that can be fired.</returns>
        Task<List<string>> GetPermittedTriggersAsync(int caseId);
        
        /// <summary>
        /// Checks if a specific trigger can be fired for a case in its current state.
        /// </summary>
        /// <param name="caseId">The ID of the case.</param>
        /// <param name="trigger">The trigger to check.</param>
        /// <returns>True if the trigger can be fired, otherwise false.</returns>
        Task<bool> CanFireAsync(int caseId, LodTrigger trigger);
        
        /// <summary>
        /// Validates whether a trigger can be fired on a case before attempting the transition.
        /// </summary>
        /// <param name="caseId">The ID of the case.</param>
        /// <param name="trigger">The trigger to validate.</param>
        /// <returns>Validation result indicating success or failure with error messages.</returns>
        Task<ValidationResult> ValidateTransitionAsync(int caseId, LodTrigger trigger);
        
        /// <summary>
        /// Determines the authority level responsible for a given state.
        /// </summary>
        /// <param name="state">The LOD state.</param>
        /// <returns>The responsible authority.</returns>
        LodAuthority GetCurrentAuthority(LodState state);
    }
}
