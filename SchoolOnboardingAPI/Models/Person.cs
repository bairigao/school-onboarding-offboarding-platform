namespace SchoolOnboardingAPI.Models;

/// <summary>
/// Represents a student or staff member in the system.
/// Central entity for managing person information throughout their lifecycle.
/// </summary>
public class Person
{
    public int Id { get; set; }
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    /// <summary>
    /// Type of person: "student" or "staff"
    /// </summary>
    public string PersonType { get; set; } = null!;
    
    /// <summary>
    /// Unique identifier: student_id for students, staff_id for staff
    /// </summary>
    public string Identifier { get; set; } = null!;
    
    /// <summary>
    /// For teachers: their role (e.g., "Mathematics Teacher")
    /// For students: their homeroom (e.g., "Grade 10 - Room 101")
    /// </summary>
    public string? RoleOrHomeroom { get; set; }
    
    /// <summary>
    /// Current status in lifecycle: onboarding | active | offboarding | offboarded
    /// </summary>
    public string Status { get; set; } = "active";
    
    /// <summary>
    /// Date when person started/will start
    /// </summary>
    public DateTime? StartDate { get; set; }
    
    /// <summary>
    /// Date when person ended/will end
    /// </summary>
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// Additional notes: medical info, special cases, accommodations
    /// </summary>
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<LifecycleRequest> LifecycleRequests { get; set; } = new List<LifecycleRequest>();
    
    public ICollection<AssetAssignment> AssetAssignments { get; set; } = new List<AssetAssignment>();
}
