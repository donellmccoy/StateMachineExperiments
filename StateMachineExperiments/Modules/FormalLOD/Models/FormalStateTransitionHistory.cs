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
        public FormalLodState FromState { get; set; }
        public FormalLodState ToState { get; set; }
        public FormalLodTrigger Trigger { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public FormalLodAuthority PerformedByAuthority { get; set; }
        public string? Notes { get; set; }
        
        // Navigation property
        public FormalLineOfDuty FormalLodCase { get; set; } = null!;
    }
}
