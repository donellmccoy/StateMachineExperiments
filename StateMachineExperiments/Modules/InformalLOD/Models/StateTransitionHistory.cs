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
        public int CaseId { get; set; }
        public LodState FromState { get; set; }
        public LodState ToState { get; set; }
        public LodTrigger Trigger { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public LodAuthority PerformedByAuthority { get; set; }
        public string? Notes { get; set; }
        
        // Navigation property
        public InformalLineOfDuty LodCase { get; set; } = null!;
    }
}
