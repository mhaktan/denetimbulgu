using System.Collections.Generic;
using Abp.Dependency;

namespace DenetimBulgu.Authorization
{
    /// <summary>Single permission descriptor — name, group (entity), description.</summary>
    public class PermissionInfo
    {
        public string Name { get; }
        public string Group { get; }
        public string Description { get; }
        public bool IsRbac { get; }

        public PermissionInfo(string name, string group, string description, bool isRbac)
        {
            Name = name; Group = group; Description = description; IsRbac = isRbac;
        }
    }

    public interface IPermissionRegistry
    {
        IReadOnlyList<PermissionInfo> All { get; }
    }

    public class PermissionRegistry : IPermissionRegistry, ISingletonDependency
    {
        public IReadOnlyList<PermissionInfo> All { get; } = new List<PermissionInfo>
        {
            new PermissionInfo("Department.Read", "Department", "Read Department", false),
            new PermissionInfo("Department.Create", "Department", "Create Department", false),
            new PermissionInfo("Department.Update", "Department", "Update Department", false),
            new PermissionInfo("Department.Delete", "Department", "Delete Department", false),
            new PermissionInfo("Employee.Read", "Employee", "Read Employee", false),
            new PermissionInfo("Employee.Create", "Employee", "Create Employee", false),
            new PermissionInfo("Employee.Update", "Employee", "Update Employee", false),
            new PermissionInfo("Employee.Delete", "Employee", "Delete Employee", false),
            new PermissionInfo("AuditType.Read", "AuditType", "Read AuditType", false),
            new PermissionInfo("AuditType.Create", "AuditType", "Create AuditType", false),
            new PermissionInfo("AuditType.Update", "AuditType", "Update AuditType", false),
            new PermissionInfo("AuditType.Delete", "AuditType", "Delete AuditType", false),
            new PermissionInfo("FindingLevel.Read", "FindingLevel", "Read FindingLevel", false),
            new PermissionInfo("FindingLevel.Create", "FindingLevel", "Create FindingLevel", false),
            new PermissionInfo("FindingLevel.Update", "FindingLevel", "Update FindingLevel", false),
            new PermissionInfo("FindingLevel.Delete", "FindingLevel", "Delete FindingLevel", false),
            new PermissionInfo("RequirementReference.Read", "RequirementReference", "Read RequirementReference", false),
            new PermissionInfo("RequirementReference.Create", "RequirementReference", "Create RequirementReference", false),
            new PermissionInfo("RequirementReference.Update", "RequirementReference", "Update RequirementReference", false),
            new PermissionInfo("RequirementReference.Delete", "RequirementReference", "Delete RequirementReference", false),
            new PermissionInfo("AuditPlan.Read", "AuditPlan", "Read AuditPlan", false),
            new PermissionInfo("AuditPlan.Create", "AuditPlan", "Create AuditPlan", false),
            new PermissionInfo("AuditPlan.Update", "AuditPlan", "Update AuditPlan", false),
            new PermissionInfo("AuditPlan.Delete", "AuditPlan", "Delete AuditPlan", false),
            new PermissionInfo("AuditPlan.ChangeStatus", "AuditPlan", "Change AuditPlan status", false),
            new PermissionInfo("Audit.Read", "Audit", "Read Audit", false),
            new PermissionInfo("Audit.Create", "Audit", "Create Audit", false),
            new PermissionInfo("Audit.Update", "Audit", "Update Audit", false),
            new PermissionInfo("Audit.Delete", "Audit", "Delete Audit", false),
            new PermissionInfo("Audit.ChangeStatus", "Audit", "Change Audit status", false),
            new PermissionInfo("AuditTeamMember.Read", "AuditTeamMember", "Read AuditTeamMember", false),
            new PermissionInfo("AuditTeamMember.Create", "AuditTeamMember", "Create AuditTeamMember", false),
            new PermissionInfo("AuditTeamMember.Update", "AuditTeamMember", "Update AuditTeamMember", false),
            new PermissionInfo("AuditTeamMember.Delete", "AuditTeamMember", "Delete AuditTeamMember", false),
            new PermissionInfo("Finding.Read", "Finding", "Read Finding", false),
            new PermissionInfo("Finding.Create", "Finding", "Create Finding", false),
            new PermissionInfo("Finding.Update", "Finding", "Update Finding", false),
            new PermissionInfo("Finding.Delete", "Finding", "Delete Finding", false),
            new PermissionInfo("Finding.ChangeStatus", "Finding", "Change Finding status", false),
            new PermissionInfo("CorrectiveAction.Read", "CorrectiveAction", "Read CorrectiveAction", false),
            new PermissionInfo("CorrectiveAction.Create", "CorrectiveAction", "Create CorrectiveAction", false),
            new PermissionInfo("CorrectiveAction.Update", "CorrectiveAction", "Update CorrectiveAction", false),
            new PermissionInfo("CorrectiveAction.Delete", "CorrectiveAction", "Delete CorrectiveAction", false),
            new PermissionInfo("CorrectiveAction.ChangeStatus", "CorrectiveAction", "Change CorrectiveAction status", false),
            new PermissionInfo("ActionProgressNote.Read", "ActionProgressNote", "Read ActionProgressNote", false),
            new PermissionInfo("ActionProgressNote.Create", "ActionProgressNote", "Create ActionProgressNote", false),
            new PermissionInfo("ActionProgressNote.Update", "ActionProgressNote", "Update ActionProgressNote", false),
            new PermissionInfo("ActionProgressNote.Delete", "ActionProgressNote", "Delete ActionProgressNote", false),
            new PermissionInfo("Evidence.Read", "Evidence", "Read Evidence", false),
            new PermissionInfo("Evidence.Create", "Evidence", "Create Evidence", false),
            new PermissionInfo("Evidence.Update", "Evidence", "Update Evidence", false),
            new PermissionInfo("Evidence.Delete", "Evidence", "Delete Evidence", false),
            new PermissionInfo("AppUser.Read", "AppUser", "Read users", true),
            new PermissionInfo("AppRole.Read", "AppRole", "Read roles", true),
            new PermissionInfo("AppUser.Create", "AppUser", "Create users", true),
            new PermissionInfo("AppRole.Create", "AppRole", "Create roles", true),
            new PermissionInfo("AppUser.Update", "AppUser", "Update users", true),
            new PermissionInfo("AppRole.Update", "AppRole", "Update roles", true),
            new PermissionInfo("AppUser.Delete", "AppUser", "Delete users", true),
            new PermissionInfo("AppRole.Delete", "AppRole", "Delete roles", true),
            new PermissionInfo("AppRole.AssignPermissions", "AppRole", "Assign permissions to roles", true),
        };
    }
}
