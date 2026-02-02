using System.ComponentModel.DataAnnotations;

namespace StateMachineExperiments.Models;

public class NewInformalCaseFormModel
{
    [Required(ErrorMessage = "Case number is required")]
    [StringLength(50, ErrorMessage = "Case number cannot exceed 50 characters")]
    public string CaseNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Member ID is required")]
    [StringLength(20, ErrorMessage = "Member ID cannot exceed 20 characters")]
    public string MemberId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Member name is required")]
    [StringLength(100, ErrorMessage = "Member name cannot exceed 100 characters")]
    public string MemberName { get; set; } = string.Empty;
}

public class NewFormalCaseFormModel
{
    [Required(ErrorMessage = "Case number is required")]
    [StringLength(50, ErrorMessage = "Case number cannot exceed 50 characters")]
    public string CaseNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Member ID is required")]
    [StringLength(20, ErrorMessage = "Member ID cannot exceed 20 characters")]
    public string MemberId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Member name is required")]
    [StringLength(100, ErrorMessage = "Member name cannot exceed 100 characters")]
    public string MemberName { get; set; } = string.Empty;

    public bool IsDeathCase { get; set; } = false;
}
