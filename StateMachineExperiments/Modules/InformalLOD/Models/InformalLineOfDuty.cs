using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StateMachineExperiments.Modules.InformalLOD.Models
{
    /// <summary>
    /// Represents an Informal Line of Duty case with state machine tracking.
    /// Supports optimistic concurrency control and dynamic business rule evaluation.
    /// </summary>
    public class InformalLineOfDuty
    {
        /// <summary>
        /// Gets or sets the unique database identifier.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the unique case number (e.g., LOD-2026-001).
        /// </summary>
        public string CaseNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the current state of the case in the workflow.
        /// </summary>
        public LodState CurrentState { get; set; } = LodState.Start;
        
        /// <summary>
        /// Gets or sets the date and time when the case was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets the date and time when the case was last modified.
        /// </summary>
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets the member ID associated with the case.
        /// </summary>
        public string? MemberId { get; set; }
        
        /// <summary>
        /// Gets or sets the member name associated with the case.
        /// </summary>
        public string? MemberName { get; set; }
        
        /// <summary>
        /// Gets or sets the row version for optimistic concurrency control.
        /// Updated automatically by EF Core on each save.
        /// </summary>
        [Timestamp]
        public byte[]? RowVersion { get; set; }
        
        /// <summary>
        /// Gets or sets whether the case requires legal review.
        /// Determined by business rules based on injury severity and cost.
        /// </summary>
        public bool RequiresLegalReview { get; set; }
        
        /// <summary>
        /// Gets or sets whether the case requires wing commander review.
        /// Determined by business rules based on injury severity and cost.
        /// </summary>
        public bool RequiresWingReview { get; set; }
        
        /// <summary>
        /// Gets or sets whether an appeal has been filed for this case.
        /// </summary>
        public bool AppealFiled { get; set; }
        
        /// <summary>
        /// Gets or sets the estimated cost associated with the case.
        /// Used in business rule evaluation.
        /// </summary>
        public decimal? EstimatedCost { get; set; }
        
        /// <summary>
        /// Gets or sets the injury severity rating (1-10 scale).
        /// Used in business rule evaluation.
        /// </summary>
        public int? InjurySeverity { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of state transitions for this case.
        /// </summary>
        public ICollection<StateTransitionHistory> TransitionHistory { get; set; } = [];
    }
}
