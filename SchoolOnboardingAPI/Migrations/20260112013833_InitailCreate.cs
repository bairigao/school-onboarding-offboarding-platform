using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolOnboardingAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitailCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: true),
                    NewValue = table.Column<string>(type: "text", nullable: true),
                    ChangedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PersonType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "student_id or staff_id"),
                    RoleOrHomeroom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "active"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    SnipeItAssetId = table.Column<int>(type: "int", nullable: true),
                    AssetTag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ReturnedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConditionNotes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "fk_asset_assignment_person",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LifecycleRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SubmittedRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "pending"),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifecycleRequests", x => x.Id);
                    table.ForeignKey(
                        name: "fk_lifecycle_request_person",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LifecycleTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LifecycleRequestId = table.Column<int>(type: "int", nullable: false),
                    TaskType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Required = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Completed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifecycleTasks", x => x.Id);
                    table.ForeignKey(
                        name: "fk_lifecycle_task_request",
                        column: x => x.LifecycleRequestId,
                        principalTable: "LifecycleRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LifecycleRequestId = table.Column<int>(type: "int", nullable: false),
                    OsTicketTicketId = table.Column<int>(type: "int", nullable: false),
                    TicketType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketLinks", x => x.Id);
                    table.ForeignKey(
                        name: "fk_ticket_link_request",
                        column: x => x.LifecycleRequestId,
                        principalTable: "LifecycleRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_asset_assignment_person_id",
                table: "AssetAssignments",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "idx_asset_assignment_returned",
                table: "AssetAssignments",
                column: "ReturnedAt");

            migrationBuilder.CreateIndex(
                name: "idx_asset_assignment_snipeit_id",
                table: "AssetAssignments",
                column: "SnipeItAssetId");

            migrationBuilder.CreateIndex(
                name: "idx_asset_assignment_tag",
                table: "AssetAssignments",
                column: "AssetTag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_audit_log_action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "idx_audit_log_changed_at",
                table: "AuditLogs",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "idx_audit_log_changed_by",
                table: "AuditLogs",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "idx_audit_log_entity",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "idx_lifecycle_request_person_id",
                table: "LifecycleRequests",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "idx_lifecycle_request_person_type",
                table: "LifecycleRequests",
                columns: new[] { "PersonId", "RequestType" });

            migrationBuilder.CreateIndex(
                name: "idx_lifecycle_request_status",
                table: "LifecycleRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "idx_lifecycle_task_completed",
                table: "LifecycleTasks",
                column: "Completed");

            migrationBuilder.CreateIndex(
                name: "idx_lifecycle_task_request_id",
                table: "LifecycleTasks",
                column: "LifecycleRequestId");

            migrationBuilder.CreateIndex(
                name: "idx_lifecycle_task_type",
                table: "LifecycleTasks",
                column: "TaskType");

            migrationBuilder.CreateIndex(
                name: "idx_person_identifier",
                table: "People",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_person_status",
                table: "People",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "idx_person_type",
                table: "People",
                column: "PersonType");

            migrationBuilder.CreateIndex(
                name: "idx_ticket_link_osticket_id",
                table: "TicketLinks",
                column: "OsTicketTicketId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_ticket_link_request_id",
                table: "TicketLinks",
                column: "LifecycleRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetAssignments");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "LifecycleTasks");

            migrationBuilder.DropTable(
                name: "TicketLinks");

            migrationBuilder.DropTable(
                name: "LifecycleRequests");

            migrationBuilder.DropTable(
                name: "People");
        }
    }
}
