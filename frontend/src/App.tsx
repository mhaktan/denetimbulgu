import React, { useState } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter, Route, Routes, Navigate } from 'react-router-dom';
import { isAuthenticated, clearAuth } from './dataProvider';
import { LoginPage } from './screens/LoginPage';
import { GlobalMenu } from './components/GlobalMenu';
import { FlowProvider } from './flows/FlowProvider';
import { DepartmentList } from './screens/Department/DepartmentList';
import { EmployeeList } from './screens/Employee/EmployeeList';
import { AuditTypeList } from './screens/AuditType/AuditTypeList';
import { FindingLevelList } from './screens/FindingLevel/FindingLevelList';
import { RequirementReferenceList } from './screens/RequirementReference/RequirementReferenceList';
import { AuditPlanList } from './screens/AuditPlan/AuditPlanList';
import { AuditList } from './screens/Audit/AuditList';
import { AuditTeamMemberList } from './screens/AuditTeamMember/AuditTeamMemberList';
import { FindingList } from './screens/Finding/FindingList';
import { CorrectiveActionList } from './screens/CorrectiveAction/CorrectiveActionList';
import { ActionProgressNoteList } from './screens/ActionProgressNote/ActionProgressNoteList';
import { EvidenceList } from './screens/Evidence/EvidenceList';
import { DashboardScreen } from './screens/dashboard/DashboardScreen';
import TaskInboxScreen from './screens/tasks/TaskInboxScreen';
import UserListScreen from './admin/UserListScreen';
import RoleListScreen from './admin/RoleListScreen';

const queryClient = new QueryClient({
  defaultOptions: { queries: { retry: 1, staleTime: 0 } },
});

export const App: React.FC = () => {
  const [authenticated, setAuthenticated] = useState(isAuthenticated());


  const handleLogout = () => {
    clearAuth();
    setAuthenticated(false);
    queryClient.clear();
  };

  return (
    <QueryClientProvider client={queryClient}>
      <FlowProvider>
      <BrowserRouter>
        <Routes>
          <Route path="*" element={
            authenticated
              ? (
                <GlobalMenu>
                  <Routes>
                    <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard" element={<DashboardScreen />} />
          <Route path="/Department" element={<DepartmentList />} />
          <Route path="/Employee" element={<EmployeeList />} />
          <Route path="/AuditType" element={<AuditTypeList />} />
          <Route path="/FindingLevel" element={<FindingLevelList />} />
          <Route path="/RequirementReference" element={<RequirementReferenceList />} />
          <Route path="/AuditPlan" element={<AuditPlanList />} />
          <Route path="/Audit" element={<AuditList />} />
          <Route path="/AuditTeamMember" element={<AuditTeamMemberList />} />
          <Route path="/Finding" element={<FindingList />} />
          <Route path="/CorrectiveAction" element={<CorrectiveActionList />} />
          <Route path="/ActionProgressNote" element={<ActionProgressNoteList />} />
          <Route path="/Evidence" element={<EvidenceList />} />
          <Route path="/tasks" element={<TaskInboxScreen />} />
          <Route path="/users" element={<UserListScreen />} />
          <Route path="/roles" element={<RoleListScreen />} />
                  </Routes>
                </GlobalMenu>
              )
              : <LoginPage onLogin={() => setAuthenticated(true)} />
          } />
        </Routes>
      </BrowserRouter>
        </FlowProvider>
    </QueryClientProvider>
  );
};
