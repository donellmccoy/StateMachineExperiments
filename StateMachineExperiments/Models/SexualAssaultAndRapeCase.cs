namespace StateMachineExperiments.Models;

/// <summary>
/// Represents a Sexual Assault Response Coordinator (SARC) case,
/// including member information, incident details, and approval workflow signatures.
/// </summary>
public class SexualAssaultAndRapeCase
{
    public int Id { get; set; }
    
    public int LineOfDutyCaseId { get; set; }
    
    public required string CaseId { get; set; }
    
    public int Status { get; set; }
    
    public byte? Workflow { get; set; }
    
    // Member Information
    public required string MemberName { get; set; }
    
    public required string MemberSsn { get; set; }
    
    public int MemberGrade { get; set; }
    
    public required string MemberUnit { get; set; }
    
    public int MemberUnitId { get; set; }
    
    public DateTime? MemberDob { get; set; }
    
    public required string MemberCompo { get; set; }
    
    // Case Details
    public int CreatedBy { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public int? DocGroupId { get; set; }
    
    public DateTime? IncidentDate { get; set; }
    
    public int? DutyStatus { get; set; }
    
    public DateTime? DurationStartDate { get; set; }
    
    public DateTime? DurationEndDate { get; set; }
    
    // ICD Codes
    public bool? IcdE9688 { get; set; }
    
    public bool? IcdE9699 { get; set; }
    
    public bool? IcdOther { get; set; }
    
    public bool? InDutyStatus { get; set; }
    
    // RSL Wing SARC Signature
    public DateTime? SigDateRslWingSarc { get; set; }
    
    public string? SigNameRslWingSarc { get; set; }
    
    public string? SigTitleRslWingSarc { get; set; }
    
    // SARC A1 Signature
    public DateTime? SigDateSarcA1 { get; set; }
    
    public string? SigNameSarcA1 { get; set; }
    
    public string? SigTitleSarcA1 { get; set; }
    
    // Board Medical Signature
    public DateTime? SigDateBoardMedical { get; set; }
    
    public string? SigNameBoardMedical { get; set; }
    
    public string? SigTitleBoardMedical { get; set; }
    
    // Board JA Signature
    public DateTime? SigDateBoardJa { get; set; }
    
    public string? SigNameBoardJa { get; set; }
    
    public string? SigTitleBoardJa { get; set; }
    
    // Board Admin Signature
    public DateTime? SigDateBoardAdmin { get; set; }
    
    public string? SigNameBoardAdmin { get; set; }
    
    public string? SigTitleBoardAdmin { get; set; }
    
    // Approving Authority Signature
    public DateTime? SigDateApproving { get; set; }
    
    public string? SigNameApproving { get; set; }
    
    public string? SigTitleApproving { get; set; }
    
    // RWOA Information
    public byte? RwoaReason { get; set; }
    
    public string? RwoaExplanation { get; set; }
    
    public DateTime? RwoaDate { get; set; }
    
    // Audit Fields
    public int? ModifiedBy { get; set; }
    
    public DateTime? ModifiedDate { get; set; }
    
    // Cancellation Information
    public int? CancelReason { get; set; }
    
    public string? CancelExplanation { get; set; }
    
    public DateTime? CancelDate { get; set; }
    
    // Additional Information
    public string? DefSexAssaultDbCaseNum { get; set; }
    
    public int? ReturnToGroup { get; set; }
    
    public int? ReturnByGroup { get; set; }
    
    public byte? ConsultationFromUsergroupId { get; set; }
    
    public string? ReturnComment { get; set; }
    
    public bool IsPostProcessingComplete { get; set; }
    
    // Navigation Properties
    // =====================
    
    public LineOfDutyCase? LineOfDutyCase { get; set; }
}
