using System.ComponentModel.DataAnnotations;

namespace StateMachineExperiments.Models;

public class NewInformalCaseFormModel
{
    [Required(ErrorMessage = "Case number is required")]
    [StringLength(50, ErrorMessage = "Case number cannot exceed 50 characters")]
    public string CaseNumber { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the member (foreign key). Either select existing or create new.
    /// </summary>
    public int? SelectedMemberId { get; set; }

    // Properties for creating a new member if SelectedMemberId is null
    [StringLength(20, ErrorMessage = "Card ID cannot exceed 20 characters")]
    public string? NewMemberCardId { get; set; }

    [StringLength(100, ErrorMessage = "Member name cannot exceed 100 characters")]
    public string? NewMemberName { get; set; }

    [StringLength(50, ErrorMessage = "Rank cannot exceed 50 characters")]
    public string? NewMemberRank { get; set; }

    [StringLength(200, ErrorMessage = "Unit cannot exceed 200 characters")]
    public string? NewMemberUnit { get; set; }
}

public class NewFormalCaseFormModel
{
    [Required(ErrorMessage = "Case number is required")]
    [StringLength(50, ErrorMessage = "Case number cannot exceed 50 characters")]
    public string CaseNumber { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the member (foreign key). Either select existing or create new.
    /// </summary>
    public int? SelectedMemberId { get; set; }

    // Properties for creating a new member if SelectedMemberId is null
    [StringLength(20, ErrorMessage = "Card ID cannot exceed 20 characters")]
    public string? NewMemberCardId { get; set; }

    [StringLength(100, ErrorMessage = "Member name cannot exceed 100 characters")]
    public string? NewMemberName { get; set; }

    [StringLength(50, ErrorMessage = "Rank cannot exceed 50 characters")]
    public string? NewMemberRank { get; set; }

    [StringLength(200, ErrorMessage = "Unit cannot exceed 200 characters")]
    public string? NewMemberUnit { get; set; }

    public bool IsDeathCase { get; set; } = false;
}
