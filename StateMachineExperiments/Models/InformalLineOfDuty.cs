using System;
using System.Collections.Generic;

namespace StateMachineExperiments.Models
{
    public class InformalLineOfDuty
    {
        public int Id { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string CurrentState { get; set; } = nameof(LodState.Start);
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
        public string? MemberId { get; set; }
        public string? MemberName { get; set; }
        
        // Navigation property
        public ICollection<StateTransitionHistory> TransitionHistory { get; set; } = [];
    }
}
