namespace SchoolOnboardingAPI.Services;

using SchoolOnboardingAPI.Data;
using SchoolOnboardingAPI.Integrations.SnipeIT;
using SchoolOnboardingAPI.Integrations.SnipeIT.DTOs;
using SchoolOnboardingAPI.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Interface for asset management service
/// </summary>
public interface IAssetService
{
    Task<List<SnipeAssetDto>> GetAvailableAssetsAsync(int page = 1, int pageSize = 50);
    Task<SnipeAssetDto?> GetAssetAsync(int assetId);
    Task SyncAssetsFromSnipeITAsync();
}

/// <summary>
/// Asset management service
/// Handles device assignment, returns, and synchronization with Snipe-IT
/// </summary>
public class AssetService : IAssetService
{
    private readonly ISnipeITClient _snipeITClient;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<AssetService> _logger;

    public AssetService(ISnipeITClient snipeITClient, ApplicationDbContext dbContext, ILogger<AssetService> logger)
    {
        _snipeITClient = snipeITClient;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Get all available assets from Snipe-IT (paginated)
    /// </summary>
    public async Task<List<SnipeAssetDto>> GetAvailableAssetsAsync(int page = 1, int pageSize = 50)
    {
        try
        {
            _logger.LogInformation("Fetching available assets from Snipe-IT - Page: {Page}, Size: {PageSize}", page, pageSize);

            // Calculate offset from page number
            int offset = (page - 1) * pageSize;
            
            var response = await _snipeITClient.GetAssetsAsync(pageSize, offset);
            
            if (!response.Success || response.Data == null)
            {
                _logger.LogWarning("Failed to fetch assets at offset {Offset}", offset);
                return [];
            }

            _logger.LogInformation("Retrieved {Count} assets from Snipe-IT", response.Data.Count);
            return response.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching available assets");
            return [];
        }
    }

    /// <summary>
    /// Get specific asset from Snipe-IT
    /// </summary>
    public async Task<SnipeAssetDto?> GetAssetAsync(int assetId)
    {
        try
        {
            _logger.LogInformation("Fetching asset {AssetId} from Snipe-IT", assetId);
            return await _snipeITClient.GetAssetByIdAsync(assetId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching asset {AssetId}", assetId);
            return null;
        }
    }

    /// <summary>
    /// Synchronize assets from Snipe-IT
    /// Useful for periodic sync to keep device inventory up to date
    /// </summary>
    public async Task SyncAssetsFromSnipeITAsync()
    {
        try
        {
            _logger.LogInformation("Starting asset synchronization from Snipe-IT");

            var assets = await GetAvailableAssetsAsync();
            _logger.LogInformation("Synced {Count} assets from Snipe-IT", assets.Count);

            // Note: This is a read-only sync for now
            // Device assignments happen automatically when users log in
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error synchronizing assets from Snipe-IT");
        }
    }
}
