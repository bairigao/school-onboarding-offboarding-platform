using Microsoft.AspNetCore.Mvc;
using SchoolOnboardingAPI.Data;
using SchoolOnboardingAPI.DTOs;
using SchoolOnboardingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace SchoolOnboardingAPI.Controllers;

/// <summary>
/// Controller for managing onboarding and offboarding lifecycle requests
/// Enrollment officers and HR staff can submit requests
/// IT staff can view, manage, and update all requests
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LifecycleController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<LifecycleController> _logger;

    public LifecycleController(ApplicationDbContext dbContext, ILogger<LifecycleController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Get lifecycle requests
    /// IT staff see all requests, EO/HR see only their own (based on SubmittedBy header)
    /// </summary>
    /// <param name="personId">Optional filter by person ID</param>
    /// <param name="requestType">Optional filter by type: onboard or offboard</param>
    /// <param name="status">Optional filter by status: pending, in_progress, completed</param>
    /// <param name="role">User role (for authorization): it, hr, eo</param>
    /// <param name="userId">Current user ID (for authorization)</param>
    /// <param name="page">Page number (default 1)</param>
    /// <param name="pageSize">Items per page (default 20)</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<LifecycleRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LifecycleRequestDto>>> GetRequests(
        [FromQuery] int? personId = null,
        [FromQuery] string? requestType = null,
        [FromQuery] string? status = null,
        [FromHeader(Name = "X-User-Role")] string? role = null,
        [FromHeader(Name = "X-User-Id")] string? userId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            _logger.LogInformation("Getting lifecycle requests - Role: {Role}, Type: {Type}, Status: {Status}", role, requestType, status);

            var query = _dbContext.LifecycleRequests
                .Include(r => r.Person)
                .Include(r => r.Tasks)
                .Include(r => r.TicketLinks)
                .AsQueryable();

            // Authorization: IT sees all, EO/HR see only their own
            if (role != "it")
            {
                query = query.Where(r => r.SubmittedBy == userId);
            }

            // Apply filters
            if (personId.HasValue)
                query = query.Where(r => r.PersonId == personId);

            if (!string.IsNullOrEmpty(requestType))
                query = query.Where(r => r.RequestType == requestType);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            var dbRequests = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var requests = dbRequests.Select(r => MapToDto(r)).ToList();

            _logger.LogInformation("Retrieved {Count} lifecycle requests", requests.Count);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lifecycle requests");
            return StatusCode(500, "An error occurred while retrieving requests");
        }
    }

    /// <summary>
    /// Get a specific lifecycle request
    /// </summary>
    /// <param name="id">Request ID</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LifecycleRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LifecycleRequestDto>> GetRequest(int id)
    {
        try
        {
            _logger.LogInformation("Getting lifecycle request {RequestId}", id);

            var request = await _dbContext.LifecycleRequests
                .Include(r => r.Person)
                .Include(r => r.Tasks)
                .Include(r => r.TicketLinks)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                _logger.LogWarning("Request {RequestId} not found", id);
                return NotFound("Request not found");
            }

            var dto = new LifecycleRequestDto
            {
                Id = request.Id,
                PersonId = request.PersonId,
                Person = request.Person != null ? new PersonDto
                {
                    Id = request.Person.Id,
                    FirstName = request.Person.FirstName,
                    LastName = request.Person.LastName,
                    PersonType = request.Person.PersonType,
                    Status = request.Person.Status,
                    StartDate = request.Person.StartDate,
                    EndDate = request.Person.EndDate,
                    Notes = request.Person.Notes,
                    CreatedAt = request.Person.CreatedAt,
                    UpdatedAt = request.Person.UpdatedAt
                } : null,
                RequestType = request.RequestType,
                Status = request.Status,
                SubmittedRole = request.SubmittedRole,
                SubmittedBy = request.SubmittedBy,
                EffectiveDate = request.EffectiveDate,
                Notes = request.Notes,
                Tasks = request.Tasks?.Select(t => new LifecycleTaskDto
                {
                    Id = t.Id,
                    LifecycleRequestId = t.LifecycleRequestId,
                    TaskType = t.TaskType,
                    Required = t.Required,
                    Completed = t.Completed,
                    CompletedAt = t.CompletedAt,
                    Notes = t.Notes
                }).ToList(),
                TicketLinks = request.TicketLinks?.Select(tl => new TicketLinkDto
                {
                    Id = tl.Id,
                    LifecycleRequestId = tl.LifecycleRequestId,
                    OsTicketTicketId = tl.OsTicketTicketId.ToString(),
                    TicketType = tl.TicketType,
                    CreatedAt = tl.CreatedAt
                }).ToList(),
                CreatedAt = request.CreatedAt
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving request {RequestId}", id);
            return StatusCode(500, "An error occurred while retrieving the request");
        }
    }

    /// <summary>
    /// Submit a new onboarding/offboarding request
    /// Enrollment officers and HR staff can submit requests
    /// </summary>
    /// <param name="createDto">Request creation data</param>
    [HttpPost]
    [ProducesResponseType(typeof(LifecycleRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LifecycleRequestDto>> CreateRequest(
        [FromBody] CreateLifecycleRequestDto createDto,
        [FromHeader(Name = "X-User-Id")] string? userId = null)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(createDto.RequestType) ||
                string.IsNullOrWhiteSpace(createDto.SubmittedRole))
            {
                return BadRequest("RequestType and SubmittedRole are required");
            }

            // Validate request type
            if (createDto.RequestType != "onboard" && createDto.RequestType != "offboard")
            {
                return BadRequest("RequestType must be 'onboard' or 'offboard'");
            }

            // Validate role
            if (createDto.SubmittedRole != "enrolment" && createDto.SubmittedRole != "hr")
            {
                return BadRequest("SubmittedRole must be 'enrolment' or 'hr'");
            }

            // Verify person exists
            var person = await _dbContext.People.FindAsync(createDto.PersonId);
            if (person == null)
            {
                _logger.LogWarning("Person {PersonId} not found for request", createDto.PersonId);
                return NotFound("Person not found");
            }

            var request = new LifecycleRequest
            {
                PersonId = createDto.PersonId,
                RequestType = createDto.RequestType,
                Status = "pending",
                SubmittedRole = createDto.SubmittedRole,
                SubmittedBy = userId ?? "unknown",
                EffectiveDate = createDto.EffectiveDate,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Create default workflow tasks based on request type
            var defaultTasks = CreateDefaultTasks(createDto.RequestType);
            request.Tasks = defaultTasks;

            _dbContext.LifecycleRequests.Add(request);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created lifecycle request {RequestId} for person {PersonId}", request.Id, request.PersonId);

            return CreatedAtAction(nameof(GetRequest), new { id = request.Id }, MapToDto(request));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lifecycle request");
            return StatusCode(500, "An error occurred while creating the request");
        }
    }

    /// <summary>
    /// Update a lifecycle request
    /// EO/HR can only update their own requests
    /// IT staff can update any request
    /// </summary>
    /// <param name="id">Request ID to update</param>
    /// <param name="updateDto">Updated request data</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(LifecycleRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LifecycleRequestDto>> UpdateRequest(
        int id,
        [FromBody] UpdateLifecycleRequestDto updateDto,
        [FromHeader(Name = "X-User-Role")] string? role = null,
        [FromHeader(Name = "X-User-Id")] string? userId = null)
    {
        try
        {
            var request = await _dbContext.LifecycleRequests
                .Include(r => r.Person)
                .Include(r => r.Tasks)
                .Include(r => r.TicketLinks)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                _logger.LogWarning("Request {RequestId} not found for update", id);
                return NotFound("Request not found");
            }

            // Authorization: EO/HR can only update their own, IT can update any
            if (role != "it" && request.SubmittedBy != userId)
            {
                _logger.LogWarning("User {UserId} not authorized to update request {RequestId}", userId, id);
                return Forbid();
            }

            // Update fields if provided
            if (!string.IsNullOrEmpty(updateDto.Status))
                request.Status = updateDto.Status;

            if (updateDto.EffectiveDate.HasValue)
                request.EffectiveDate = updateDto.EffectiveDate.Value;

            if (updateDto.Notes != null)
                request.Notes = updateDto.Notes;

            request.UpdatedAt = DateTime.UtcNow;

            _dbContext.LifecycleRequests.Update(request);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated lifecycle request {RequestId}", id);

            return Ok(MapToDto(request));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating request {RequestId}", id);
            return StatusCode(500, "An error occurred while updating the request");
        }
    }

    /// <summary>
    /// Create default workflow tasks based on request type
    /// </summary>
    private List<LifecycleTask> CreateDefaultTasks(string requestType)
    {
        var tasks = new List<LifecycleTask>();

        if (requestType == "onboard")
        {
            tasks.Add(new LifecycleTask { TaskType = "assign_device", Required = true, Completed = false });
            tasks.Add(new LifecycleTask { TaskType = "issue_badge", Required = false, Completed = false });
            tasks.Add(new LifecycleTask { TaskType = "collect_keys", Required = false, Completed = false });
        }
        else if (requestType == "offboard")
        {
            tasks.Add(new LifecycleTask { TaskType = "return_device", Required = true, Completed = false });
            tasks.Add(new LifecycleTask { TaskType = "collect_keys", Required = true, Completed = false });
        }

        return tasks;
    }

    /// <summary>
    /// Map LifecycleRequest entity to DTO
    /// </summary>
    private LifecycleRequestDto MapToDto(LifecycleRequest request)
    {
        return new LifecycleRequestDto
        {
            Id = request.Id,
            PersonId = request.PersonId,
            Person = request.Person != null ? new PersonDto
            {
                Id = request.Person.Id,
                FirstName = request.Person.FirstName,
                LastName = request.Person.LastName,
                PersonType = request.Person.PersonType,
                Status = request.Person.Status,
                StartDate = request.Person.StartDate,
                EndDate = request.Person.EndDate,
                Notes = request.Person.Notes,
                CreatedAt = request.Person.CreatedAt,
                UpdatedAt = request.Person.UpdatedAt
            } : null,
            RequestType = request.RequestType,
            Status = request.Status,
            SubmittedRole = request.SubmittedRole,
            SubmittedBy = request.SubmittedBy,
            EffectiveDate = request.EffectiveDate,
            Notes = request.Notes,
            Tasks = request.Tasks?.Select(t => new LifecycleTaskDto
            {
                Id = t.Id,
                LifecycleRequestId = t.LifecycleRequestId,
                TaskType = t.TaskType,
                Required = t.Required,
                Completed = t.Completed,
                CompletedAt = t.CompletedAt,
                Notes = t.Notes
            }).ToList(),
            TicketLinks = request.TicketLinks?.Select(tl => new TicketLinkDto
            {
                Id = tl.Id,
                LifecycleRequestId = tl.LifecycleRequestId,
                OsTicketTicketId = tl.OsTicketTicketId.ToString(),
                TicketType = tl.TicketType,
                CreatedAt = tl.CreatedAt
            }).ToList(),
            CreatedAt = request.CreatedAt
        };
    }
}
