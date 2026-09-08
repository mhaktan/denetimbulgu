import React, { useState, useCallback } from 'react';
import { TkButton } from '@takeoff-ui/react';
import { useListQuery } from '../../shared/useListQuery';
import { useDeleteMutation } from '../../shared/useDeleteMutation';
import { DeleteConfirmDialog } from '../../shared/DeleteConfirmDialog';
import { ListPageLayout } from '../../shared/ListPageLayout';
import type { TableColumn } from '../../shared/ListPageLayout';
import { actionColumn } from '../../shared/ActionButtons';
import { RequirementReferenceCreate } from './RequirementReferenceCreate';
import { RequirementReferenceEdit } from './RequirementReferenceEdit';
import { useFlows } from '../../flows/FlowProvider';

// ---------------------------------------------------------------------------
// Types
// ---------------------------------------------------------------------------

type RequirementReferenceRecord = {
  id: string | number;
  code: string;
  title: string;
  sourceStandard: string;
  description?: string;
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
  { field: 'code', header: "Code", sortable: true, searchable: true, filterType: 'text' },
  { field: 'title', header: "Title", sortable: true, searchable: true, filterType: 'text' },
  { field: 'sourceStandard', header: "Source Standard", sortable: true, filterType: 'radio', filterOptions: [{ label: "Authority", value: "0" }, { label: "ISO", value: "1" }, { label: "InternalProcedure", value: "2" }], html: (row: Record<string, unknown>) => { const m: Record<string, string> = {["0"]: "Authority", ["1"]: "ISO", ["2"]: "InternalProcedure"}; return m[String(row.sourceStandard ?? '')] ?? String(row.sourceStandard ?? '\u2014'); } },
  { field: 'description', header: "Description", sortable: true, searchable: true, filterType: 'text' },
];

// ---------------------------------------------------------------------------
// RequirementReferenceList
// ---------------------------------------------------------------------------

export const RequirementReferenceList: React.FC = () => {
  const list = useListQuery<RequirementReferenceRecord>({ resource: 'RequirementReference' });
  const [showCreate, setShowCreate] = useState(false);
  const [editRecord, setEditRecord] = useState<RequirementReferenceRecord | null>(null);
  const [selectedRows, setSelectedRows] = useState<RequirementReferenceRecord[]>([]);
  const { triggerFlows } = useFlows();
  const del = useDeleteMutation('RequirementReference', (ids) => { ids.forEach(id => triggerFlows('delete', 'RequirementReference', { id })); });


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
        title="RequirementReference"
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
        onSelectionChange={(rows) => setSelectedRows(rows as RequirementReferenceRecord[])}
        onCrudAction={handleCrudAction}
        headerActions={<>
          {selectedRows.length > 0 && (
            <TkButton label={`Delete (${selectedRows.length})`} variant="danger" onTkClick={() => del.requestDelete(selectedRows.map(r => r.id), `${selectedRows.length} record(s)`)} />
          )}

          <TkButton label="+ Create RequirementReference" variant="primary" onTkClick={() => setShowCreate(true)} />
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
      <RequirementReferenceCreate open={showCreate} onClose={() => setShowCreate(false)} onSuccess={list.invalidate} />
      <RequirementReferenceEdit record={editRecord} onClose={() => setEditRecord(null)} onSuccess={list.invalidate} />

    </>
  );
};

