import React, { useState, useCallback } from 'react';
import { TkButton } from '@takeoff-ui/react';
import { useListQuery } from '../../shared/useListQuery';
import { useDeleteMutation } from '../../shared/useDeleteMutation';
import { DeleteConfirmDialog } from '../../shared/DeleteConfirmDialog';
import { ListPageLayout } from '../../shared/ListPageLayout';
import type { TableColumn } from '../../shared/ListPageLayout';
import { actionColumn } from '../../shared/ActionButtons';
import { FindingCreate } from './FindingCreate';
import { FindingEdit } from './FindingEdit';
import { useFlows } from '../../flows/FlowProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

type FindingRecord = {
  id: string | number;
  title: string;
  description: string;
  detectedDate: string;
  dueDate: string;
  closedDate?: string;
  status: string;
  revisionNote?: string;
  auditId: string;
  findingLevelId: string;
  departmentId: string;
  requirementReferenceId: string;
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
  { field: 'title', header: "Title", sortable: true, searchable: true, filterType: 'text' },
  { field: 'description', header: "Description", sortable: true, searchable: true, filterType: 'text' },
  { field: 'detectedDate', header: "Detected Date", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => row.detectedDate ? new Date(String(row.detectedDate)).toLocaleDateString() : '—' },
  { field: 'dueDate', header: "Due Date", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => row.dueDate ? new Date(String(row.dueDate)).toLocaleDateString() : '—' },
  { field: 'closedDate', header: "Closed Date", sortable: true, filterType: 'datepicker', html: (row: Record<string, unknown>) => row.closedDate ? new Date(String(row.closedDate)).toLocaleDateString() : '—' },
  { field: 'status', header: "Status", sortable: true, filterType: 'radio', filterOptions: [{ label: "Open", value: "0" }, { label: "ActionPlanned", value: "1" }, { label: "InProgress", value: "2" }, { label: "PendingClosure", value: "3" }, { label: "Closed", value: "4" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "Open", ["1"]: "ActionPlanned", ["2"]: "InProgress", ["3"]: "PendingClosure", ["4"]: "Closed"}; return m[String(row.status ?? '')] ?? String(row.status ?? '\u2014'); } },
  { field: 'revisionNote', header: "Revision Note", sortable: true, searchable: true, filterType: 'text' },
  { field: 'auditId', header: "Denetim", sortable: true },
  { field: 'findingLevelId', header: "Bulgu Seviyesi", sortable: true },
  { field: 'departmentId', header: "Birim", sortable: true },
  { field: 'requirementReferenceId', header: "Gereksinim Kataloğu", sortable: true },
];

// ---------------------------------------------------------------------------
// FindingList
// ---------------------------------------------------------------------------

export const FindingList: React.FC = () => {
  const list = useListQuery<FindingRecord>({ resource: 'Finding' });
  const [showCreate, setShowCreate] = useState(false);
  const [editRecord, setEditRecord] = useState<FindingRecord | null>(null);
  const [selectedRows, setSelectedRows] = useState<FindingRecord[]>([]);
  const { triggerFlows } = useFlows();
  const del = useDeleteMutation('Finding', (ids) => { ids.forEach(id => triggerFlows('delete', 'Finding', { id })); });


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
        title="Finding"
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
        onSelectionChange={(rows) => setSelectedRows(rows as FindingRecord[])}
        onCrudAction={handleCrudAction}
        headerActions={<>
          {selectedRows.length > 0 && (
            <TkButton label={`Delete (${selectedRows.length})`} variant="danger" onTkClick={() => del.requestDelete(selectedRows.map(r => r.id), `${selectedRows.length} record(s)`)} />
          )}

          <TkButton label="+ Create Finding" variant="primary" onTkClick={() => setShowCreate(true)} />
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
      <FindingCreate open={showCreate} onClose={() => setShowCreate(false)} onSuccess={list.invalidate} />
      <FindingEdit record={editRecord} onClose={() => setEditRecord(null)} onSuccess={list.invalidate} />

    </>
  );
};

