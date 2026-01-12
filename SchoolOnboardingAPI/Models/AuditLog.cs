namespace SchoolOnboardingAPI.Models;

/// <summary>
/// Represents an audit log entry tracking all changes in the system.
/// Provides complete audit trail for compliance, debugging, and accountability.
/// </summary>
public class AuditLog
{
    public int Id { get; set; }
    
    /// <summary>
    /// Type of entity that was changed: person | lifecycle_request | asset_assignment
    /// </summary>
    public string EntityType { get; set; } = null!;
    
    /// <summary>
    /// ID of the entity that was changed
    /// </summary>
    public int EntityId { get; set; }
    
    /// <summary>
    /// Action performed: created | updated | status_changed
    /// </summary>
    public string Action { get; set; } = null!;
    
    /// <summary>
    /// Previous value (before change). Null for create actions.
    /// </summary>
    public string? OldValue { get; set; }
    
    /// <summary>
    /// New value (after change). For create actions, shows initial value.
    /// </summary>
    public string? NewValue { get; set; }
    
    /// <summary>
    /// User or system that made the change
    /// </summary>
    public string ChangedBy { get; set; } = "system";
    
    /// <summary>
    /// Timestamp of when the change occurred
    /// </summary>
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
