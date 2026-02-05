using System;

namespace StateMachineExperiments.Models
{
    /// <summary>
    /// Represents findings associated with a Line of Duty case.
    /// </summary>
    public class Finding
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
        /// Gets or sets the personnel type.
        /// </summary>
        public short PersonnelType { get; set; }
        
        /// <summary>
        /// Gets or sets the Social Security Number.
        /// </summary>
        public string? Ssn { get; set; }
        
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// Gets or sets the grade.
        /// </summary>
        public string? Grade { get; set; }
        
        /// <summary>
        /// Gets or sets the component.
        /// </summary>
        public string? Component { get; set; }
        
        /// <summary>
        /// Gets or sets the rank.
        /// </summary>
        public string? Rank { get; set; }
        
        /// <summary>
        /// Gets or sets the PAS code.
        /// </summary>
        public string? PasCode { get; set; }
        
        /// <summary>
        /// Gets or sets the finding code.
        /// </summary>
        public byte? FindingCode { get; set; }
        
        /// <summary>
        /// Gets or sets the decision indicator.
        /// </summary>
        public string? DecisionYn { get; set; }
        
        /// <summary>
        /// Gets or sets the explanation.
        /// </summary>
        public string? Explanation { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the user who last modified this record.
        /// </summary>
        public int ModifiedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the date when this record was last modified.
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets the ID of the user who created this record.
        /// </summary>
        public int CreatedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the date when this record was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets the findings text.
        /// </summary>
        public string? FindingsText { get; set; }
        
        /// <summary>
        /// Gets or sets whether to refer to DES.
        /// </summary>
        public bool? ReferDes { get; set; }
        
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
        /// Gets or sets whether the member was correctly identified.
        /// </summary>
        public string? CorrectlyIdentified { get; set; }
        
        /// <summary>
        /// Gets or sets whether documentation was verified and attached.
        /// </summary>
        public string? VerifiedAndAttached { get; set; }
        
        /// <summary>
        /// Gets or sets the IDT (Inactive Duty Training) status.
        /// </summary>
        public string? IdtStatus { get; set; }
        
        /// <summary>
        /// Gets or sets whether IPCARS is attached.
        /// </summary>
        public bool? IpcarsAttached { get; set; }
        
        /// <summary>
        /// Gets or sets whether the eight-year rule applies.
        /// </summary>
        public bool? EightYearRule { get; set; }
        
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
