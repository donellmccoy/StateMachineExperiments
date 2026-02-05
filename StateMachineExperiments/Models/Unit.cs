using System;

namespace StateMachineExperiments.Models
{
    /// <summary>
    /// Represents unit-specific information associated with a Line of Duty case.
    /// </summary>
    public class Unit
    {
        /// <summary>
        /// Gets or sets the unique database identifier.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the Line of Duty case ID (foreign key).
        /// </summary>
        public int LineOfDutyCaseId { get; set; }
        
        /// <summary>
        /// Gets or sets the commander's circumstance details.
        /// </summary>
        public string? CommanderCircumstanceDetails { get; set; }
        
        /// <summary>
        /// Gets or sets the commander's duty determination.
        /// </summary>
        public string? CommanderDutyDetermination { get; set; }
        
        /// <summary>
        /// Gets or sets the commander's determination of duty start date/time.
        /// </summary>
        public DateTime? CommanderDutyFrom { get; set; }
        
        /// <summary>
        /// Gets or sets other commander duty-related information.
        /// </summary>
        public string? CommanderDutyOthers { get; set; }
        
        /// <summary>
        /// Gets or sets the commander's determination of duty end date/time.
        /// </summary>
        public DateTime? CommanderDutyTo { get; set; }
        
        /// <summary>
        /// Gets or sets whether the commander was activated.
        /// </summary>
        public string? CommanderActivatedYn { get; set; }
        
        /// <summary>
        /// Gets or sets the date when this record was last modified.
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets the ID of the user who last modified this record.
        /// </summary>
        public int ModifiedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the source information identifier.
        /// </summary>
        public int? SourceInformation { get; set; }
        
        /// <summary>
        /// Gets or sets witness information as XML data.
        /// </summary>
        public string? Witnesses { get; set; }
        
        /// <summary>
        /// Gets or sets additional specification for source information.
        /// </summary>
        public string? SourceInformationSpecify { get; set; }
        
        /// <summary>
        /// Gets or sets the member occurrence identifier.
        /// </summary>
        public int? MemberOccurrence { get; set; }
        
        /// <summary>
        /// Gets or sets the date/time when the member was absent from.
        /// </summary>
        public DateTime? AbsentFrom { get; set; }
        
        /// <summary>
        /// Gets or sets the date/time when the member was absent to.
        /// </summary>
        public DateTime? AbsentTo { get; set; }
        
        /// <summary>
        /// Gets or sets whether the member was on orders.
        /// </summary>
        public string? MemberOnOrders { get; set; }
        
        /// <summary>
        /// Gets or sets the member's credibility status.
        /// </summary>
        public string? MemberCredible { get; set; }
        
        /// <summary>
        /// Gets or sets the proximate cause identifier.
        /// </summary>
        public int? ProximateCause { get; set; }
        
        /// <summary>
        /// Gets or sets additional specification for proximate cause.
        /// </summary>
        public string? ProximateCauseSpecify { get; set; }
        
        /// <summary>
        /// Gets or sets the workflow identifier.
        /// </summary>
        public int? Workflow { get; set; }
        
        /// <summary>
        /// Gets or sets the travel occurrence identifier.
        /// </summary>
        public int? TravelOccurrence { get; set; }
        
        /// <summary>
        /// Gets or sets whether the status has been verified.
        /// </summary>
        public bool? VerifiedStatus { get; set; }
        
        /// <summary>
        /// Gets or sets whether proof of status exists.
        /// </summary>
        public bool? ProofOfStatus { get; set; }
        
        /// <summary>
        /// Gets or sets whether LOD initiation has occurred.
        /// </summary>
        public bool? LodInitiation { get; set; }
        
        /// <summary>
        /// Gets or sets whether a written diagnosis was provided.
        /// </summary>
        public bool? WrittenDiagnosis { get; set; }
        
        /// <summary>
        /// Gets or sets whether this was a member-initiated request.
        /// </summary>
        public bool? MemberRequest { get; set; }
        
        /// <summary>
        /// Gets or sets whether the condition existed prior to duty status.
        /// </summary>
        public bool? PriorToDutyStatus { get; set; }
        
        /// <summary>
        /// Gets or sets whether the status worsened.
        /// </summary>
        public string? StatusWorsened { get; set; }
        
        /// <summary>
        /// Gets or sets whether the condition was incurred or aggravated.
        /// </summary>
        public bool? IncurredOrAggravated { get; set; }
        
        /// <summary>
        /// Gets or sets the member's status when injured.
        /// </summary>
        public string? StatusWhenInjured { get; set; }
        
        /// <summary>
        /// Gets or sets an explanation of the member's status when injured.
        /// </summary>
        public string? StatusWhenInjuredExplanation { get; set; }
        
        /// <summary>
        /// Gets or sets whether orders are attached.
        /// </summary>
        public string? OrdersAttached { get; set; }
        
        /// <summary>
        /// Gets or sets the IDT (Inactive Duty Training) status.
        /// </summary>
        public string? IdtStatus { get; set; }
        
        /// <summary>
        /// Gets or sets whether UTAPS (Unit Training Assembly Participation System) is attached.
        /// </summary>
        public string? UtapsAttached { get; set; }
        
        /// <summary>
        /// Gets or sets whether PCARS is attached.
        /// </summary>
        public bool? PcarsAttached { get; set; }
        
        /// <summary>
        /// Gets or sets whether PCARS history exists.
        /// </summary>
        public bool? PcarsHistory { get; set; }
        
        /// <summary>
        /// Gets or sets whether the eight-year rule applies.
        /// </summary>
        public bool? EightYearRule { get; set; }
        
        // ========== NAVIGATION PROPERTIES ==========
        
        /// <summary>
        /// Gets or sets the associated Line of Duty case.
        /// </summary>
        public LineOfDutyCase? LineOfDutyCase { get; set; }
    }
}
