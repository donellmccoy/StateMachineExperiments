namespace StateMachineExperiments.Models;

/// <summary>
/// Represents a request to reinvestigate a Line of Duty case,
/// including approval workflow signatures and status tracking.
/// </summary>
public class ReinvestigationRequest
{
    public int Id { get; set; }
    
    public int? LineOfDutyCaseId { get; set; }
    
    public int InitialLodId { get; set; }
    
    public required string CaseId { get; set; }
    
    public int CreatedBy { get; set; }
    
    public DateOnly CreatedDate { get; set; }
    
    public int? Workflow { get; set; }
    
    public int? Status { get; set; }
    
    public byte? RwoaReason { get; set; }
    
    public DateTime? RwoaDate { get; set; }
    
    public string? RwoaExplanation { get; set; }
    
    // MPF Signature
    public DateTime? SigDateMpf { get; set; }
    
    public string? SigNameMpf { get; set; }
    
    public string? SigTitleMpf { get; set; }
    
    // Wing JA Signature
    public DateTime? SigDateWingJa { get; set; }
    
    public string? SigNameWingJa { get; set; }
    
    public string? SigTitleWingJa { get; set; }
    
    // Wing CC Signature
    public DateTime? SigDateWingCc { get; set; }
    
    public string? SigNameWingCc { get; set; }
    
    public string? SigTitleWingCc { get; set; }
    
    // Board Admin Signature
    public DateTime? SigDateBoardAdmin { get; set; }
    
    public string? SigNameBoardAdmin { get; set; }
    
    public string? SigTitleBoardAdmin { get; set; }
    
    // Board Medical Signature
    public DateTime? SigDateBoardMedical { get; set; }
    
    public string? SigNameBoardMedical { get; set; }
    
    public string? SigTitleBoardMedical { get; set; }
    
    // Board Legal Signature
    public DateTime? SigDateBoardLegal { get; set; }
    
    public string? SigNameBoardLegal { get; set; }
    
    public string? SigTitleBoardLegal { get; set; }
    
    // Approval Signature
    public DateTime? SigDateApproval { get; set; }
    
    public string? SigNameApproval { get; set; }
    
    public string? SigTitleApproval { get; set; }
    
    // Board Tech Final Signature
    public DateTime? SigDateBoardTechFinal { get; set; }
    
    public string? SigNameBoardTechFinal { get; set; }
    
    public string? SigTitleBoardTechFinal { get; set; }
    
    // LOD PM Signature
    public DateTime? SigDateLodPm { get; set; }
    
    public string? SigNameLodPm { get; set; }
    
    public string? SigTitleLodPm { get; set; }
    
    // Board A1 Signature
    public DateTime? SigDateBoardA1 { get; set; }
    
    public string? SigNameBoardA1 { get; set; }
    
    public string? SigTitleBoardA1 { get; set; }
    
    // Approval Flags
    public byte? WingCcApproved { get; set; }
    
    public byte? BoardMedicalApproved { get; set; }
    
    public byte? BoardLegalApproved { get; set; }
    
    public byte? AaFinalApproved { get; set; }
    
    public byte? WingJaApproved { get; set; }
    
    public byte? BoardTechApproval1 { get; set; }
    
    public byte? BoardTechApproval2 { get; set; }
    
    public byte? BoardA1Approved { get; set; }
    
    // Approval Comments
    public string? ReturnComment { get; set; }
    
    public string? WingJaApprovalComment { get; set; }
    
    public string? WingCcApprovalComment { get; set; }
    
    public string? BoardTechApproval1Comment { get; set; }
    
    public string? BoardMedicalApprovalComment { get; set; }
    
    public string? BoardLegalApprovalComment { get; set; }
    
    public string? AaFinalApprovalComment { get; set; }
    
    public string? BoardA1ApprovalComment { get; set; }
    
    // Member Information
    public string? MemberSsn { get; set; }
    
    public string? MemberName { get; set; }
    
    public required string MemberUnit { get; set; }
    
    public int MemberUnitId { get; set; }
    
    public required string MemberCompo { get; set; }
    
    public int MemberGrade { get; set; }
    
    // Audit and Tracking
    public int? ModifiedBy { get; set; }
    
    public DateTime? ModifiedDate { get; set; }
    
    public int? DocGroupId { get; set; }
    
    // Cancellation Information
    public int? CancelReason { get; set; }
    
    public string? CancelExplanation { get; set; }
    
    public DateTime? CancelDate { get; set; }
    
    // Return Routing
    public int? ReturnToGroup { get; set; }
    
    public int? ReturnByGroup { get; set; }
    
    // Flags
    public bool IsNonDbSignCase { get; set; }
    
    // Navigation Properties
    // =====================
    
    public LineOfDutyCase? InitialLineOfDutyCase { get; set; }
    
    public LineOfDutyCase? ReinvestigationLineOfDutyCase { get; set; }
}
