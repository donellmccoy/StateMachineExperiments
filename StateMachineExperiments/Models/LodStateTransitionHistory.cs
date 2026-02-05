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
        public int Id { get; set; }
        public int LineOfDutyCaseId { get; set; }
        public LineOfDutyState FromState { get; set; }
        public LineOfDutyState ToState { get; set; }
        public LineOfDutyTrigger Trigger { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public LodAuthority PerformedByAuthority { get; set; }
        public string? Description { get; set; }
        
        // Navigation property
        public LineOfDutyCase LineOfDutyCase { get; set; } = null!;
    }
}
