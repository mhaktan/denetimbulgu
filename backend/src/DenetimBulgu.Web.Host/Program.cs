using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DenetimBulgu.EntityFrameworkCore;
using DenetimBulgu.EntityFrameworkCore.Seed;
using DenetimBulgu.Entities;

namespace DenetimBulgu.Web.Host
{
    /// <summary>
    /// Background service that runs migration + seed once at startup
    /// without blocking the HTTP pipeline.
    /// </summary>
    public class MigrationHostedService : IHostedService
    {
        private readonly IConfiguration _config;

        public MigrationHostedService(IConfiguration config)
        {
            _config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var connStr = _config.GetConnectionString("Default") ?? "";
            if (string.IsNullOrEmpty(connStr)) return;

            // Run in background to avoid blocking host startup (prevents EF tooling timeout)
            _ = Task.Run(async () =>
            {
                // Small delay to ensure host is fully started before DB operations
                await Task.Delay(1000, cancellationToken);
                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder();
                    optionsBuilder.UseNpgsql(connStr);

                    using (var db = new DenetimBulguDbContext(optionsBuilder.Options))
                    {
                        // EnsureCreated bos veritabaninda en guncel semayi kurar ve true doner.
                        // Dolu veritabaninda hicbir sey yapmaz — o durumda bekleyen migration'lar
                        // SchemaMigrations.Apply icinde sirayla calistirilir.
                        var freshlyCreated = db.Database.EnsureCreated();
                        Console.WriteLine("[Migration] Migration tanimi yok — sema EnsureCreated ile yonetiliyor.");
                        Console.WriteLine("[Migration] Database is up to date.");

                        // Seed sample data — wrapped in its own try so a failure here doesn't block RBAC seed below.
                        try
                        {
                    if (!db.Departments.Any())
                    {
                        db.Departments.AddRange(
                    new Department { Id = 1, Code = "ABC-001", Name = "Alice Johnson", IsActive = true, DepartmentId = 1 },
                    new Department { Id = 2, Code = "XYZ-002", Name = "Bob Smith", IsActive = false, DepartmentId = 2 }
                        );
                    }
                    if (!db.Employees.Any())
                    {
                        db.Employees.AddRange(
                    new Employee { Id = 3, RegistrationNumber = "ABC-001", FullName = "Alice Johnson", Email = "alice@example.com", IsActive = true, DepartmentId = 1 },
                    new Employee { Id = 4, RegistrationNumber = "XYZ-002", FullName = "Bob Smith", Email = "bob@example.com", IsActive = false, DepartmentId = 2 }
                        );
                    }
                    if (!db.AuditTypes.Any())
                    {
                        db.AuditTypes.AddRange(
                    new AuditType { Id = 5, Code = "ABC-001", Name = "Alice Johnson", Description = "Lorem ipsum dolor sit amet" },
                    new AuditType { Id = 6, Code = "XYZ-002", Name = "Bob Smith", Description = "Consectetur adipiscing elit" }
                        );
                    }
                    if (!db.FindingLevels.Any())
                    {
                        db.FindingLevels.AddRange(
                    new FindingLevel { Id = 7, Code = "ABC-001", Name = "Alice Johnson", DefaultClosureDays = 42 },
                    new FindingLevel { Id = 8, Code = "XYZ-002", Name = "Bob Smith", DefaultClosureDays = 17 }
                        );
                    }
                    if (!db.RequirementReferences.Any())
                    {
                        db.RequirementReferences.AddRange(
                    new RequirementReference { Id = 9, Code = "ABC-001", Title = "Introduction to Physics", SourceStandard = (RequirementReferenceSourceStandard)0, Description = "Lorem ipsum dolor sit amet" },
                    new RequirementReference { Id = 10, Code = "XYZ-002", Title = "Advanced Mathematics", SourceStandard = (RequirementReferenceSourceStandard)1, Description = "Consectetur adipiscing elit" }
                        );
                    }
                    if (!db.AuditPlans.Any())
                    {
                        db.AuditPlans.AddRange(
                    new AuditPlan { Id = 11, Year = 42, Period = "Sample Item 1", ScopeDescription = "Lorem ipsum dolor sit amet", Status = (AuditPlanStatus)0, QualityManagerApproverId = 1000L },
                    new AuditPlan { Id = 12, Year = 17, Period = "Sample Item 2", ScopeDescription = "Consectetur adipiscing elit", Status = (AuditPlanStatus)1, QualityManagerApproverId = 2000L }
                        );
                    }
                    if (!db.Audits.Any())
                    {
                        db.Audits.AddRange(
                    new Audit { Id = 13, AuditNumber = "ABC-001", PlannedDate = new DateTime(2024, 3, 15), ActualDate = new DateTime(2024, 3, 15), Status = (AuditStatus)0, AuditPlanId = 11, AuditTypeId = 5, DepartmentId = 1, EmployeeId = 3 },
                    new Audit { Id = 14, AuditNumber = "XYZ-002", PlannedDate = new DateTime(2024, 6, 20), ActualDate = new DateTime(2024, 6, 20), Status = (AuditStatus)1, AuditPlanId = 12, AuditTypeId = 6, DepartmentId = 2, EmployeeId = 4 }
                        );
                    }
                    if (!db.AuditTeamMembers.Any())
                    {
                        db.AuditTeamMembers.AddRange(
                    new AuditTeamMember { Id = 15, Role = (AuditTeamMemberRole)0, AuditId = 13, EmployeeId = 3 },
                    new AuditTeamMember { Id = 16, Role = (AuditTeamMemberRole)1, AuditId = 14, EmployeeId = 4 }
                        );
                    }
                    if (!db.Findings.Any())
                    {
                        db.Findings.AddRange(
                    new Finding { Id = 17, Title = "Introduction to Physics", Description = "Lorem ipsum dolor sit amet", DetectedDate = new DateTime(2024, 3, 15), DueDate = new DateTime(2024, 3, 15), ClosedDate = new DateTime(2024, 3, 15), Status = (FindingStatus)0, RevisionNote = "Lorem ipsum dolor sit amet", AuditId = 13, FindingLevelId = 7, DepartmentId = 1, RequirementReferenceId = 9 },
                    new Finding { Id = 18, Title = "Advanced Mathematics", Description = "Consectetur adipiscing elit", DetectedDate = new DateTime(2024, 6, 20), DueDate = new DateTime(2024, 6, 20), ClosedDate = new DateTime(2024, 6, 20), Status = (FindingStatus)1, RevisionNote = "Consectetur adipiscing elit", AuditId = 14, FindingLevelId = 8, DepartmentId = 2, RequirementReferenceId = 10 }
                        );
                    }
                    if (!db.CorrectiveActions.Any())
                    {
                        db.CorrectiveActions.AddRange(
                    new CorrectiveAction { Id = 19, RootCauseAnalysis = "Sample Item 1", ActionDescription = "Lorem ipsum dolor sit amet", TargetDate = new DateTime(2024, 3, 15), ActualClosureDate = new DateTime(2024, 3, 15), Status = (CorrectiveActionStatus)0, FindingId = 17, EmployeeId = 3 },
                    new CorrectiveAction { Id = 20, RootCauseAnalysis = "Sample Item 2", ActionDescription = "Consectetur adipiscing elit", TargetDate = new DateTime(2024, 6, 20), ActualClosureDate = new DateTime(2024, 6, 20), Status = (CorrectiveActionStatus)1, FindingId = 18, EmployeeId = 4 }
                        );
                    }
                    if (!db.ActionProgressNotes.Any())
                    {
                        db.ActionProgressNotes.AddRange(
                    new ActionProgressNote { Id = 21, Note = "Lorem ipsum dolor sit amet", NoteDate = new DateTime(2024, 3, 15), CorrectiveActionId = 19, EmployeeId = 3 },
                    new ActionProgressNote { Id = 22, Note = "Consectetur adipiscing elit", NoteDate = new DateTime(2024, 6, 20), CorrectiveActionId = 20, EmployeeId = 4 }
                        );
                    }
                    if (!db.Evidences.Any())
                    {
                        db.Evidences.AddRange(
                    new Evidence { Id = 23, Description = "Lorem ipsum dolor sit amet", FileName = "Alice Johnson", UploadDate = new DateTime(2024, 3, 15), CorrectiveActionId = 19, EmployeeId = 3 },
                    new Evidence { Id = 24, Description = "Consectetur adipiscing elit", FileName = "Bob Smith", UploadDate = new DateTime(2024, 6, 20), CorrectiveActionId = 20, EmployeeId = 4 }
                        );
                    }
                            db.SaveChanges();
                            Console.WriteLine("[Seed] Sample data created.");
                        }
                        catch (Exception sampleEx)
                        {
                            Console.WriteLine($"[Seed] Sample data skipped: {sampleEx.GetType().Name}: {sampleEx.Message}");
                            // Carry on — RBAC seed must still run so admin/123qwe is usable.
                        }
                        // Sync identity sequences to MAX(Id). Seeded rows carry explicit Ids which do NOT
                        // advance Postgres identity sequences → nextval collides with a seed row and the
                        // first few inserts fail with a duplicate-key 500. Runs every startup; idempotent.
                        try
                        {
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Departments\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Departments\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Employees\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Employees\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"AuditTypes\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"AuditTypes\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"FindingLevels\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"FindingLevels\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"RequirementReferences\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"RequirementReferences\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"AuditPlans\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"AuditPlans\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Audits\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Audits\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"AuditTeamMembers\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"AuditTeamMembers\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Findings\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Findings\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"CorrectiveActions\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"CorrectiveActions\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"ActionProgressNotes\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"ActionProgressNotes\") + 1, false);");
                            db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Evidences\"', 'Id'), (SELECT COALESCE(MAX(\"Id\"), 0) FROM \"Evidences\") + 1, false);");
                            Console.WriteLine("[Seed] Identity sequences synced.");
                        }
                        catch (Exception seqEx)
                        {
                            Console.WriteLine($"[Seed] Sequence sync skipped: {seqEx.GetType().Name}: {seqEx.Message}");
                        }
                    }
                    // RBAC seed (Admin/User roles + permissions + admin user) runs through ABP DI
                    // so PermissionRegistry can be injected. SeedHelper is idempotent.
                    SeedHelper.SeedHostDb(Abp.Dependency.IocManager.Instance);
                    Console.WriteLine("[Seed] RBAC seed complete (Admin role + admin user).");
                }
                catch (Exception ex)
                {
                    // Full diagnostic — surface the real cause so silent seed failures are debuggable.
                    Console.WriteLine($"[Migration] FAILED: {ex.GetType().Name}: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"[Migration] InnerException: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                    Console.WriteLine("[Migration] StackTrace:");
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("[Migration] App continues without migration — admin user will not exist.");
                }
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    public class Program
    {
        // Runtime entry: WebHost is required because ABP Startup returns IServiceProvider.
        public static void Main(string[] args)
        {
            // Npgsql 7+ requires UTC DateTimes — enable legacy behavior for ABP compatibility
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        // Design-time entry for EF Core tools (dotnet ef migrations).
        // Without this, EF tools wait 5 minutes for IHost build (resolver default timeout)
        // and then SIGTERM any running dotnet process — killing live dev servers.
        // We expose a minimal IHost that EF tools resolve in milliseconds; the actual
        // DbContext is built by IDesignTimeDbContextFactory in the EntityFrameworkCore project.
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);
    }
}
