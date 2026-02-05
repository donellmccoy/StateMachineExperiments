using StateMachineExperiments.Enums;
using System;

namespace StateMachineExperiments.Models
{
    /// <summary>
    /// Represents a single state transition in a Line of Duty case's history.
    /// Provides an immutable audit trail of all state changes for both Informal and Formal LOD cases.
    /// </summary>
    public class LineOfDutyStateTransitionHistory
    {
        /// <summary>
        /// Gets or sets the unique database identifier.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the Line of Duty case ID (foreign key).
        /// </summary>
        public int LineOfDutyCaseId { get; set; }
        
        /// <summary>
        /// Gets or sets the state transitioned from.
        /// </summary>
        public LineOfDutyState FromState { get; set; }
        
        /// <summary>
        /// Gets or sets the state transitioned to.
        /// </summary>
        public LineOfDutyState ToState { get; set; }
        
        /// <summary>
        /// Gets or sets the trigger that caused the transition.
        /// </summary>
        public LineOfDutyTrigger Trigger { get; set; }
        
        /// <summary>
        /// Gets or sets the timestamp when the transition occurred.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets the authority that performed this transition.
        /// </summary>
        public LineOfDutyAuthority PerformedByAuthority { get; set; }
        
        /// <summary>
        /// Gets or sets an optional description of the transition.
        /// </summary>
        public string? Description { get; set; }
        
        // ========== NAVIGATION PROPERTIES ==========
        
        /// <summary>
        /// Gets or sets the associated Line of Duty case.
        /// </summary>
        public LineOfDutyCase LineOfDutyCase { get; set; } = null!;
    }
}
