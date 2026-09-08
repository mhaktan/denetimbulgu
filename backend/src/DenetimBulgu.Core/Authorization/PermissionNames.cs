namespace DenetimBulgu.Authorization
{
    public static class PermissionNames
    {
        public const string Pages = "Pages";

        // Department
        public const string Department_Read = "Department.Read";
        public const string Department_Create = "Department.Create";
        public const string Department_Update = "Department.Update";
        public const string Department_Delete = "Department.Delete";

        // Employee
        public const string Employee_Read = "Employee.Read";
        public const string Employee_Create = "Employee.Create";
        public const string Employee_Update = "Employee.Update";
        public const string Employee_Delete = "Employee.Delete";

        // AuditType
        public const string AuditType_Read = "AuditType.Read";
        public const string AuditType_Create = "AuditType.Create";
        public const string AuditType_Update = "AuditType.Update";
        public const string AuditType_Delete = "AuditType.Delete";

        // FindingLevel
        public const string FindingLevel_Read = "FindingLevel.Read";
        public const string FindingLevel_Create = "FindingLevel.Create";
        public const string FindingLevel_Update = "FindingLevel.Update";
        public const string FindingLevel_Delete = "FindingLevel.Delete";

        // RequirementReference
        public const string RequirementReference_Read = "RequirementReference.Read";
        public const string RequirementReference_Create = "RequirementReference.Create";
        public const string RequirementReference_Update = "RequirementReference.Update";
        public const string RequirementReference_Delete = "RequirementReference.Delete";

        // AuditPlan
        public const string AuditPlan_Read = "AuditPlan.Read";
        public const string AuditPlan_Create = "AuditPlan.Create";
        public const string AuditPlan_Update = "AuditPlan.Update";
        public const string AuditPlan_Delete = "AuditPlan.Delete";
        public const string AuditPlan_ChangeStatus = "AuditPlan.ChangeStatus";

        // Audit
        public const string Audit_Read = "Audit.Read";
        public const string Audit_Create = "Audit.Create";
        public const string Audit_Update = "Audit.Update";
        public const string Audit_Delete = "Audit.Delete";
        public const string Audit_ChangeStatus = "Audit.ChangeStatus";

        // AuditTeamMember
        public const string AuditTeamMember_Read = "AuditTeamMember.Read";
        public const string AuditTeamMember_Create = "AuditTeamMember.Create";
        public const string AuditTeamMember_Update = "AuditTeamMember.Update";
        public const string AuditTeamMember_Delete = "AuditTeamMember.Delete";

        // Finding
        public const string Finding_Read = "Finding.Read";
        public const string Finding_Create = "Finding.Create";
        public const string Finding_Update = "Finding.Update";
        public const string Finding_Delete = "Finding.Delete";
        public const string Finding_ChangeStatus = "Finding.ChangeStatus";

        // CorrectiveAction
        public const string CorrectiveAction_Read = "CorrectiveAction.Read";
        public const string CorrectiveAction_Create = "CorrectiveAction.Create";
        public const string CorrectiveAction_Update = "CorrectiveAction.Update";
        public const string CorrectiveAction_Delete = "CorrectiveAction.Delete";
        public const string CorrectiveAction_ChangeStatus = "CorrectiveAction.ChangeStatus";

        // ActionProgressNote
        public const string ActionProgressNote_Read = "ActionProgressNote.Read";
        public const string ActionProgressNote_Create = "ActionProgressNote.Create";
        public const string ActionProgressNote_Update = "ActionProgressNote.Update";
        public const string ActionProgressNote_Delete = "ActionProgressNote.Delete";

        // Evidence
        public const string Evidence_Read = "Evidence.Read";
        public const string Evidence_Create = "Evidence.Create";
        public const string Evidence_Update = "Evidence.Update";
        public const string Evidence_Delete = "Evidence.Delete";

        // RBAC management
        public const string AppUser_Read = "AppUser.Read";
        public const string AppRole_Read = "AppRole.Read";
        public const string AppUser_Create = "AppUser.Create";
        public const string AppRole_Create = "AppRole.Create";
        public const string AppUser_Update = "AppUser.Update";
        public const string AppRole_Update = "AppRole.Update";
        public const string AppUser_Delete = "AppUser.Delete";
        public const string AppRole_Delete = "AppRole.Delete";
        public const string AppRole_AssignPermissions = "AppRole.AssignPermissions";

    }
}
