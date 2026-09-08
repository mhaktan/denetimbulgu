using Abp.Authorization;
using Abp.Localization;

namespace DenetimBulgu.Authorization
{
    public class DenetimBulguAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull("Pages") ?? context.CreatePermission("Pages", L("Pages"));

            // Department
            pages.CreateChildPermission(PermissionNames.Department_Read, L("Department.Read"));
            pages.CreateChildPermission(PermissionNames.Department_Create, L("Department.Create"));
            pages.CreateChildPermission(PermissionNames.Department_Update, L("Department.Update"));
            pages.CreateChildPermission(PermissionNames.Department_Delete, L("Department.Delete"));

            // Employee
            pages.CreateChildPermission(PermissionNames.Employee_Read, L("Employee.Read"));
            pages.CreateChildPermission(PermissionNames.Employee_Create, L("Employee.Create"));
            pages.CreateChildPermission(PermissionNames.Employee_Update, L("Employee.Update"));
            pages.CreateChildPermission(PermissionNames.Employee_Delete, L("Employee.Delete"));

            // AuditType
            pages.CreateChildPermission(PermissionNames.AuditType_Read, L("AuditType.Read"));
            pages.CreateChildPermission(PermissionNames.AuditType_Create, L("AuditType.Create"));
            pages.CreateChildPermission(PermissionNames.AuditType_Update, L("AuditType.Update"));
            pages.CreateChildPermission(PermissionNames.AuditType_Delete, L("AuditType.Delete"));

            // FindingLevel
            pages.CreateChildPermission(PermissionNames.FindingLevel_Read, L("FindingLevel.Read"));
            pages.CreateChildPermission(PermissionNames.FindingLevel_Create, L("FindingLevel.Create"));
            pages.CreateChildPermission(PermissionNames.FindingLevel_Update, L("FindingLevel.Update"));
            pages.CreateChildPermission(PermissionNames.FindingLevel_Delete, L("FindingLevel.Delete"));

            // RequirementReference
            pages.CreateChildPermission(PermissionNames.RequirementReference_Read, L("RequirementReference.Read"));
            pages.CreateChildPermission(PermissionNames.RequirementReference_Create, L("RequirementReference.Create"));
            pages.CreateChildPermission(PermissionNames.RequirementReference_Update, L("RequirementReference.Update"));
            pages.CreateChildPermission(PermissionNames.RequirementReference_Delete, L("RequirementReference.Delete"));

            // AuditPlan
            pages.CreateChildPermission(PermissionNames.AuditPlan_Read, L("AuditPlan.Read"));
            pages.CreateChildPermission(PermissionNames.AuditPlan_Create, L("AuditPlan.Create"));
            pages.CreateChildPermission(PermissionNames.AuditPlan_Update, L("AuditPlan.Update"));
            pages.CreateChildPermission(PermissionNames.AuditPlan_Delete, L("AuditPlan.Delete"));
            pages.CreateChildPermission(PermissionNames.AuditPlan_ChangeStatus, L("AuditPlan.ChangeStatus"));

            // Audit
            pages.CreateChildPermission(PermissionNames.Audit_Read, L("Audit.Read"));
            pages.CreateChildPermission(PermissionNames.Audit_Create, L("Audit.Create"));
            pages.CreateChildPermission(PermissionNames.Audit_Update, L("Audit.Update"));
            pages.CreateChildPermission(PermissionNames.Audit_Delete, L("Audit.Delete"));
            pages.CreateChildPermission(PermissionNames.Audit_ChangeStatus, L("Audit.ChangeStatus"));

            // AuditTeamMember
            pages.CreateChildPermission(PermissionNames.AuditTeamMember_Read, L("AuditTeamMember.Read"));
            pages.CreateChildPermission(PermissionNames.AuditTeamMember_Create, L("AuditTeamMember.Create"));
            pages.CreateChildPermission(PermissionNames.AuditTeamMember_Update, L("AuditTeamMember.Update"));
            pages.CreateChildPermission(PermissionNames.AuditTeamMember_Delete, L("AuditTeamMember.Delete"));

            // Finding
            pages.CreateChildPermission(PermissionNames.Finding_Read, L("Finding.Read"));
            pages.CreateChildPermission(PermissionNames.Finding_Create, L("Finding.Create"));
            pages.CreateChildPermission(PermissionNames.Finding_Update, L("Finding.Update"));
            pages.CreateChildPermission(PermissionNames.Finding_Delete, L("Finding.Delete"));
            pages.CreateChildPermission(PermissionNames.Finding_ChangeStatus, L("Finding.ChangeStatus"));

            // CorrectiveAction
            pages.CreateChildPermission(PermissionNames.CorrectiveAction_Read, L("CorrectiveAction.Read"));
            pages.CreateChildPermission(PermissionNames.CorrectiveAction_Create, L("CorrectiveAction.Create"));
            pages.CreateChildPermission(PermissionNames.CorrectiveAction_Update, L("CorrectiveAction.Update"));
            pages.CreateChildPermission(PermissionNames.CorrectiveAction_Delete, L("CorrectiveAction.Delete"));
            pages.CreateChildPermission(PermissionNames.CorrectiveAction_ChangeStatus, L("CorrectiveAction.ChangeStatus"));

            // ActionProgressNote
            pages.CreateChildPermission(PermissionNames.ActionProgressNote_Read, L("ActionProgressNote.Read"));
            pages.CreateChildPermission(PermissionNames.ActionProgressNote_Create, L("ActionProgressNote.Create"));
            pages.CreateChildPermission(PermissionNames.ActionProgressNote_Update, L("ActionProgressNote.Update"));
            pages.CreateChildPermission(PermissionNames.ActionProgressNote_Delete, L("ActionProgressNote.Delete"));

            // Evidence
            pages.CreateChildPermission(PermissionNames.Evidence_Read, L("Evidence.Read"));
            pages.CreateChildPermission(PermissionNames.Evidence_Create, L("Evidence.Create"));
            pages.CreateChildPermission(PermissionNames.Evidence_Update, L("Evidence.Update"));
            pages.CreateChildPermission(PermissionNames.Evidence_Delete, L("Evidence.Delete"));

            // RBAC
            pages.CreateChildPermission(PermissionNames.AppUser_Read, L("AppUser.Read"));
            pages.CreateChildPermission(PermissionNames.AppRole_Read, L("AppRole.Read"));
            pages.CreateChildPermission(PermissionNames.AppUser_Create, L("AppUser.Create"));
            pages.CreateChildPermission(PermissionNames.AppRole_Create, L("AppRole.Create"));
            pages.CreateChildPermission(PermissionNames.AppUser_Update, L("AppUser.Update"));
            pages.CreateChildPermission(PermissionNames.AppRole_Update, L("AppRole.Update"));
            pages.CreateChildPermission(PermissionNames.AppUser_Delete, L("AppUser.Delete"));
            pages.CreateChildPermission(PermissionNames.AppRole_Delete, L("AppRole.Delete"));
            pages.CreateChildPermission(PermissionNames.AppRole_AssignPermissions, L("AppRole.AssignPermissions"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, DenetimBulguConsts.LocalizationSourceName);
        }
    }
}
