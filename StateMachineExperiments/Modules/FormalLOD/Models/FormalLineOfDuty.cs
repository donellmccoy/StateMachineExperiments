using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StateMachineExperiments.Modules.FormalLOD.Models
{
    /// <summary>
    /// Represents a Formal Line of Duty case with state machine tracking.
    /// Used when questionable circumstances require formal investigation.
    /// Supports optimistic concurrency control and toxicology tracking.
    /// </summary>
    public class FormalLineOfDuty
    {
        /// <summary>
        /// Gets or sets the unique database identifier.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the unique case number (e.g., FORMAL-LOD-2026-001).
        /// </summary>
        public string CaseNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the current state of the case in the workflow.
        /// </summary>
        public string CurrentState { get; set; } = nameof(FormalLodState.Start);
        
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
        /// Gets or sets whether the case is a death case (requires expedited processing).
        /// </summary>
        public bool IsDeathCase { get; set; }
        
        /// <summary>
        /// Gets or sets whether toxicology reports are required for this case.
        /// </summary>
        public bool ToxicologyRequired { get; set; }
        
        /// <summary>
        /// Gets or sets whether toxicology reports have been received.
        /// </summary>
        public bool ToxicologyComplete { get; set; }
        
        /// <summary>
        /// Gets or sets whether an appeal has been filed for this case.
        /// </summary>
        public bool AppealFiled { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the appointed investigating officer.
        /// </summary>
        public string? InvestigatingOfficerId { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the appointed investigating officer.
        /// </summary>
        public string? InvestigatingOfficerName { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the investigation began.
        /// </summary>
        public DateTime? InvestigationStartDate { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the investigation was completed.
        /// </summary>
        public DateTime? InvestigationCompletionDate { get; set; }
        
        /// <summary>
        /// Gets or sets the final determination result.
        /// </summary>
        public string? DeterminationResult { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of state transitions for this case.
        /// </summary>
        public ICollection<FormalStateTransitionHistory> TransitionHistory { get; set; } = [];
    }
}
