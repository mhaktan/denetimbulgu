// ---------------------------------------------------------------------------
// Enum definitions — auto-generated from ER model
// Maps integer values to display labels for enum fields
// ---------------------------------------------------------------------------

// RequirementReference.sourceStandard
export const RequirementReferenceSourceStandardMap: Record<string, string> = {
  '0': 'Authority',
  '1': 'ISO',
  '2': 'InternalProcedure'
};
export const RequirementReferenceSourceStandardOptions = [
  { label: 'Authority', value: '0' },
  { label: 'ISO', value: '1' },
  { label: 'InternalProcedure', value: '2' }
];

// AuditPlan.status
export const AuditPlanStatusMap: Record<string, string> = {
  '0': 'Draft',
  '1': 'Approved'
};
export const AuditPlanStatusOptions = [
  { label: 'Draft', value: '0' },
  { label: 'Approved', value: '1' }
];

// Audit.status
export const AuditStatusMap: Record<string, string> = {
  '0': 'Planned',
  '1': 'InProgress',
  '2': 'Completed'
};
export const AuditStatusOptions = [
  { label: 'Planned', value: '0' },
  { label: 'InProgress', value: '1' },
  { label: 'Completed', value: '2' }
];

// AuditTeamMember.role
export const AuditTeamMemberRoleMap: Record<string, string> = {
  '0': 'LeadAuditor',
  '1': 'Auditor',
  '2': 'Observer'
};
export const AuditTeamMemberRoleOptions = [
  { label: 'LeadAuditor', value: '0' },
  { label: 'Auditor', value: '1' },
  { label: 'Observer', value: '2' }
];

// Finding.status
export const FindingStatusMap: Record<string, string> = {
  '0': 'Open',
  '1': 'ActionPlanned',
  '2': 'InProgress',
  '3': 'PendingClosure',
  '4': 'Closed'
};
export const FindingStatusOptions = [
  { label: 'Open', value: '0' },
  { label: 'ActionPlanned', value: '1' },
  { label: 'InProgress', value: '2' },
  { label: 'PendingClosure', value: '3' },
  { label: 'Closed', value: '4' }
];

// CorrectiveAction.status
export const CorrectiveActionStatusMap: Record<string, string> = {
  '0': 'Open',
  '1': 'InProgress',
  '2': 'Completed',
  '3': 'Cancelled'
};
export const CorrectiveActionStatusOptions = [
  { label: 'Open', value: '0' },
  { label: 'InProgress', value: '1' },
  { label: 'Completed', value: '2' },
  { label: 'Cancelled', value: '3' }
];
