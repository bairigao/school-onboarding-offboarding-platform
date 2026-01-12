namespace SchoolOnboardingAPI.Models;

/// <summary>
/// Represents a link between a lifecycle request and a support ticket system.
/// Enables tracking of onboarding/offboarding through ticketing system (OSTicket).
/// </summary>
public class TicketLink
{
    public int Id { get; set; }
    
    public int LifecycleRequestId { get; set; }
    
    /// <summary>
    /// Ticket ID in OSTicket system
    /// </summary>
    public int OsTicketTicketId { get; set; }
    
    /// <summary>
    /// Type of ticket: onboarding | offboarding | issue
    /// </summary>
    public string TicketType { get; set; } = null!;
    
    /// <summary>
    /// Timestamp when the ticket link was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public LifecycleRequest LifecycleRequest { get; set; } = null!;
}
