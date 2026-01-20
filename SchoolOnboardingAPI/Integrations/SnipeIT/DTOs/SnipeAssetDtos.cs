namespace SchoolOnboardingAPI.Integrations.SnipeIT.DTOs;

/// <summary>
/// Snipe-IT Asset response from API
/// </summary>
public class SnipeAssetDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string AssetTag { get; set; } = null!;
    public string? Status { get; set; }
    public string? Model { get; set; }
    public string? Category { get; set; }
    public int? ModelId { get; set; }
    public int? StatusId { get; set; }
    public int? CategoryId { get; set; }
    public object? AssignedTo { get; set; }
    public DateTime? CheckoutDate { get; set; }
    public DateTime? ExpectedCheckinDate { get; set; }
    public DateTime? LastAuditDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Snipe-IT API response wrapper
/// </summary>
public class SnipeApiResponse<T>
{
    public T? Data { get; set; }
    public int Total { get; set; }
    public int? Offset { get; set; }
    public int? Limit { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// Snipe-IT paginated assets response
/// </summary>
public class SnipeAssetsResponse
{
    public List<SnipeAssetDto> Data { get; set; } = new();
    public int Total { get; set; }
    public int? Offset { get; set; }
    public int? Limit { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// Snipe-IT checkout response when assigning device
/// </summary>
public class SnipeCheckoutResponse
{
    public int? Status { get; set; }
    public int? Payload { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// Snipe-IT checkin response when returning device
/// </summary>
public class SnipeCheckinResponse
{
    public int? Status { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}
