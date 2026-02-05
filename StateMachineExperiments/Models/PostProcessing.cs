namespace StateMachineExperiments.Models;

/// <summary>
/// Represents post-processing information for a Line of Duty case,
/// including appeal contact information and notification details.
/// </summary>
public class PostProcessing
{
    public int Id { get; set; }
    
    public int LineOfDutyCaseId { get; set; }
    
    public string? HelpExtensionNumber { get; set; }
    
    public string? AppealStreet { get; set; }
    
    public string? AppealCity { get; set; }
    
    public string? AppealState { get; set; }
    
    public string? AppealZip { get; set; }
    
    public string? AppealCountry { get; set; }
    
    public string? NokFirstName { get; set; }
    
    public string? NokLastName { get; set; }
    
    public string? NokMiddleName { get; set; }
    
    public DateTime? NotificationDate { get; set; }
    
    public string? Email { get; set; }
    
    public int? AddressFlag { get; set; }
    
    public int? EmailFlag { get; set; }
    
    public int? PhoneFlag { get; set; }
    
    // Navigation Properties
    // =====================
    
    public LineOfDutyCase? LineOfDutyCase { get; set; }
}
