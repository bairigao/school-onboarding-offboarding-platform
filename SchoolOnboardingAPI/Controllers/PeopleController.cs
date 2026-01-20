using Microsoft.AspNetCore.Mvc;
using SchoolOnboardingAPI.Data;
using SchoolOnboardingAPI.DTOs;
using SchoolOnboardingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace SchoolOnboardingAPI.Controllers;

/// <summary>
/// Controller for managing people (students and staff) in the system
/// Enrollment officers and HR staff can create and manage people
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PeopleController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PeopleController> _logger;

    public PeopleController(ApplicationDbContext dbContext, ILogger<PeopleController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Get all people, optionally filtered by type or status
    /// </summary>
    /// <param name="personType">Optional filter: "student" or "staff"</param>
    /// <param name="status">Optional filter: "onboarding", "active", "offboarding", "offboarded"</param>
    /// <param name="search">Optional search by first name or last name (case-insensitive)</param>
    /// <param name="page">Page number (default 1)</param>
    /// <param name="pageSize">Items per page (default 20)</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<PersonDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PersonDto>>> GetPeople(
        [FromQuery] string? personType = null,
        [FromQuery] string? status = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            _logger.LogInformation("Getting people list with filters - Type: {Type}, Status: {Status}, Search: {Search}", personType, status, search);

            var query = _dbContext.People.AsQueryable();

            if (!string.IsNullOrEmpty(personType))
                query = query.Where(p => p.PersonType == personType);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(p => p.Status == status);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.FirstName.ToLower().Contains(search.ToLower()) ||
                                         p.LastName.ToLower().Contains(search.ToLower()));

            var people = await query
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PersonDto
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    PersonType = p.PersonType,
                    Status = p.Status,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Notes = p.Notes,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} people", people.Count);
            return Ok(people);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving people list");
            return StatusCode(500, "An error occurred while retrieving people");
        }
    }

    /// <summary>
    /// Get a specific person by ID
    /// </summary>
    /// <param name="id">Person ID</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> GetPerson(int id)
    {
        try
        {
            _logger.LogInformation("Getting person {PersonId}", id);

            var person = await _dbContext.People.FindAsync(id);
            if (person == null)
            {
                _logger.LogWarning("Person {PersonId} not found", id);
                return NotFound("Person not found");
            }

            var dto = new PersonDto
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                PersonType = person.PersonType,
                Status = person.Status,
                StartDate = person.StartDate,
                EndDate = person.EndDate,
                Notes = person.Notes,
                CreatedAt = person.CreatedAt,
                UpdatedAt = person.UpdatedAt
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving person {PersonId}", id);
            return StatusCode(500, "An error occurred while retrieving the person");
        }
    }

    /// <summary>
    /// Create a new person (student or staff)
    /// Enrollment officers and HR staff can create people
    /// </summary>
    /// <param name="createDto">Person creation data</param>
    [HttpPost]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PersonDto>> CreatePerson([FromBody] CreatePersonDto createDto)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(createDto.FirstName) ||
                string.IsNullOrWhiteSpace(createDto.LastName) ||
                string.IsNullOrWhiteSpace(createDto.PersonType))
            {
                return BadRequest("First name, last name, and person type are required");
            }

            // Validate person type
            if (createDto.PersonType != "student" && createDto.PersonType != "staff")
            {
                return BadRequest("Person type must be 'student' or 'staff'");
            }

            var person = new Person
            {
                FirstName = createDto.FirstName.Trim(),
                LastName = createDto.LastName.Trim(),
                Identifier = $"{(createDto.PersonType == "student" ? "STU" : "EMP")}-{Guid.NewGuid().ToString().Substring(0, 8)}",
                PersonType = createDto.PersonType,
                Status = "onboarding", // New people start in onboarding
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.People.Add(person);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created person {PersonId}: {Name}", person.Id, person.FirstName);

            var resultDto = new PersonDto
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                PersonType = person.PersonType,
                Status = person.Status,
                StartDate = person.StartDate,
                EndDate = person.EndDate,
                Notes = person.Notes,
                CreatedAt = person.CreatedAt,
                UpdatedAt = person.UpdatedAt
            };

            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, resultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating person");
            return StatusCode(500, "An error occurred while creating the person");
        }
    }

    /// <summary>
    /// Update person information
    /// Enrollment officers and HR staff can update people they created
    /// </summary>
    /// <param name="id">Person ID to update</param>
    /// <param name="updateDto">Updated person data</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> UpdatePerson(int id, [FromBody] UpdatePersonDto updateDto)
    {
        try
        {
            var person = await _dbContext.People.FindAsync(id);
            if (person == null)
            {
                _logger.LogWarning("Person {PersonId} not found for update", id);
                return NotFound("Person not found");
            }

            // Update fields if provided
            if (!string.IsNullOrWhiteSpace(updateDto.FirstName))
                person.FirstName = updateDto.FirstName.Trim();

            if (!string.IsNullOrWhiteSpace(updateDto.LastName))
                person.LastName = updateDto.LastName.Trim();

            if (updateDto.EndDate.HasValue)
                person.EndDate = updateDto.EndDate;

            if (!string.IsNullOrWhiteSpace(updateDto.Status))
                person.Status = updateDto.Status;

            if (updateDto.Notes != null)
                person.Notes = updateDto.Notes;

            person.UpdatedAt = DateTime.UtcNow;

            _dbContext.People.Update(person);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated person {PersonId}", id);

            var resultDto = new PersonDto
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                PersonType = person.PersonType,
                Status = person.Status,
                StartDate = person.StartDate,
                EndDate = person.EndDate,
                Notes = person.Notes,
                CreatedAt = person.CreatedAt,
                UpdatedAt = person.UpdatedAt
            };

            return Ok(resultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating person {PersonId}", id);
            return StatusCode(500, "An error occurred while updating the person");
        }
    }
}
