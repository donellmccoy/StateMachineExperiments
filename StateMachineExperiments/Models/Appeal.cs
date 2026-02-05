using System;

namespace StateMachineExperiments.Models
{
    /// <summary>
    /// Represents an appeal for a Line of Duty case decision.
    /// </summary>
    public class Appeal
    {
        /// <summary>
        /// Gets or sets the unique database identifier.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the initial Line of Duty case ID (foreign key).
        /// </summary>
        public int LineOfDutyCaseId { get; set; }
        
        /// <summary>
        /// Gets or sets the case identifier.
        /// </summary>
        public string CaseId { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the ID of the user who created this appeal.
        /// </summary>
        public int CreatedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the date when this appeal was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets the ID of the user who last modified this appeal.
        /// </summary>
        public int? ModifiedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the date when this appeal was last modified.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }
        
        /// <summary>
        /// Gets or sets the workflow identifier.
        /// </summary>
        public int? Workflow { get; set; }
        
        /// <summary>
        /// Gets or sets the status of the appeal.
        /// </summary>
        public int? Status { get; set; }
        
        /// <summary>
        /// Gets or sets the cancellation reason code.
        /// </summary>
        public int? CancelReason { get; set; }
        
        /// <summary>
        /// Gets or sets the cancellation explanation.
        /// </summary>
        public string? CancelExplanation { get; set; }
        
        /// <summary>
        /// Gets or sets the cancellation date.
        /// </summary>
        public DateTime? CancelDate { get; set; }
        
        /// <summary>
        /// Gets or sets the RWOA (Return Without Action) reason code.
        /// </summary>
        public byte? RwoaReason { get; set; }
        
        /// <summary>
        /// Gets or sets the RWOA date.
        /// </summary>
        public DateTime? RwoaDate { get; set; }
        
        /// <summary>
        /// Gets or sets the RWOA explanation.
        /// </summary>
        public string? RwoaExplanation { get; set; }
        
        /// <summary>
        /// Gets or sets the group to which the appeal was returned.
        /// </summary>
        public int? ReturnToGroup { get; set; }
        
        /// <summary>
        /// Gets or sets the group that returned the appeal.
        /// </summary>
        public int? ReturnByGroup { get; set; }
        
        /// <summary>
        /// Gets or sets the Program Manager signature date.
        /// </summary>
        public DateTime? SignatureDatePm { get; set; }
        
        /// <summary>
        /// Gets or sets the Program Manager signature name.
        /// </summary>
        public string? SignatureNamePm { get; set; }
        
        /// <summary>
        /// Gets or sets the Program Manager signature title.
        /// </summary>
        public string? SignatureTitlePm { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Tech signature date.
        /// </summary>
        public DateTime? SignatureDateBoardTech { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Tech signature name.
        /// </summary>
        public string? SignatureNameBoardTech { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Tech signature title.
        /// </summary>
        public string? SignatureTitleBoardTech { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Medical signature date.
        /// </summary>
        public DateTime? SignatureDateBoardMedical { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Medical signature name.
        /// </summary>
        public string? SignatureNameBoardMedical { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Medical signature title.
        /// </summary>
        public string? SignatureTitleBoardMedical { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Legal signature date.
        /// </summary>
        public DateTime? SignatureDateBoardLegal { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Legal signature name.
        /// </summary>
        public string? SignatureNameBoardLegal { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Legal signature title.
        /// </summary>
        public string? SignatureTitleBoardLegal { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Admin signature date.
        /// </summary>
        public DateTime? SignatureDateBoardAdmin { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Admin signature name.
        /// </summary>
        public string? SignatureNameBoardAdmin { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Admin signature title.
        /// </summary>
        public string? SignatureTitleBoardAdmin { get; set; }
        
        /// <summary>
        /// Gets or sets the Approving Authority signature date.
        /// </summary>
        public DateTime? SignatureDateApprovingAuth { get; set; }
        
        /// <summary>
        /// Gets or sets the Approving Authority signature name.
        /// </summary>
        public string? SignatureNameApprovingAuth { get; set; }
        
        /// <summary>
        /// Gets or sets the Approving Authority signature title.
        /// </summary>
        public string? SignatureTitleApprovingAuth { get; set; }
        
        /// <summary>
        /// Gets or sets the Appellate Authority signature date.
        /// </summary>
        public DateTime? SignatureDateAppellateAuth { get; set; }
        
        /// <summary>
        /// Gets or sets the Appellate Authority signature name.
        /// </summary>
        public string? SignatureNameAppellateAuth { get; set; }
        
        /// <summary>
        /// Gets or sets the Appellate Authority signature title.
        /// </summary>
        public string? SignatureTitleAppellateAuth { get; set; }
        
        /// <summary>
        /// Gets or sets whether the LOD Program Manager approved.
        /// </summary>
        public byte? LodPmApproved { get; set; }
        
        /// <summary>
        /// Gets or sets whether the Board Tech approved.
        /// </summary>
        public byte? BoardTechApproved { get; set; }
        
        /// <summary>
        /// Gets or sets whether the Board Medical approved.
        /// </summary>
        public byte? BoardMedicalApproved { get; set; }
        
        /// <summary>
        /// Gets or sets whether the Board Legal approved.
        /// </summary>
        public byte? BoardLegalApproved { get; set; }
        
        /// <summary>
        /// Gets or sets whether the Board Admin approved.
        /// </summary>
        public byte? BoardAdminApproved { get; set; }
        
        /// <summary>
        /// Gets or sets whether the Approval Authority approved.
        /// </summary>
        public byte? ApprovalAuthApproved { get; set; }
        
        /// <summary>
        /// Gets or sets whether the Appellate Authority approved.
        /// </summary>
        public byte? AppellateAuthApproved { get; set; }
        
        /// <summary>
        /// Gets or sets the LOD Program Manager approval comment.
        /// </summary>
        public string? LodPmApprovalComment { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Tech approval comment.
        /// </summary>
        public string? BoardTechApprovalComment { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Medical approval comment.
        /// </summary>
        public string? BoardMedicalApprovalComment { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Legal approval comment.
        /// </summary>
        public string? BoardLegalApprovalComment { get; set; }
        
        /// <summary>
        /// Gets or sets the Board Admin approval comment.
        /// </summary>
        public string? BoardAdminApprovalComment { get; set; }
        
        /// <summary>
        /// Gets or sets the Approval Authority approval comment.
        /// </summary>
        public string? ApprovalAuthApprovalComment { get; set; }
        
        /// <summary>
        /// Gets or sets the Appellate Authority approval comment.
        /// </summary>
        public string? AppellateAuthApprovalComment { get; set; }
        
        /// <summary>
        /// Gets or sets the return comment.
        /// </summary>
        public string? ReturnComment { get; set; }
        
        /// <summary>
        /// Gets or sets the member's Social Security Number.
        /// </summary>
        public string? MemberSsn { get; set; }
        
        /// <summary>
        /// Gets or sets the member's name.
        /// </summary>
        public string? MemberName { get; set; }
        
        /// <summary>
        /// Gets or sets the document group identifier.
        /// </summary>
        public int? DocGroupId { get; set; }
        
        /// <summary>
        /// Gets or sets whether the member was notified.
        /// </summary>
        public bool? MemberNotified { get; set; }
        
        /// <summary>
        /// Gets or sets whether post-processing is complete.
        /// </summary>
        public bool IsPostProcessingComplete { get; set; }
                
        /// <summary>
        /// Gets or sets the member's component.
        /// </summary>
        public string MemberCompo { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the member's grade.
        /// </summary>
        public int MemberGrade { get; set; }
        
        /// <summary>
        /// Gets or sets whether this is a non-DB sign case.
        /// </summary>
        public bool IsNonDbSignCase { get; set; }
        
        // ========== NAVIGATION PROPERTIES ==========
        
        /// <summary>
        /// Gets or sets the initial Line of Duty case.
        /// </summary>
        public LineOfDutyCase? LineOfDutyCase { get; set; }
    }
}
