using Microsoft.EntityFrameworkCore;
using Abp.EntityFrameworkCore;
using DenetimBulgu.Entities;

namespace DenetimBulgu.EntityFrameworkCore
{
    public class DenetimBulguDbContext : AbpDbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<AuditType> AuditTypes { get; set; }
        public DbSet<FindingLevel> FindingLevels { get; set; }
        public DbSet<RequirementReference> RequirementReferences { get; set; }
        public DbSet<AuditPlan> AuditPlans { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<AuditTeamMember> AuditTeamMembers { get; set; }
        public DbSet<Finding> Findings { get; set; }
        public DbSet<CorrectiveAction> CorrectiveActions { get; set; }
        public DbSet<ActionProgressNote> ActionProgressNotes { get; set; }
        public DbSet<Evidence> Evidences { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<ApprovalRecord> ApprovalRecords { get; set; }
        public DbSet<StatusChangeLog> StatusChangeLogs { get; set; }


        public DenetimBulguDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Department 1:N Department
            modelBuilder.Entity<Department>()
                .HasOne(x => x.ParentDepartment)
                .WithMany(x => x.Departments)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Department 1:N Employee
            modelBuilder.Entity<Employee>()
                .HasOne(x => x.Department)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // AuditPlan 1:N Audit
            modelBuilder.Entity<Audit>()
                .HasOne(x => x.AuditPlan)
                .WithMany(x => x.Audits)
                .HasForeignKey(x => x.AuditPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // AuditType 1:N Audit
            modelBuilder.Entity<Audit>()
                .HasOne(x => x.AuditType)
                .WithMany(x => x.Audits)
                .HasForeignKey(x => x.AuditTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Department 1:N Audit
            modelBuilder.Entity<Audit>()
                .HasOne(x => x.Department)
                .WithMany(x => x.Audits)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee 1:N Audit
            modelBuilder.Entity<Audit>()
                .HasOne(x => x.Employee)
                .WithMany(x => x.Audits)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Audit 1:N AuditTeamMember
            modelBuilder.Entity<AuditTeamMember>()
                .HasOne(x => x.Audit)
                .WithMany(x => x.AuditTeamMembers)
                .HasForeignKey(x => x.AuditId)
                .OnDelete(DeleteBehavior.Cascade);

            // Employee 1:N AuditTeamMember
            modelBuilder.Entity<AuditTeamMember>()
                .HasOne(x => x.Employee)
                .WithMany(x => x.AuditTeamMembers)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Audit 1:N Finding
            modelBuilder.Entity<Finding>()
                .HasOne(x => x.Audit)
                .WithMany(x => x.Findings)
                .HasForeignKey(x => x.AuditId)
                .OnDelete(DeleteBehavior.Restrict);

            // FindingLevel 1:N Finding
            modelBuilder.Entity<Finding>()
                .HasOne(x => x.FindingLevel)
                .WithMany(x => x.Findings)
                .HasForeignKey(x => x.FindingLevelId)
                .OnDelete(DeleteBehavior.Restrict);

            // Department 1:N Finding
            modelBuilder.Entity<Finding>()
                .HasOne(x => x.Department)
                .WithMany(x => x.Findings)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // RequirementReference 1:N Finding
            modelBuilder.Entity<Finding>()
                .HasOne(x => x.RequirementReference)
                .WithMany(x => x.Findings)
                .HasForeignKey(x => x.RequirementReferenceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Finding 1:N CorrectiveAction
            modelBuilder.Entity<CorrectiveAction>()
                .HasOne(x => x.Finding)
                .WithMany(x => x.CorrectiveActions)
                .HasForeignKey(x => x.FindingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee 1:N CorrectiveAction
            modelBuilder.Entity<CorrectiveAction>()
                .HasOne(x => x.Employee)
                .WithMany(x => x.CorrectiveActions)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // CorrectiveAction 1:N ActionProgressNote
            modelBuilder.Entity<ActionProgressNote>()
                .HasOne(x => x.CorrectiveAction)
                .WithMany(x => x.ActionProgressNotes)
                .HasForeignKey(x => x.CorrectiveActionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Employee 1:N ActionProgressNote
            modelBuilder.Entity<ActionProgressNote>()
                .HasOne(x => x.Employee)
                .WithMany(x => x.ActionProgressNotes)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // CorrectiveAction 1:N Evidence
            modelBuilder.Entity<Evidence>()
                .HasOne(x => x.CorrectiveAction)
                .WithMany(x => x.Evidences)
                .HasForeignKey(x => x.CorrectiveActionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Employee 1:N Evidence
            modelBuilder.Entity<Evidence>()
                .HasOne(x => x.Employee)
                .WithMany(x => x.Evidences)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);


            // RBAC: AppUser N:N AppRole via UserRole junction
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            // RolePermission: AppRole 1:N RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RolePermission>()
                .HasIndex(rp => new { rp.RoleId, rp.PermissionName })
                .IsUnique();

            // AppRole.Name unique
            modelBuilder.Entity<AppRole>()
                .HasIndex(r => r.Name)
                .IsUnique();

        }
    }
}
