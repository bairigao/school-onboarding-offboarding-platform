namespace SchoolOnboardingAPI.Models;

/// <summary>
/// Represents the assignment of an asset (device) to a person.
/// Tracks device lifecycle through assignment, usage, and return.
/// Syncs with Snipe-IT asset management system.
/// </summary>
public class AssetAssignment
{
    public int Id { get; set; }
    
    public int PersonId { get; set; }
    
    /// <summary>
    /// Snipe-IT asset ID for external system integration
    /// </summary>
    public int? SnipeItAssetId { get; set; }
    
    /// <summary>
    /// Asset tag/barcode (e.g., "LAP-001", "TAB-045")
    /// </summary>
    public string AssetTag { get; set; } = null!;
    
    /// <summary>
    /// Timestamp when device was assigned to person
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Timestamp when device was returned (null if still assigned)
    /// </summary>
    public DateTime? ReturnedAt { get; set; }
    
    /// <summary>
    /// Notes on device condition, damage, or special handling
    /// </summary>
    public string? ConditionNotes { get; set; }
    
    // Navigation property
    public Person Person { get; set; } = null!;
}
