namespace SchoolOnboardingAPI.Models;

/// <summary>
/// Represents a request to onboard or offboard a person.
/// Initiates the lifecycle workflow with associated tasks.
/// </summary>
public class LifecycleRequest
{
    public int Id { get; set; }
    
    public int PersonId { get; set; }
    
    /// <summary>
    /// Type of request: "onboard" or "offboard"
    /// </summary>
    public string RequestType { get; set; } = null!;
    
    /// <summary>
    /// Date when the action becomes effective
    /// </summary>
    public DateTime EffectiveDate { get; set; }
    
    /// <summary>
    /// Name or user ID of the person who submitted the request
    /// </summary>
    public string SubmittedBy { get; set; } = null!;
    
    /// <summary>
    /// Role of the person who submitted: "enrolment" or "hr"
    /// </summary>
    public string SubmittedRole { get; set; } = null!;
    
    /// <summary>
    /// Current status: pending | in_progress | completed
    /// </summary>
    public string Status { get; set; } = "pending";
    
    /// <summary>
    /// Detailed notes: comments, special instructions, reasons
    /// </summary>
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Person Person { get; set; } = null!;
    
    public ICollection<LifecycleTask> Tasks { get; set; } = new List<LifecycleTask>();
    
    public ICollection<TicketLink> TicketLinks { get; set; } = new List<TicketLink>();
}
