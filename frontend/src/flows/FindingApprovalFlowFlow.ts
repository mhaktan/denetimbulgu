// Auto-generated flow: Finding Approval Flow
// Auto-generated approval flow for Finding. Customize email templates and add conditions as needed.
// Resource: Finding
// Enabled: true
//
// Nodes:
  // trigger: On Finding Submit
  // condition: Status = InProgress?
  // approval: Finding Approval
  // action: Send Approval Email
  // trigger: On Finding Approved
  // action: Send Completion Email
//
// Edges:
  // On Finding Submit → Status = InProgress?
  // Status = InProgress? → Finding Approval (true)
  // Finding Approval → Send Approval Email
  // On Finding Approved → Send Completion Email
//
// This file is for documentation purposes.
// Flow execution is handled by FlowEngine.ts using flowDefinitions.json.

export const FLOW_FINDING_APPROVAL_FLOW_ID = 'flow-Finding-approval';
