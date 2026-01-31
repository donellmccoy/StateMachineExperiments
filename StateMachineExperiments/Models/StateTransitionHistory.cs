using System;

namespace StateMachineExperiments.Models
{
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
