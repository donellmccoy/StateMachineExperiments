using System;

namespace StateMachineExperiments.Models
{
    /// <summary>
    /// Represents medical information associated with a Line of Duty case.
    /// </summary>
    public class Medical
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
        /// Gets or sets the member status at the time of the event.
        /// </summary>
        public string? MemberStatus { get; set; }
        
        /// <summary>
        /// Gets or sets the type of event nature.
        /// </summary>
        public string? EventNatureType { get; set; }
        
        /// <summary>
        /// Gets or sets the detailed description of the event nature.
        /// </summary>
        public string? EventNatureDetails { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the medical facility where treatment occurred.
        /// </summary>
        public string? MedicalFacility { get; set; }
        
        /// <summary>
        /// Gets or sets the type of medical facility.
        /// </summary>
        public string? MedicalFacilityType { get; set; }
        
        /// <summary>
        /// Gets or sets the date when treatment was provided.
        /// </summary>
        public DateTime? TreatmentDate { get; set; }
        
        /// <summary>
        /// Gets or sets whether death was involved in the case.
        /// </summary>
        public string? DeathInvolvedYn { get; set; }
        
        /// <summary>
        /// Gets or sets whether a motor vehicle accident was involved.
        /// </summary>
        public string? MvaInvolvedYn { get; set; }
        
        /// <summary>
        /// Gets or sets the ICD-9 diagnosis code identifier.
        /// </summary>
        public int? Icd9Id { get; set; }
        
        /// <summary>
        /// Gets or sets whether the condition existed prior to service (EPTS).
        /// </summary>
        public byte? EptsYn { get; set; }
        
        /// <summary>
        /// Gets or sets the physician's approval comments.
        /// </summary>
        public string? PhysicianApprovalComments { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the user who last modified this record.
        /// </summary>
        public int ModifiedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the date when this record was last modified.
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets the reason code for physician cancellation.
        /// </summary>
        public byte? PhysicianCancelReason { get; set; }
        
        /// <summary>
        /// Gets or sets the explanation for physician cancellation.
        /// </summary>
        public string? PhysicianCancelExplanation { get; set; }
        
        /// <summary>
        /// Gets or sets the diagnosis text description.
        /// </summary>
        public string? DiagnosisText { get; set; }
        
        /// <summary>
        /// Gets or sets the ICD 7th character code.
        /// </summary>
        public string? Icd7thChar { get; set; }
        
        /// <summary>
        /// Gets or sets the member's origin identifier.
        /// </summary>
        public int? MemberFrom { get; set; }
        
        /// <summary>
        /// Gets or sets the member's component identifier.
        /// </summary>
        public int? MemberComponent { get; set; }
        
        /// <summary>
        /// Gets or sets the member's category identifier.
        /// </summary>
        public int? MemberCategory { get; set; }
        
        /// <summary>
        /// Gets or sets the influence identifier.
        /// </summary>
        public int? Influence { get; set; }
        
        /// <summary>
        /// Gets or sets whether the member was responsible.
        /// </summary>
        public string? MemberResponsible { get; set; }
        
        /// <summary>
        /// Gets or sets the psychological evaluation status.
        /// </summary>
        public string? PsychEval { get; set; }
        
        /// <summary>
        /// Gets or sets the date of psychological evaluation.
        /// </summary>
        public DateTime? PsychDate { get; set; }
        
        /// <summary>
        /// Gets or sets relevant medical conditions.
        /// </summary>
        public string? RelevantCondition { get; set; }
        
        /// <summary>
        /// Gets or sets other test information.
        /// </summary>
        public string? OtherTest { get; set; }
        
        /// <summary>
        /// Gets or sets the date of other tests.
        /// </summary>
        public DateTime? OtherTestDate { get; set; }
        
        /// <summary>
        /// Gets or sets the deployed location.
        /// </summary>
        public string? DeployedLocation { get; set; }
        
        /// <summary>
        /// Gets or sets whether the condition existed prior to service.
        /// </summary>
        public bool? ConditionEpts { get; set; }
        
        /// <summary>
        /// Gets or sets whether the condition was aggravated by service.
        /// </summary>
        public bool? ServiceAggravated { get; set; }
        
        /// <summary>
        /// Gets or sets the mobility standards status.
        /// </summary>
        public string? MobilityStandards { get; set; }
        
        /// <summary>
        /// Gets or sets the member's condition status.
        /// </summary>
        public string? MemberCondition { get; set; }
        
        /// <summary>
        /// Gets or sets whether an alcohol test was done.
        /// </summary>
        public string? AlcoholTestDone { get; set; }
        
        /// <summary>
        /// Gets or sets whether a drug test was done.
        /// </summary>
        public string? DrugTestDone { get; set; }
        
        /// <summary>
        /// Gets or sets the board finalization status.
        /// </summary>
        public string? BoardFinalization { get; set; }
        
        /// <summary>
        /// Gets or sets the workflow identifier.
        /// </summary>
        public int? Workflow { get; set; }
        
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
        
        // ========== NAVIGATION PROPERTIES ==========
        
        /// <summary>
        /// Gets or sets the associated Line of Duty case.
        /// </summary>
        public LineOfDutyCase? LineOfDutyCase { get; set; }
    }
}
