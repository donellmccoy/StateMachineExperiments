using System;

namespace StateMachineExperiments.Modules.FormalLOD.Models
{
    /// <summary>
    /// Represents a single state transition in a Formal LOD case's history.
    /// Provides an immutable audit trail of all state changes.
    /// </summary>
    public class FormalStateTransitionHistory
    {
        public int Id { get; set; }
        public int FormalLodCaseId { get; set; }
        public string FromState { get; set; } = string.Empty;
        public string ToState { get; set; } = string.Empty;
        public string Trigger { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? PerformedByAuthority { get; set; }
        public string? Notes { get; set; }
        
        // Navigation property
        public FormalLineOfDuty FormalLodCase { get; set; } = null!;
    }
}
