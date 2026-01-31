using System;

namespace StateMachineExperiments.Modules.InformalLOD.Models
{
    /// <summary>
    /// Represents a single state transition in a LOD case's history.
    /// Provides an immutable audit trail of all state changes.
    /// </summary>
    public class StateTransitionHistory
    {
        public int Id { get; set; }
        public int LodCaseId { get; set; }
        public string FromState { get; set; } = string.Empty;
        public string ToState { get; set; } = string.Empty;
        public string Trigger { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? PerformedByAuthority { get; set; }
        public string? Notes { get; set; }
        
        // Navigation property
        public InformalLineOfDuty LodCase { get; set; } = null!;
    }
}
