using Microsoft.EntityFrameworkCore;
using SchoolOnboardingAPI.Models;

namespace SchoolOnboardingAPI.Data;

/// <summary>
/// Entity Framework Core DbContext for School Onboarding/Offboarding Platform.
/// Manages all database operations and relationships between entities.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets represent tables in the database
    public DbSet<Person> People { get; set; }
    public DbSet<LifecycleRequest> LifecycleRequests { get; set; }
    public DbSet<LifecycleTask> LifecycleTasks { get; set; }
    public DbSet<AssetAssignment> AssetAssignments { get; set; }
    public DbSet<TicketLink> TicketLinks { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    /// <summary>
    /// Configures the database schema and relationships.
    /// Called by Entity Framework when creating the database.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Person entity
        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PersonType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Identifier)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("student_id or staff_id");

            entity.Property(e => e.RoleOrHomeroom)
                .HasMaxLength(200);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("active");

            entity.Property(e => e.Notes)
                .HasColumnType("text");

            // Index for common queries
            entity.HasIndex(e => e.Identifier)
                .IsUnique()
                .HasDatabaseName("idx_person_identifier");

            entity.HasIndex(e => e.PersonType)
                .HasDatabaseName("idx_person_type");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("idx_person_status");

            // Relationships
            entity.HasMany(e => e.LifecycleRequests)
                .WithOne(e => e.Person)
                .HasForeignKey(e => e.PersonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_lifecycle_request_person");

            entity.HasMany(e => e.AssetAssignments)
                .WithOne(e => e.Person)
                .HasForeignKey(e => e.PersonId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_asset_assignment_person");
        });

        // Configure LifecycleRequest entity
        modelBuilder.Entity<LifecycleRequest>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.RequestType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.SubmittedBy)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.SubmittedRole)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("pending");

            entity.Property(e => e.Notes)
                .HasColumnType("text");

            // Indexes
            entity.HasIndex(e => e.PersonId)
                .HasDatabaseName("idx_lifecycle_request_person_id");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("idx_lifecycle_request_status");

            entity.HasIndex(e => new { e.PersonId, e.RequestType })
                .HasDatabaseName("idx_lifecycle_request_person_type");

            // Relationships
            entity.HasMany(e => e.Tasks)
                .WithOne(e => e.LifecycleRequest)
                .HasForeignKey(e => e.LifecycleRequestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_lifecycle_task_request");

            entity.HasMany(e => e.TicketLinks)
                .WithOne(e => e.LifecycleRequest)
                .HasForeignKey(e => e.LifecycleRequestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_ticket_link_request");
        });

        // Configure LifecycleTask entity
        modelBuilder.Entity<LifecycleTask>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TaskType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Notes)
                .HasColumnType("text");

            entity.Property(e => e.Completed)
                .HasDefaultValue(false);

            entity.Property(e => e.Required)
                .HasDefaultValue(true);

            // Indexes
            entity.HasIndex(e => e.LifecycleRequestId)
                .HasDatabaseName("idx_lifecycle_task_request_id");

            entity.HasIndex(e => e.TaskType)
                .HasDatabaseName("idx_lifecycle_task_type");

            entity.HasIndex(e => e.Completed)
                .HasDatabaseName("idx_lifecycle_task_completed");
        });

        // Configure AssetAssignment entity
        modelBuilder.Entity<AssetAssignment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AssetTag)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.ConditionNotes)
                .HasColumnType("text");

            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            entity.HasIndex(e => e.PersonId)
                .HasDatabaseName("idx_asset_assignment_person_id");

            entity.HasIndex(e => e.AssetTag)
                .IsUnique()
                .HasDatabaseName("idx_asset_assignment_tag");

            entity.HasIndex(e => e.SnipeItAssetId)
                .HasDatabaseName("idx_asset_assignment_snipeit_id");

            entity.HasIndex(e => e.ReturnedAt)
                .HasDatabaseName("idx_asset_assignment_returned");
        });

        // Configure TicketLink entity
        modelBuilder.Entity<TicketLink>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TicketType)
                .IsRequired()
                .HasMaxLength(50);

            // Indexes
            entity.HasIndex(e => e.LifecycleRequestId)
                .HasDatabaseName("idx_ticket_link_request_id");

            entity.HasIndex(e => e.OsTicketTicketId)
                .IsUnique()
                .HasDatabaseName("idx_ticket_link_osticket_id");
        });

        // Configure AuditLog entity
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.EntityType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Action)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.OldValue)
                .HasColumnType("text");

            entity.Property(e => e.NewValue)
                .HasColumnType("text");

            entity.Property(e => e.ChangedBy)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes for audit queries
            entity.HasIndex(e => new { e.EntityType, e.EntityId })
                .HasDatabaseName("idx_audit_log_entity");

            entity.HasIndex(e => e.Action)
                .HasDatabaseName("idx_audit_log_action");

            entity.HasIndex(e => e.ChangedAt)
                .HasDatabaseName("idx_audit_log_changed_at");

            entity.HasIndex(e => e.ChangedBy)
                .HasDatabaseName("idx_audit_log_changed_by");
        });
    }
}
