using StateMachineExperiments.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StateMachineExperiments.Models
{
    /// <summary>
    /// Represents a Line of Duty case with state machine tracking.
    /// Supports both Informal and Formal LOD case types through a discriminator enum.
    /// Includes optimistic concurrency control and type-specific business logic.
    /// </summary>
    public class LineOfDuty
    {
        /// <summary>
        /// Gets or sets the unique database identifier.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the discriminator indicating the type of LOD case (Informal or Formal).
        /// </summary>
        public LodType CaseType { get; set; } = LodType.Informal;
        
        /// <summary>
        /// Gets or sets the unique case number (e.g., LOD-2026-001 or FORMAL-LOD-2026-001).
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
        /// Gets or sets whether an appeal has been filed for this case.
        /// Applies to both Informal and Formal cases.
        /// </summary>
        public bool AppealFiled { get; set; }
        
        // ========== INFORMAL LOD SPECIFIC PROPERTIES ==========
        
        /// <summary>
        /// Gets or sets whether the case requires legal review (Informal only).
        /// Determined by business rules based on injury severity and cost.
        /// </summary>
        public bool RequiresLegalReview { get; set; }
        
        /// <summary>
        /// Gets or sets whether the case requires wing commander review (Informal only).
        /// Determined by business rules based on injury severity and cost.
        /// </summary>
        public bool RequiresWingReview { get; set; }
        
        /// <summary>
        /// Gets or sets the estimated cost associated with the case (Informal only).
        /// Used in business rule evaluation.
        /// </summary>
        public decimal? EstimatedCost { get; set; }
        
        /// <summary>
        /// Gets or sets the injury severity rating (1-10 scale, Informal only).
        /// Used in business rule evaluation.
        /// </summary>
        public int? InjurySeverity { get; set; }
        
        // ========== FORMAL LOD SPECIFIC PROPERTIES ==========
        
        /// <summary>
        /// Gets or sets whether the case is a death case (Formal only, requires expedited processing).
        /// </summary>
        public bool IsDeathCase { get; set; }
        
        /// <summary>
        /// Gets or sets whether toxicology reports are required for this case (Formal only).
        /// </summary>
        public bool ToxicologyRequired { get; set; }
        
        /// <summary>
        /// Gets or sets whether toxicology reports have been received (Formal only).
        /// </summary>
        public bool ToxicologyComplete { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the appointed investigating officer (Formal only).
        /// </summary>
        public string? InvestigatingOfficerId { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the appointed investigating officer (Formal only).
        /// </summary>
        public string? InvestigatingOfficerName { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the investigation began (Formal only).
        /// </summary>
        public DateTime? InvestigationStartDate { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the investigation was completed (Formal only).
        /// </summary>
        public DateTime? InvestigationCompletionDate { get; set; }
        
        /// <summary>
        /// Gets or sets the final determination result (Formal only).
        /// </summary>
        public string? DeterminationResult { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of state transitions for this case.
        /// </summary>
        public ICollection<LodStateTransitionHistory> TransitionHistory { get; set; } = [];
    }
}
