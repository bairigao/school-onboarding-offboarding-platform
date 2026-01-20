using Microsoft.AspNetCore.Mvc;
using SchoolOnboardingAPI.Data;
using SchoolOnboardingAPI.DTOs;
using SchoolOnboardingAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace SchoolOnboardingAPI.Controllers;

/// <summary>
/// Controller for managing workflow tasks in lifecycle requests
/// IT staff can complete tasks (especially device returns/checkins)
/// </summary>
[ApiController]
[Route("api/lifecycle-tasks")]
public class LifecycleTasksController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IAssetService _assetService;
    private readonly ILogger<LifecycleTasksController> _logger;

    public LifecycleTasksController(
        ApplicationDbContext dbContext,
        IAssetService assetService,
        ILogger<LifecycleTasksController> logger)
    {
        _dbContext = dbContext;
        _assetService = assetService;
        _logger = logger;
    }

    /// <summary>
    /// Get all tasks for a lifecycle request
    /// </summary>
    /// <param name="requestId">Lifecycle request ID</param>
    [HttpGet("request/{requestId}")]
    [ProducesResponseType(typeof(List<LifecycleTaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<LifecycleTaskDto>>> GetTasksByRequest(int requestId)
    {
        try
        {
            _logger.LogInformation("Getting tasks for request {RequestId}", requestId);

            // Verify request exists
            var request = await _dbContext.LifecycleRequests.FindAsync(requestId);
            if (request == null)
            {
                _logger.LogWarning("Request {RequestId} not found", requestId);
                return NotFound("Request not found");
            }

            var tasks = await _dbContext.LifecycleTasks
                .Where(t => t.LifecycleRequestId == requestId)
                .Select(t => new LifecycleTaskDto
                {
                    Id = t.Id,
                    LifecycleRequestId = t.LifecycleRequestId,
                    TaskType = t.TaskType,
                    Required = t.Required,
                    Completed = t.Completed,
                    CompletedAt = t.CompletedAt,
                    Notes = t.Notes
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} tasks for request {RequestId}", tasks.Count, requestId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for request {RequestId}", requestId);
            return StatusCode(500, "An error occurred while retrieving tasks");
        }
    }

    /// <summary>
    /// Get a specific task
    /// </summary>
    /// <param name="id">Task ID</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LifecycleTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LifecycleTaskDto>> GetTask(int id)
    {
        try
        {
            _logger.LogInformation("Getting task {TaskId}", id);

            var task = await _dbContext.LifecycleTasks.FindAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Task {TaskId} not found", id);
                return NotFound("Task not found");
            }

            var dto = new LifecycleTaskDto
            {
                Id = task.Id,
                LifecycleRequestId = task.LifecycleRequestId,
                TaskType = task.TaskType,
                Required = task.Required,
                Completed = task.Completed,
                CompletedAt = task.CompletedAt,
                Notes = task.Notes
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task {TaskId}", id);
            return StatusCode(500, "An error occurred while retrieving the task");
        }
    }

    /// <summary>
    /// Mark a task as completed
    /// For return_device tasks, IT will specify the asset tag to checkin from Snipe-IT
    /// </summary>
    /// <param name="id">Task ID to complete</param>
    /// <param name="updateDto">Task completion data</param>
    /// <param name="role">User role (requires IT staff)</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(LifecycleTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LifecycleTaskDto>> UpdateTask(
        int id,
        [FromBody] UpdateLifecycleTaskDto updateDto,
        [FromHeader(Name = "X-User-Role")] string? role = null)
    {
        try
        {
            // Authorization: Only IT staff can complete tasks
            if (role != "it")
            {
                _logger.LogWarning("User with role {Role} not authorized to complete tasks", role);
                return Forbid();
            }

            var task = await _dbContext.LifecycleTasks
                .Include(t => t.LifecycleRequest)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                _logger.LogWarning("Task {TaskId} not found", id);
                return NotFound("Task not found");
            }

            // Mark task as completed
            if (updateDto.Completed.HasValue && updateDto.Completed.Value)
            {
                task.Completed = true;
                task.CompletedAt = DateTime.UtcNow;

                // If this is a return_device task, the Notes should contain the asset tag to checkin
                if (task.TaskType == "return_device" && !string.IsNullOrEmpty(updateDto.Notes))
                {
                    try
                    {
                        // Attempt to checkin the device from Snipe-IT
                        var assetTag = updateDto.Notes.Split('|')[0].Trim(); // Format: "asset-tag|notes"
                        var notes = updateDto.Notes.Contains('|') 
                            ? string.Join("|", updateDto.Notes.Split('|').Skip(1)).Trim() 
                            : null;

                        // Find asset by tag (you might need to implement this in your AssetService)
                        // For now, we'll just store the notes
                        task.Notes = updateDto.Notes;

                        _logger.LogInformation("Device checkin noted for task {TaskId}: Asset {AssetTag}", id, assetTag);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not process device checkin details for task {TaskId}", id);
                        task.Notes = updateDto.Notes;
                    }
                }
                else if (!string.IsNullOrEmpty(updateDto.Notes))
                {
                    task.Notes = updateDto.Notes;
                }

                _dbContext.LifecycleTasks.Update(task);
                await _dbContext.SaveChangesAsync();

                // Check if all required tasks are completed to update request status
                var request = task.LifecycleRequest;
                if (request != null)
                {
                    var allTasksCompleted = await _dbContext.LifecycleTasks
                        .Where(t => t.LifecycleRequestId == request.Id && t.Required)
                        .AllAsync(t => t.Completed);

                    if (allTasksCompleted && request.Status == "in_progress")
                    {
                        request.Status = "completed";
                        request.UpdatedAt = DateTime.UtcNow;
                        _dbContext.LifecycleRequests.Update(request);
                        await _dbContext.SaveChangesAsync();

                        _logger.LogInformation("Lifecycle request {RequestId} marked as completed", request.Id);
                    }
                }

                _logger.LogInformation("Completed task {TaskId}", id);
            }
            else
            {
                return BadRequest("Completed must be set to true");
            }

            var resultDto = new LifecycleTaskDto
            {
                Id = task.Id,
                LifecycleRequestId = task.LifecycleRequestId,
                TaskType = task.TaskType,
                Required = task.Required,
                Completed = task.Completed,
                CompletedAt = task.CompletedAt,
                Notes = task.Notes
            };

            return Ok(resultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task {TaskId}", id);
            return StatusCode(500, "An error occurred while updating the task");
        }
    }
}
