using StateMachineExperiments.Enums;
using System;

namespace StateMachineExperiments.Models
{
    /// <summary>
    /// Represents a single state transition in a Line of Duty case's history.
    /// Provides an immutable audit trail of all state changes for both Informal and Formal LOD cases.
    /// </summary>
    public class LodStateTransitionHistory
    {
        public int Id { get; set; }
        public int LineOfDutyCaseId { get; set; }
        public LodState FromState { get; set; }
        public LodState ToState { get; set; }
        public LodTrigger Trigger { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public LodAuthority PerformedByAuthority { get; set; }
        public string? Notes { get; set; }
        
        // Navigation property
        public LineOfDuty LineOfDutyCase { get; set; } = null!;
    }
}
