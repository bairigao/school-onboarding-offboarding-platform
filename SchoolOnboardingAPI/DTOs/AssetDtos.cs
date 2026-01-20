namespace SchoolOnboardingAPI.DTOs;

/// <summary>
/// DTO for asset information returned to API consumers
/// </summary>
public class AssetDto
{
    /// <summary>
    /// Snipe-IT asset ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Asset name/description
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Asset tag/serial number
    /// </summary>
    public string? AssetTag { get; set; }

    /// <summary>
    /// Model of the asset (e.g., "MacBook Pro 16", "Dell Latitude 7420")
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Manufacturer (e.g., "Apple", "Dell")
    /// </summary>
    public string? Manufacturer { get; set; }

    /// <summary>
    /// Current status (ready_to_deploy, deployed, returned, etc.)
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Current location/assigned user if deployed
    /// </summary>
    public string? AssignedTo { get; set; }

    /// <summary>
    /// Condition of asset
    /// </summary>
    public string? Condition { get; set; }

    /// <summary>
    /// Purchase date
    /// </summary>
    public DateTime? PurchaseDate { get; set; }

    /// <summary>
    /// Is asset available for assignment?
    /// </summary>
    public bool Available { get; set; }
}

/// <summary>
/// DTO for paginated asset list response
/// </summary>
public class AssetListDto
{
    /// <summary>
    /// Total number of assets
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Assets on this page
    /// </summary>
    public required List<AssetDto> Assets { get; set; } = [];
}
