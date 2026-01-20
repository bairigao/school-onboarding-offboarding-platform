namespace SchoolOnboardingAPI.DTOs;

/// <summary>
/// DTO for creating a new person (student or staff)
/// </summary>
public class CreatePersonDto
{
    /// <summary>
    /// Person's first name
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Person's last name
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Person type: student or staff
    /// </summary>
    public required string PersonType { get; set; } // "student" or "staff"

    /// <summary>
    /// Start date for this person (enrollment/hire date)
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Optional end date (for offboarding tracking)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Additional notes about the person
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for updating person information
/// </summary>
public class UpdatePersonDto
{
    /// <summary>
    /// Person's first name
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Person's last name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Optional end date (for offboarding tracking)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Current status: onboarding, active, offboarding, offboarded
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Additional notes about the person
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for returning person information
/// </summary>
public class PersonDto
{
    /// <summary>
    /// Person ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Person's first name
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Person's last name
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Full name (first + last)
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Person type: student or staff
    /// </summary>
    public required string PersonType { get; set; }

    /// <summary>
    /// Current status: onboarding, active, offboarding, offboarded
    /// </summary>
    public required string Status { get; set; }

    /// <summary>
    /// Start date for this person
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date if known (usually set during offboarding)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// When this record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When this record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
