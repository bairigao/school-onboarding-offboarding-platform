namespace SchoolOnboardingAPI.DTOs;

/// <summary>
/// DTO for submitting a new onboarding/offboarding request
/// </summary>
public class CreateLifecycleRequestDto
{
    /// <summary>
    /// Person ID being onboarded/offboarded
    /// </summary>
    public required int PersonId { get; set; }

    /// <summary>
    /// Type of request: "onboard" or "offboard"
    /// </summary>
    public required string RequestType { get; set; }

    /// <summary>
    /// Role of person submitting: "enrolment" or "hr"
    /// </summary>
    public required string SubmittedRole { get; set; }

    /// <summary>
    /// Username/ID of person submitting request
    /// </summary>
    public required string SubmittedBy { get; set; }

    /// <summary>
    /// Date when this request becomes effective
    /// </summary>
    public required DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Optional notes about the request
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for updating lifecycle request status
/// </summary>
public class UpdateLifecycleRequestDto
{
    /// <summary>
    /// New status: "pending", "in_progress", "completed"
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Updated effective date
    /// </summary>
    public DateTime? EffectiveDate { get; set; }

    /// <summary>
    /// Updated notes
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for returning lifecycle request information
/// </summary>
public class LifecycleRequestDto
{
    /// <summary>
    /// Request ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Person ID
    /// </summary>
    public int PersonId { get; set; }

    /// <summary>
    /// Person being onboarded/offboarded
    /// </summary>
    public PersonDto? Person { get; set; }

    /// <summary>
    /// Type: "onboard" or "offboard"
    /// </summary>
    public required string RequestType { get; set; }

    /// <summary>
    /// Current status: "pending", "in_progress", "completed"
    /// </summary>
    public required string Status { get; set; }

    /// <summary>
    /// Role of person submitting: "enrolment" or "hr"
    /// </summary>
    public required string SubmittedRole { get; set; }

    /// <summary>
    /// Username/ID of person submitting
    /// </summary>
    public required string SubmittedBy { get; set; }

    /// <summary>
    /// Effective date
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Request notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Associated workflow tasks
    /// </summary>
    public List<LifecycleTaskDto>? Tasks { get; set; }

    /// <summary>
    /// Linked support tickets
    /// </summary>
    public List<TicketLinkDto>? TicketLinks { get; set; }

    /// <summary>
    /// When request was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for workflow task information
/// </summary>
public class LifecycleTaskDto
{
    /// <summary>
    /// Task ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Parent request ID
    /// </summary>
    public int LifecycleRequestId { get; set; }

    /// <summary>
    /// Task type: assign_device, return_device, issue_badge, collect_keys
    /// </summary>
    public required string TaskType { get; set; }

    /// <summary>
    /// Is this task required?
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Has task been completed?
    /// </summary>
    public bool Completed { get; set; }

    /// <summary>
    /// When task was completed (null if not completed)
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Additional task notes
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for updating lifecycle task completion
/// </summary>
public class UpdateLifecycleTaskDto
{
    /// <summary>
    /// Mark task as completed
    /// </summary>
    public bool? Completed { get; set; }

    /// <summary>
    /// Optional notes when completing task (e.g., device condition notes)
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for support ticket link information
/// </summary>
public class TicketLinkDto
{
    /// <summary>
    /// Link ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Parent request ID
    /// </summary>
    public int LifecycleRequestId { get; set; }

    /// <summary>
    /// OSTicket ticket ID
    /// </summary>
    public required string OsTicketTicketId { get; set; }

    /// <summary>
    /// Ticket type: onboarding, offboarding, issue
    /// </summary>
    public required string TicketType { get; set; }

    /// <summary>
    /// When ticket link was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
