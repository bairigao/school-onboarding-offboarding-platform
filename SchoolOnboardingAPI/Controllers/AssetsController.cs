using Microsoft.AspNetCore.Mvc;
using SchoolOnboardingAPI.Data;
using SchoolOnboardingAPI.DTOs;
using SchoolOnboardingAPI.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SchoolOnboardingAPI.Controllers;

/// <summary>
/// Controller for accessing devices from Snipe-IT asset management system
/// Devices are automatically assigned when users log in
/// This controller retrieves assigned devices during offboarding process
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assetService;
    private readonly IMapper _mapper;
    private readonly ILogger<AssetsController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public AssetsController(IAssetService assetService, IMapper mapper, ILogger<AssetsController> logger, ApplicationDbContext dbContext)
    {
        _assetService = assetService;
        _mapper = mapper;
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Get devices assigned to a specific person (for offboarding workflow)
    /// </summary>
    /// <param name="personId">Person ID to get assigned devices for</param>
    [HttpGet("person/{personId}")]
    [ProducesResponseType(typeof(List<AssetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<AssetDto>>> GetPersonAssets(int personId)
    {
        try
        {
            _logger.LogInformation("Getting assets assigned to person {PersonId}", personId);

            // Query from AssetAssignments table to find devices assigned to this person
            var assignments = await _dbContext.AssetAssignments
                .Where(aa => aa.PersonId == personId)
                .ToListAsync();

            if (!assignments.Any())
            {
                _logger.LogInformation("No assets assigned to person {PersonId}", personId);
                return Ok(new List<AssetDto>());
            }

            var assets = new List<AssetDto>();

            // Get details for each assigned device from Snipe-IT
            foreach (var assignment in assignments)
            {
                try
                {
                    var snipeAsset = await _assetService.GetAssetAsync(assignment.SnipeItAssetId ?? 0);
                    if (snipeAsset != null)
                    {
                        assets.Add(new AssetDto
                        {
                            Id = snipeAsset.Id,
                            Name = snipeAsset.Name,
                            AssetTag = snipeAsset.AssetTag,
                            Model = snipeAsset.Model,
                            Manufacturer = snipeAsset.Category,
                            Status = snipeAsset.Status,
                            AssignedTo = snipeAsset.AssignedTo?.ToString(),
                            Condition = null,
                            PurchaseDate = null,
                            Available = snipeAsset.StatusId == 1
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error fetching asset details for device {AssetId}", assignment.SnipeItAssetId);
                    // Continue with other assets even if one fails
                }
            }

            _logger.LogInformation("Retrieved {Count} assets for person {PersonId}", assets.Count, personId);
            return Ok(assets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assets for person {PersonId}", personId);
            return StatusCode(500, "An error occurred while retrieving person's assets");
        }
    }

    /// <summary>
    /// Get available devices from Snipe-IT (paginated)
    /// </summary>
    /// <param name="page">Page number (default 1)</param>
    /// <param name="pageSize">Items per page (default 50)</param>
    [HttpGet]
    [ProducesResponseType(typeof(AssetListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AssetListDto>> GetAssets(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            _logger.LogInformation("Getting available assets - Page: {Page}, Size: {PageSize}", page, pageSize);

            // Get assets from Snipe-IT
            var snipeAssets = await _assetService.GetAvailableAssetsAsync(page, pageSize);

            // Convert to response DTO
            var assets = snipeAssets.Select(sa => new AssetDto
            {
                Id = sa.Id,
                Name = sa.Name,
                AssetTag = sa.AssetTag,
                Model = sa.Model,
                Manufacturer = sa.Category,
                Status = sa.Status,
                AssignedTo = sa.AssignedTo?.ToString(),
                Condition = null, // Snipe-IT API doesn't return condition in basic asset DTO
                PurchaseDate = null,
                Available = sa.StatusId == 1 // Assuming status ID 1 is "ready_to_deploy"
            }).ToList();

            var result = new AssetListDto
            {
                Total = snipeAssets.Count,
                Page = page,
                Assets = assets
            };

            _logger.LogInformation("Retrieved {Count} assets from Snipe-IT", assets.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assets from Snipe-IT");
            return StatusCode(500, "An error occurred while retrieving assets from Snipe-IT");
        }
    }

    /// <summary>
    /// Get a specific device from Snipe-IT by asset ID
    /// </summary>
    /// <param name="id">Snipe-IT asset ID</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AssetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AssetDto>> GetAsset(int id)
    {
        try
        {
            _logger.LogInformation("Getting asset {AssetId}", id);

            var snipeAsset = await _assetService.GetAssetAsync(id);
            if (snipeAsset == null)
            {
                _logger.LogWarning("Asset {AssetId} not found in Snipe-IT", id);
                return NotFound("Asset not found");
            }

            var asset = new AssetDto
            {
                Id = snipeAsset.Id,
                Name = snipeAsset.Name,
                AssetTag = snipeAsset.AssetTag,
                Model = snipeAsset.Model,
                Manufacturer = snipeAsset.Category,
                Status = snipeAsset.Status,
                AssignedTo = snipeAsset.AssignedTo?.ToString(),
                Condition = null,
                PurchaseDate = null,
                Available = snipeAsset.StatusId == 1
            };

            return Ok(asset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset {AssetId}", id);
            return StatusCode(500, "An error occurred while retrieving the asset");
        }
    }

    /// <summary>
    /// Sync assets from Snipe-IT to update local cache
    /// This endpoint is typically called periodically to keep device inventory in sync
    /// </summary>
    [HttpPost("sync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SyncAssets()
    {
        try
        {
            _logger.LogInformation("Starting asset sync from Snipe-IT");
            await _assetService.SyncAssetsFromSnipeITAsync();

            _logger.LogInformation("Asset sync completed successfully");
            return Ok(new { message = "Assets synced successfully from Snipe-IT" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing assets from Snipe-IT");
            return StatusCode(500, "An error occurred while syncing assets from Snipe-IT");
        }
    }
}
