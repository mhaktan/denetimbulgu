import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { TkButton, TkDatepicker, TkInput, TkSelect } from '@takeoff-ui/react';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type CorrectiveActionRecord = {
  id: string | number;
  rootCauseAnalysis: string;
  actionDescription: string;
  targetDate: string;
  actualClosureDate?: string;
  status: string;
  findingId: string;
  employeeId: string;
};

interface CorrectiveActionEditProps {
  record: CorrectiveActionRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const CorrectiveActionEdit: React.FC<CorrectiveActionEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<CorrectiveActionRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<CorrectiveActionRecord>) =>
      dataProvider.update('CorrectiveAction', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'CorrectiveAction', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>Edit CorrectiveAction</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                <div>
                  <TkInput mode="text" label="Root Cause Analysis *" value={String(form.rootCauseAnalysis ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('rootCauseAnalysis', v))(e.detail)} />
                </div>
                <div>
                  <TkInput mode="text" label="Action Description *" value={String(form.actionDescription ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('actionDescription', v))(e.detail)} />
                </div>
                <div>
                  <TkDatepicker label="Target Date *" value={String(form.targetDate ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('targetDate', v))(e.detail)} />
                </div>
                <div>
                  <TkDatepicker label="Actual Closure Date" value={String(form.actualClosureDate ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('actualClosureDate', v))(e.detail)} />
                </div>
                <div>
                  <LookupSelect label="Status *" value={String(form.status ?? '')} onChange={(v) => setField('status', v ? Number(v) : null)} searchable={false} options={[{ label: 'Open', value: '0' }, { label: 'InProgress', value: '1' }, { label: 'Completed', value: '2' }, { label: 'Cancelled', value: '3' }]} />
                </div>
                <div>
                  <LookupSelect label="Bulgu *" resource="Finding" value={String(form.findingId ?? '')} onChange={(v) => setField('findingId', v)} displayField="title" />
                </div>
                <div>
                  <LookupSelect label="Personel *" resource="Employee" value={String(form.employeeId ?? '')} onChange={(v) => setField('employeeId', v)} displayField="fullName" />
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
