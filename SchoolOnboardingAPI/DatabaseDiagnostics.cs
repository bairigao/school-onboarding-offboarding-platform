using SchoolOnboardingAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace SchoolOnboardingAPI;

/// <summary>
/// Helper class for database diagnostics and verification
/// </summary>
public static class DatabaseDiagnostics
{
    /// <summary>
    /// Verify database connection and tables exist
    /// </summary>
    public static async Task VerifyDatabaseAsync(ApplicationDbContext context)
    {
        try
        {
            // Test connection
            var canConnect = await context.Database.CanConnectAsync();
            Console.WriteLine($"✓ Database Connection: {(canConnect ? "SUCCESS" : "FAILED")}");

            // Get database name
            var dbName = context.Database.GetDbConnection().Database;
            Console.WriteLine($"✓ Database Name: {dbName}");

            // Get all table names
            var tables = await GetTablesAsync(context);
            Console.WriteLine($"\n✓ Tables Created ({tables.Count}):");
            foreach (var table in tables)
            {
                Console.WriteLine($"  - {table}");
            }

            // Get record counts
            Console.WriteLine("\n✓ Record Counts:");
            var counts = await GetTableCountsAsync(context);
            foreach (var (table, count) in counts)
            {
                Console.WriteLine($"  - {table}: {count} records");
            }

            // Get migrations applied
            var migrations = await context.Database.GetAppliedMigrationsAsync();
            Console.WriteLine($"\n✓ Migrations Applied ({migrations.Count()}):");
            foreach (var migration in migrations)
            {
                Console.WriteLine($"  - {migration}");
            }

            Console.WriteLine("\n✅ DATABASE VERIFICATION SUCCESSFUL!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ DATABASE VERIFICATION FAILED!");
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Get all table names from the database
    /// </summary>
    private static async Task<List<string>> GetTablesAsync(ApplicationDbContext context)
    {
        var tables = new List<string>();
        try
        {
            var connection = context.Database.GetDbConnection();
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT TABLE_NAME 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND TABLE_TYPE = 'BASE TABLE'
                ORDER BY TABLE_NAME";

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    tables.Add(reader.GetString(0));
                }
            }

            await connection.CloseAsync();
        }
        catch { }

        return tables;
    }

    /// <summary>
    /// Get record counts for all tables
    /// </summary>
    private static async Task<Dictionary<string, int>> GetTableCountsAsync(ApplicationDbContext context)
    {
        var counts = new Dictionary<string, int>();
        try
        {
            counts["People"] = await context.People.CountAsync();
            counts["LifecycleRequests"] = await context.LifecycleRequests.CountAsync();
            counts["LifecycleTasks"] = await context.LifecycleTasks.CountAsync();
            counts["AssetAssignments"] = await context.AssetAssignments.CountAsync();
            counts["TicketLinks"] = await context.TicketLinks.CountAsync();
            counts["AuditLogs"] = await context.AuditLogs.CountAsync();
        }
        catch { }

        return counts;
    }
}
