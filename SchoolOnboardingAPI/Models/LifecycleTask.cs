namespace SchoolOnboardingAPI.Models;

/// <summary>
/// Represents an individual task within a lifecycle workflow.
/// Tasks are generated from lifecycle requests and tracked for completion.
/// </summary>
public class LifecycleTask
{
    public int Id { get; set; }
    
    public int LifecycleRequestId { get; set; }
    
    /// <summary>
    /// Type of task: assign_device | return_device | issue_badge | collect_keys
    /// </summary>
    public string TaskType { get; set; } = null!;
    
    /// <summary>
    /// Whether this task must be completed (mandatory)
    /// </summary>
    public bool Required { get; set; } = true;
    
    /// <summary>
    /// Whether the task has been completed
    /// </summary>
    public bool Completed { get; set; } = false;
    
    /// <summary>
    /// Timestamp when the task was marked as completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Notes on task completion, issues, or additional information
    /// </summary>
    public string? Notes { get; set; }
    
    // Navigation property
    public LifecycleRequest LifecycleRequest { get; set; } = null!;
}
