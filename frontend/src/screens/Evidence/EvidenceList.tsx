import React, { useState, useCallback } from 'react';
import { TkButton } from '@takeoff-ui/react';
import { useListQuery } from '../../shared/useListQuery';
import { useDeleteMutation } from '../../shared/useDeleteMutation';
import { DeleteConfirmDialog } from '../../shared/DeleteConfirmDialog';
import { ListPageLayout } from '../../shared/ListPageLayout';
import type { TableColumn } from '../../shared/ListPageLayout';
import { actionColumn } from '../../shared/ActionButtons';
import { EvidenceCreate } from './EvidenceCreate';
import { EvidenceEdit } from './EvidenceEdit';
import { useFlows } from '../../flows/FlowProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

type EvidenceRecord = {
  id: string | number;
  description: string;
  fileName?: string;
  uploadDate: string;
  correctiveActionId: string;
  employeeId: string;
  [key: string]: unknown;
};

// ---------------------------------------------------------------------------
// Column definition — edit this array to add/remove/reorder columns
// ---------------------------------------------------------------------------
//
// Override examples:
//   • Hide a column:     remove its entry from COLUMNS
//   • Add a custom col:  { field: 'fullName', header: 'Full Name', html: (row) => `${row.firstName} ${row.lastName}` }
//   • Enable filtering:  add searchable: true  or  filterType: 'text' | 'checkbox' | 'radio' | 'datepicker'
//   • Custom cell render: html: (row) => `<span style="color:green">${row.status}</span>`
//
// Shared components (src/shared/) can be edited to change behavior globally:
//   • ListPageLayout  — table wrapper, pagination, header layout
//   • ActionButtons   — edit/delete button styles, labels, and behavior
//   • useListQuery    — data fetching, sorting, filtering logic
//   • useDeleteMutation / DeleteConfirmDialog — delete flow
//
// Action buttons override: edit src/shared/ActionButtons.ts DEFAULT_CONFIG
// or pass custom config: actionColumn('id', { hasEdit: true, config: { edit: { label: 'View', style: '...' } } })
//

const COLUMNS: TableColumn[] = [
  { field: 'id', header: 'ID', sortable: true },
  { field: 'description', header: "Description", sortable: true, searchable: true, filterType: 'text' },
  { field: 'fileName', header: "File Name", sortable: true, searchable: true, filterType: 'text' },
  { field: 'uploadDate', header: "Upload Date", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => row.uploadDate ? new Date(String(row.uploadDate)).toLocaleDateString() : '—' },
  { field: 'correctiveActionId', header: "Düzeltici Aksiyon", sortable: true },
  { field: 'employeeId', header: "Personel", sortable: true },
];

// ---------------------------------------------------------------------------
// EvidenceList
// ---------------------------------------------------------------------------

export const EvidenceList: React.FC = () => {
  const list = useListQuery<EvidenceRecord>({ resource: 'Evidence' });
  const [showCreate, setShowCreate] = useState(false);
  const [editRecord, setEditRecord] = useState<EvidenceRecord | null>(null);
  const [selectedRows, setSelectedRows] = useState<EvidenceRecord[]>([]);
  const { triggerFlows } = useFlows();
  const del = useDeleteMutation('Evidence', (ids) => { ids.forEach(id => triggerFlows('delete', 'Evidence', { id })); });


  const columns = [...COLUMNS, ...actionColumn('id', { hasEdit: true, hasDelete: true })];

  const handleCrudAction = useCallback((action: string, id: string) => {
    const row = list.records.find((r) => String(r.id) === id);
    if (!row) return;
    if (action === 'edit') setEditRecord(row);
    if (action === 'delete') del.requestSingleDelete(row.id);
  }, [list.records]);

  return (
    <>
      <ListPageLayout
        title="Evidence"
        subtitle={Object.keys(list.displayParams).length > 0 ? (
          <div style={{ fontSize: 13, color: '#666', marginTop: 4 }}>
            {Object.entries(list.displayParams).map(([k, v]) => (
              <span key={k} style={{ marginRight: 12 }}>{k}: <strong>{v}</strong></span>
            ))}
          </div>
        ) : undefined}
        records={list.records}
        columns={columns}
        dataKey="id"
        total={list.total}
        loading={list.isLoading}
        page={list.page}
        perPage={list.perPage}
        onPageChange={list.setPage}
        onPerPageChange={list.setPerPage}
        onTableRequest={list.handleTableRequest}
        selectionMode="checkbox"
        selectedRows={selectedRows}
        onSelectionChange={(rows) => setSelectedRows(rows as EvidenceRecord[])}
        onCrudAction={handleCrudAction}
        headerActions={<>
          {selectedRows.length > 0 && (
            <TkButton label={`Delete (${selectedRows.length})`} variant="danger" onTkClick={() => del.requestDelete(selectedRows.map(r => r.id), `${selectedRows.length} record(s)`)} />
          )}

          <TkButton label="+ Create Evidence" variant="primary" onTkClick={() => setShowCreate(true)} />
        </>}
      />

      {list.isError && (
        <div style={{ padding: '10px 14px', background: '#fff3f3', border: '1px solid #f5c6c6', borderRadius: 6, color: '#c62828', fontSize: 13, marginBottom: 12 }}>
          Failed to load data: {(list.error as Error).message}
        </div>
      )}

      <DeleteConfirmDialog
        visible={!!del.deleteTarget}
        label={del.deleteTarget?.label ?? ''}
        isPending={del.isPending}
        onConfirm={del.confirmDelete}
        onCancel={() => del.setDeleteTarget(null)}
      />
      <EvidenceCreate open={showCreate} onClose={() => setShowCreate(false)} onSuccess={list.invalidate} />
      <EvidenceEdit record={editRecord} onClose={() => setEditRecord(null)} onSuccess={list.invalidate} />

    </>
  );
};

