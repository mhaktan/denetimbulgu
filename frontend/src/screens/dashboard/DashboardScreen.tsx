import React from 'react';
import { UiCard } from '../../shared/ui';

import {
  ResponsiveContainer,
  LineChart, Line,
  BarChart, Bar,
  PieChart, Pie, Cell,
  CartesianGrid, XAxis, YAxis, Tooltip,
} from 'recharts';

import { API_BASE } from '../../config';
import { getRequestHeaders } from '../../dataProvider';


const LOADING_KEYFRAMES = `
@keyframes pulse { 0%, 100% { opacity: 1; } 50% { opacity: 0.4; } }
@keyframes spin { to { transform: rotate(360deg); } }
`;

export const DashboardScreen: React.FC = () => {
  const getNestedValue = (obj: Record<string, unknown>, path: string): unknown => {
    const direct = path.split('.').reduce<unknown>((o, k) => (o && typeof o === 'object') ? (o as Record<string, unknown>)[k] : undefined, obj);
    if (direct !== undefined) return direct;
    // Fallback: try last segment at top level (handles ABP-style {result: {...}} unwrapping)
    const segments = path.split('.');
    if (segments.length > 1 && obj && typeof obj === "object") {
      const lastKey = segments[segments.length - 1];
      const top = (obj as Record<string, unknown>)[lastKey];
      if (top !== undefined) return top;
    }
    return undefined;
  };

  // Unwrap common API envelopes: ABP {result, __abp}, generic {data}, etc.
  const unwrapResponse = (json: unknown): unknown => {
    if (!json || typeof json !== "object" || Array.isArray(json)) return json;
    const obj = json as Record<string, unknown>;
    // ABP envelope: {result, success, error, __abp}
    if ("__abp" in obj && "result" in obj) return obj.result;
    // Generic envelope: {success: true, data: X}
    if ("success" in obj && "data" in obj && Object.keys(obj).length <= 4) return obj.data;
    return json;
  };

  const extractArray = (json: unknown): Record<string, unknown>[] => {
    if (Array.isArray(json)) return json;
    if (json && typeof json === 'object') {
      const obj = json as Record<string, unknown>;
      for (const key of ['items', 'data', 'results', 'records', 'rows', 'list']) {
        if (Array.isArray(obj[key])) return obj[key] as Record<string, unknown>[];
      }
      // Recurse one level — handles {result: {items: [...]}}
      for (const val of Object.values(obj)) {
        if (val && typeof val === 'object' && !Array.isArray(val)) {
          const inner = val as Record<string, unknown>;
          for (const key of ['items', 'data', 'results', 'records', 'rows', 'list']) {
            if (Array.isArray(inner[key])) return inner[key] as Record<string, unknown>[];
          }
        }
        if (Array.isArray(val)) return val as Record<string, unknown>[];
      }
    }
    return [];
  };

  const [count_finding_0Data, setCount_finding_0Data] = React.useState<Record<string, unknown> | null>(null);
  const [count_finding_0Loading, setCount_finding_0Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/services/app/Finding/GetAll?Status=PendingClosure&MaxResultCount=1`, { headers: getRequestHeaders() });
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        setCount_finding_0Data(json);
      } catch { /* ignore */ }
      finally { setCount_finding_0Loading(false); }
    };
    fetchData();
  }, []);

  const [metric_finding_1Data, setMetric_finding_1Data] = React.useState<Record<string, unknown> | null>(null);
  const [metric_finding_1Loading, setMetric_finding_1Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/services/app/Finding/GetStats?Aggregate=avgDayDiff&FromField=DetectedDate&ToField=ClosedDate`, { headers: getRequestHeaders() });
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        setMetric_finding_1Data(json);
      } catch { /* ignore */ }
      finally { setMetric_finding_1Loading(false); }
    };
    fetchData();
  }, []);

  const [breakdown_finding_2Data, setBreakdown_finding_2Data] = React.useState<Record<string, unknown>[]>([]);
  const [breakdown_finding_2Loading, setBreakdown_finding_2Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/services/app/Finding/GetGroupedCount?StatusIn=Open%2CActionPlanned%2CInProgress%2CPendingClosure&GroupBy=FindingLevelId`, { headers: getRequestHeaders() });
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        const target = getNestedValue(rawJson as Record<string, unknown>, 'result') ?? getNestedValue(json, 'result');
        setBreakdown_finding_2Data(extractArray(target ?? json));
      } catch { /* ignore */ }
      finally { setBreakdown_finding_2Loading(false); }
    };
    fetchData();
  }, []);

  const [breakdown_finding_3Data, setBreakdown_finding_3Data] = React.useState<Record<string, unknown>[]>([]);
  const [breakdown_finding_3Loading, setBreakdown_finding_3Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/services/app/Finding/GetGroupedCount?StatusIn=Open%2CActionPlanned%2CInProgress%2CPendingClosure&GroupBy=DepartmentId`, { headers: getRequestHeaders() });
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        const target = getNestedValue(rawJson as Record<string, unknown>, 'result') ?? getNestedValue(json, 'result');
        setBreakdown_finding_3Data(extractArray(target ?? json));
      } catch { /* ignore */ }
      finally { setBreakdown_finding_3Loading(false); }
    };
    fetchData();
  }, []);

  const [breakdown_finding_4Data, setBreakdown_finding_4Data] = React.useState<Record<string, unknown>[]>([]);
  const [breakdown_finding_4Loading, setBreakdown_finding_4Loading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/services/app/Finding/GetGroupedCount?GroupBy=Status`, { headers: getRequestHeaders() });
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        const target = getNestedValue(rawJson as Record<string, unknown>, 'result') ?? getNestedValue(json, 'result');
        setBreakdown_finding_4Data(extractArray(target ?? json));
      } catch { /* ignore */ }
      finally { setBreakdown_finding_4Loading(false); }
    };
    fetchData();
  }, []);

  const [list_finding_5_tableData, setList_finding_5_tableData] = React.useState<Record<string, unknown> | null>(null);
  const [list_finding_5_tableLoading, setList_finding_5_tableLoading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/services/app/Finding/GetAll?StatusNot=Closed&DueDateTo=2026-09-08&MaxResultCount=10&Sorting=id%20desc`, { headers: getRequestHeaders() });
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        const target = getNestedValue(rawJson as Record<string, unknown>, 'result.items') ?? getNestedValue(json, 'result.items');
        setList_finding_5_tableData(target != null ? (target as Record<string, unknown>) : json);
      } catch { /* ignore */ }
      finally { setList_finding_5_tableLoading(false); }
    };
    fetchData();
  }, []);

  const [list_correctiveAction_6_tableData, setList_correctiveAction_6_tableData] = React.useState<Record<string, unknown> | null>(null);
  const [list_correctiveAction_6_tableLoading, setList_correctiveAction_6_tableLoading] = React.useState(true);
  React.useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`${API_BASE}/api/services/app/CorrectiveAction/GetAll?StatusIn=Open%2CInProgress&TargetDateTo=2026-09-15&MaxResultCount=10&Sorting=id%20desc`, { headers: getRequestHeaders() });
        // Token expired or invalid — clear auth and redirect to login
        if (res.status === 401) {
          ['_auth_token', '_bearer_token', '_refresh_token'].forEach(k => localStorage.removeItem(k));
          if (window.location.pathname !== '/login') window.location.href = '/login';
          return;
        }
        if (!res.ok) return;
        const rawJson = await res.json();
        // Auto-unwrap ABP/generic envelopes so {result.totalCount} or bare {totalCount} both work
        const json = unwrapResponse(rawJson) as Record<string, unknown>;
        const target = getNestedValue(rawJson as Record<string, unknown>, 'result.items') ?? getNestedValue(json, 'result.items');
        setList_correctiveAction_6_tableData(target != null ? (target as Record<string, unknown>) : json);
      } catch { /* ignore */ }
      finally { setList_correctiveAction_6_tableLoading(false); }
    };
    fetchData();
  }, []);

  return (
    <div>
      <style dangerouslySetInnerHTML={{ __html: LOADING_KEYFRAMES }} />
      <h1 style={{ margin: '0 0 24px', fontSize: 22, fontWeight: 700 }}>Dashboard</h1>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, 1fr)', gap: 16 }}>
        <div style={{ gridColumn: 'span 12' }}>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: 16 }}>
            <div style={{ gridColumn: 'span 3' }}>
              {count_finding_0Loading ? (
                <UiCard bodyStyle={{ padding: 20 }}>
                  <div style={{ height: 12, width: '40%', background: '#e0e0e0', borderRadius: 4, marginBottom: 12, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 28, width: '60%', background: '#e0e0e0', borderRadius: 4, marginBottom: 8, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 10, width: '50%', background: '#f0f0f0', borderRadius: 4, animation: 'pulse 1.5s ease-in-out infinite' }} />
                </UiCard>
              ) : (
              <UiCard bodyStyle={{ padding: 20 }}>
                <div style={{ fontSize: 12, color: '#888', marginBottom: 4 }}>Kapanış Onayı Bekleyen Bulgular</div>
                <div style={{ fontSize: 28, fontWeight: 700, color: '#1976d2' }}>{(getNestedValue(count_finding_0Data ?? {}, 'result.totalCount') as string | number) ?? '—'}</div>
              </UiCard>
              )}

            </div>
            <div style={{ gridColumn: 'span 3' }}>
              {metric_finding_1Loading ? (
                <UiCard bodyStyle={{ padding: 20 }}>
                  <div style={{ height: 12, width: '40%', background: '#e0e0e0', borderRadius: 4, marginBottom: 12, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 28, width: '60%', background: '#e0e0e0', borderRadius: 4, marginBottom: 8, animation: 'pulse 1.5s ease-in-out infinite' }} />
                  <div style={{ height: 10, width: '50%', background: '#f0f0f0', borderRadius: 4, animation: 'pulse 1.5s ease-in-out infinite' }} />
                </UiCard>
              ) : (
              <UiCard bodyStyle={{ padding: 20 }}>
                <div style={{ fontSize: 12, color: '#888', marginBottom: 4 }}>Ortalama Bulgu Kapanış Süresi (gün)</div>
                <div style={{ fontSize: 28, fontWeight: 700, color: '#4caf50' }}>{metric_finding_1Data?.['result'] ?? '—'}</div>
              </UiCard>
              )}

            </div>
          </div>
        </div>
        <div style={{ gridColumn: 'span 6' }}>
          {unknown_blockLoading ? (
            <UiCard header="Seviye Bazında Açık Bulgu Sayıları" bodyStyle={{ padding: 16, display: 'flex', alignItems: 'center', justifyContent: 'center', height: 280 }}>
              <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 12 }}>
                <div style={{ width: 32, height: 32, border: '3px solid #e0e0e0', borderTopColor: '#1976d2', borderRadius: '50%', animation: 'spin 0.8s linear infinite' }} />
                <span style={{ fontSize: 12, color: '#999' }}>Loading...</span>
              </div>
            </UiCard>
          ) : (
          <UiCard header="Seviye Bazında Açık Bulgu Sayıları" bodyStyle={{ padding: 16 }}>
            <ResponsiveContainer width="100%" height={280}>
                  <BarChart data={unknown_blockData}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="label" />
                    <YAxis />
                    <Tooltip />
                    <Bar dataKey="count" fill="#1976d2" />
                  </BarChart>
                </ResponsiveContainer>
          </UiCard>
          )}
        </div>
        <div style={{ gridColumn: 'span 6' }}>
          {unknown_blockLoading ? (
            <UiCard header="Birim Bazında Açık Bulgu Dağılımı" bodyStyle={{ padding: 16, display: 'flex', alignItems: 'center', justifyContent: 'center', height: 280 }}>
              <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 12 }}>
                <div style={{ width: 32, height: 32, border: '3px solid #e0e0e0', borderTopColor: '#1976d2', borderRadius: '50%', animation: 'spin 0.8s linear infinite' }} />
                <span style={{ fontSize: 12, color: '#999' }}>Loading...</span>
              </div>
            </UiCard>
          ) : (
          <UiCard header="Birim Bazında Açık Bulgu Dağılımı" bodyStyle={{ padding: 16 }}>
            <ResponsiveContainer width="100%" height={280}>
                  <PieChart>
                    <Pie data={unknown_blockData} dataKey="count" nameKey="label" cx="50%" cy="50%" outerRadius={80} label>
                      <Cell fill="#1976d2" />
                      <Cell fill="#ff9800" />
                      <Cell fill="#4caf50" />
                      <Cell fill="#e91e63" />
                      <Cell fill="#9c27b0" />
                    </Pie>
                    <Tooltip />
                  </PieChart>
                </ResponsiveContainer>
          </UiCard>
          )}
        </div>
        <div style={{ gridColumn: 'span 6' }}>
          {unknown_blockLoading ? (
            <UiCard header="Bulgu Durumu Dağılımı" bodyStyle={{ padding: 16, display: 'flex', alignItems: 'center', justifyContent: 'center', height: 280 }}>
              <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 12 }}>
                <div style={{ width: 32, height: 32, border: '3px solid #e0e0e0', borderTopColor: '#1976d2', borderRadius: '50%', animation: 'spin 0.8s linear infinite' }} />
                <span style={{ fontSize: 12, color: '#999' }}>Loading...</span>
              </div>
            </UiCard>
          ) : (
          <UiCard header="Bulgu Durumu Dağılımı" bodyStyle={{ padding: 16 }}>
            <ResponsiveContainer width="100%" height={280}>
                  <PieChart>
                    <Pie data={unknown_blockData} dataKey="count" nameKey="label" cx="50%" cy="50%" outerRadius={80} label>
                      <Cell fill="#1976d2" />
                      <Cell fill="#ff9800" />
                      <Cell fill="#4caf50" />
                      <Cell fill="#e91e63" />
                      <Cell fill="#9c27b0" />
                    </Pie>
                    <Tooltip />
                  </PieChart>
                </ResponsiveContainer>
          </UiCard>
          )}
        </div>
        <div style={{ gridColumn: 'span 12' }}>
          <h3 style={{ fontSize: 16, fontWeight: 600, margin: '0 0 12px' }}>Termini Geçmiş Bulgular</h3>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(1, 1fr)', gap: 16 }}>
            <div style={{ gridColumn: 'span 12' }}>
              <div style={{ background: '#fff', borderRadius: 8, overflow: 'hidden', border: '1px solid #e8e8e8' }}>
                {list_finding_5_tableLoading ? (
                  <div style={{ padding: 20, textAlign: 'center', color: '#999' }}>Loading...</div>
                ) : (
                  <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                    <thead><tr><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Title</th><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Due Date</th><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Status</th></tr></thead>
                    <tbody>
                      {(Array.isArray(list_finding_5_tableData) ? list_finding_5_tableData : extractArray(list_finding_5_tableData)).map((row: Record<string, unknown>, i: number) => (
                        <tr key={i}><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{String(row['title'] ?? '')}</td><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{String(row['dueDate'] ?? '')}</td><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{String(row['status'] ?? '')}</td></tr>
                      ))}
                    </tbody>
                  </table>
                )}
              </div>
            </div>
          </div>
        </div>
        <div style={{ gridColumn: 'span 12' }}>
          <h3 style={{ fontSize: 16, fontWeight: 600, margin: '0 0 12px' }}>Termini Yaklaşan Aksiyonlar (7 Gün İçinde)</h3>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(1, 1fr)', gap: 16 }}>
            <div style={{ gridColumn: 'span 12' }}>
              <div style={{ background: '#fff', borderRadius: 8, overflow: 'hidden', border: '1px solid #e8e8e8' }}>
                {list_correctiveAction_6_tableLoading ? (
                  <div style={{ padding: 20, textAlign: 'center', color: '#999' }}>Loading...</div>
                ) : (
                  <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                    <thead><tr><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Action Description</th><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Target Date</th><th style={{ padding: '8px 12px', textAlign: 'left', borderBottom: '2px solid #e0e0e0', fontSize: 12, color: '#666' }}>Status</th></tr></thead>
                    <tbody>
                      {(Array.isArray(list_correctiveAction_6_tableData) ? list_correctiveAction_6_tableData : extractArray(list_correctiveAction_6_tableData)).map((row: Record<string, unknown>, i: number) => (
                        <tr key={i}><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{String(row['actionDescription'] ?? '')}</td><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{String(row['targetDate'] ?? '')}</td><td style={{ padding: '8px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 }}>{String(row['status'] ?? '')}</td></tr>
                      ))}
                    </tbody>
                  </table>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
