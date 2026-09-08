import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { TkButton, TkInput, TkSelect } from '@takeoff-ui/react';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type EmployeeRecord = {
  id: string | number;
  registrationNumber: string;
  fullName: string;
  email: string;
  isActive: boolean;
  userId: string;
  departmentId: string;
};

interface EmployeeEditProps {
  record: EmployeeRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const EmployeeEdit: React.FC<EmployeeEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<EmployeeRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<EmployeeRecord>) =>
      dataProvider.update('Employee', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'Employee', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>Edit Employee</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                <div>
                  <TkInput mode="text" label="Registration Number *" value={String(form.registrationNumber ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('registrationNumber', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="text" label="Full Name *" value={String(form.fullName ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('fullName', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="text" label="Email *" value={String(form.email ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('email', v))(e.detail)} />
                </div>
                <div>
                  <label style={{ display: 'flex', alignItems: 'center', gap: 8, fontSize: 14 }}>
                    <input type="checkbox" checked={!!form.isActive} onChange={(e) => ((v) => setField('isActive', v))(e.target.checked)} style={{ width: 16, height: 16 }} />
                    Is Active *
                  </label>
                </div>
                <div>
                  <LookupSelect label="Kullanıcı (Sistem) *" resource="User" value={String(form.userId ?? '')} onChange={(v) => setField('userId', v)} displayField="name" />
                </div>
                <div>
                  <LookupSelect label="Birim *" resource="Department" value={String(form.departmentId ?? '')} onChange={(v) => setField('departmentId', v)} displayField="name" />
                </div>
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, padding: '16px 28px', borderTop: '1px solid #e8e8e8', flexShrink: 0, background: '#fff' }}>
            <TkButton label="Cancel" variant="secondary" onTkClick={onClose} />
            <TkButton label={mutation.isPending ? 'Saving…' : 'Save Changes'} variant="primary" mode="submit" disabled={mutation.isPending} />
          </div>
        </form>
      </div>
    </div>
  );
};
