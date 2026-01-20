namespace SchoolOnboardingAPI.Integrations.SnipeIT;

using SchoolOnboardingAPI.Integrations.SnipeIT.DTOs;

/// <summary>
/// Interface for Snipe-IT API client
/// </summary>
public interface ISnipeITClient
{
    Task<SnipeAssetsResponse> GetAssetsAsync(int limit = 50, int offset = 0);
    Task<SnipeAssetDto?> GetAssetByIdAsync(int assetId);
    Task<SnipeAssetDto?> GetAssetByTagAsync(string assetTag);
    Task<SnipeCheckoutResponse> CheckoutAssetAsync(int assetId, int userId, string assetType = "user");
    Task<SnipeCheckinResponse> CheckinAssetAsync(int assetId);
    Task<bool> TestConnectionAsync();
}

/// <summary>
/// Snipe-IT API client implementation
/// Handles all communication with Snipe-IT asset management system
/// </summary>
public class SnipeITClient : ISnipeITClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SnipeITClient> _logger;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public SnipeITClient(HttpClient httpClient, IConfiguration configuration, ILogger<SnipeITClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var snipeItConfig = configuration.GetSection("SnipeIt");
        _baseUrl = snipeItConfig["BaseUrl"] ?? throw new InvalidOperationException("SnipeIt BaseUrl not configured");
        _apiKey = snipeItConfig["ApiKey"] ?? throw new InvalidOperationException("SnipeIt ApiKey not configured");

        // Configure HTTP client
        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    /// <summary>
    /// Get all assets from Snipe-IT
    /// </summary>
    public async Task<SnipeAssetsResponse> GetAssetsAsync(int limit = 50, int offset = 0)
    {
        try
        {
            _logger.LogInformation("Fetching assets from Snipe-IT (limit: {Limit}, offset: {Offset})", limit, offset);

            var response = await _httpClient.GetAsync($"hardware?limit={limit}&offset={offset}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Snipe-IT API error: {StatusCode} - {Content}", 
                    response.StatusCode, await response.Content.ReadAsStringAsync());
                return new SnipeAssetsResponse { Success = false };
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<SnipeAssetsResponse>(content,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _logger.LogInformation("Successfully fetched {Count} assets from Snipe-IT", result?.Data?.Count ?? 0);
            return result ?? new SnipeAssetsResponse { Success = false };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching assets from Snipe-IT");
            return new SnipeAssetsResponse { Success = false, Message = ex.Message };
        }
    }

    /// <summary>
    /// Get specific asset by ID
    /// </summary>
    public async Task<SnipeAssetDto?> GetAssetByIdAsync(int assetId)
    {
        try
        {
            _logger.LogInformation("Fetching asset {AssetId} from Snipe-IT", assetId);

            var response = await _httpClient.GetAsync($"hardware/{assetId}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Asset {AssetId} not found in Snipe-IT", assetId);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<SnipeApiResponse<SnipeAssetDto>>(content,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return apiResponse?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching asset {AssetId} from Snipe-IT", assetId);
            return null;
        }
    }

    /// <summary>
    /// Get asset by asset tag
    /// </summary>
    public async Task<SnipeAssetDto?> GetAssetByTagAsync(string assetTag)
    {
        try
        {
            _logger.LogInformation("Fetching asset with tag {AssetTag} from Snipe-IT", assetTag);

            var response = await _httpClient.GetAsync($"hardware/bytag/{assetTag}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Asset with tag {AssetTag} not found in Snipe-IT", assetTag);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<SnipeApiResponse<SnipeAssetDto>>(content,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return apiResponse?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching asset with tag {AssetTag} from Snipe-IT", assetTag);
            return null;
        }
    }

    /// <summary>
    /// Checkout asset to a user
    /// </summary>
    public async Task<SnipeCheckoutResponse> CheckoutAssetAsync(int assetId, int userId, string assetType = "user")
    {
        try
        {
            _logger.LogInformation("Checking out asset {AssetId} to user {UserId}", assetId, userId);

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(new { assigned_user = userId }),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"hardware/{assetId}/checkout", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Snipe-IT checkout error: {StatusCode} - {Content}", 
                    response.StatusCode, errorContent);
                return new SnipeCheckoutResponse { Success = false, Message = errorContent };
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<SnipeCheckoutResponse>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _logger.LogInformation("Successfully checked out asset {AssetId}", assetId);
            return result ?? new SnipeCheckoutResponse { Success = false };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking out asset {AssetId}", assetId);
            return new SnipeCheckoutResponse { Success = false, Message = ex.Message };
        }
    }

    /// <summary>
    /// Checkin asset (return it)
    /// </summary>
    public async Task<SnipeCheckinResponse> CheckinAssetAsync(int assetId)
    {
        try
        {
            _logger.LogInformation("Checking in asset {AssetId}", assetId);

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(new { }),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"hardware/{assetId}/checkin", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Snipe-IT checkin error: {StatusCode} - {Content}", 
                    response.StatusCode, errorContent);
                return new SnipeCheckinResponse { Success = false, Message = errorContent };
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<SnipeCheckinResponse>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _logger.LogInformation("Successfully checked in asset {AssetId}", assetId);
            return result ?? new SnipeCheckinResponse { Success = false };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking in asset {AssetId}", assetId);
            return new SnipeCheckinResponse { Success = false, Message = ex.Message };
        }
    }

    /// <summary>
    /// Test connection to Snipe-IT
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            _logger.LogInformation("Testing Snipe-IT connection");
            var response = await _httpClient.GetAsync("hardware?limit=1");
            var success = response.IsSuccessStatusCode;
            
            if (success)
            {
                _logger.LogInformation("Snipe-IT connection test successful");
            }
            else
            {
                _logger.LogWarning("Snipe-IT connection test failed with status {StatusCode}", response.StatusCode);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Snipe-IT connection test failed");
            return false;
        }
    }
}
