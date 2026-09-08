// Auto-generated flow: AuditPlan Approval Flow
// Auto-generated approval flow for AuditPlan. Customize email templates and add conditions as needed.
// Resource: AuditPlan
// Enabled: true
//
// Nodes:
  // trigger: On AuditPlan Submit
  // condition: Status = Draft?
  // approval: AuditPlan Approval
  // action: Send Approval Email
  // trigger: On AuditPlan Approved
  // action: Send Completion Email
//
// Edges:
  // On AuditPlan Submit → Status = Draft?
  // Status = Draft? → AuditPlan Approval (true)
  // AuditPlan Approval → Send Approval Email
  // On AuditPlan Approved → Send Completion Email
//
// This file is for documentation purposes.
// Flow execution is handled by FlowEngine.ts using flowDefinitions.json.

export const FLOW_AUDITPLAN_APPROVAL_FLOW_ID = 'flow-AuditPlan-approval';
