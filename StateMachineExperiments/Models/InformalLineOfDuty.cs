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
        
        // Business decision flags - evaluated dynamically during workflow
        public bool RequiresLegalReview { get; set; }
        public bool RequiresWingReview { get; set; }
        public bool AppealFiled { get; set; }
        public decimal? EstimatedCost { get; set; }
        public int? InjurySeverity { get; set; }
        
        // Navigation property
        public ICollection<StateTransitionHistory> TransitionHistory { get; set; } = [];
    }
}
