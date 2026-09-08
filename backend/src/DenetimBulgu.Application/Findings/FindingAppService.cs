using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DenetimBulgu.Entities;
using DenetimBulgu.Findings.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.CorrectiveActions.Dto;
using DenetimBulgu.Approvals.Dto;
using DenetimBulgu.Authorization;
using DenetimBulgu.Flows;

namespace DenetimBulgu.Findings
{
    public class FindingAppService : AsyncCrudAppService<
        Finding,
        FindingDto,
        long,
        PagedFindingResultRequestDto,
        CreateFindingDto,
        FindingDto>,
        IFindingAppService
    {
        private readonly IRepository<StatusChangeLog, long> _statusChangeLogRepo;
        private readonly IRepository<ApprovalRecord, Guid> _approvalRepo;
        private readonly IFlowEngine _flowEngine;

        public FindingAppService(IRepository<Finding, long> repository, IFlowEngine flowEngine, IRepository<StatusChangeLog, long> statusChangeLogRepo, IRepository<ApprovalRecord, Guid> approvalRepo)
            : base(repository)
        {
            _flowEngine = flowEngine;
            _statusChangeLogRepo = statusChangeLogRepo;
            _approvalRepo = approvalRepo;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Finding_Read;
            GetAllPermissionName = PermissionNames.Finding_Read;
            CreatePermissionName = PermissionNames.Finding_Create;
            UpdatePermissionName = PermissionNames.Finding_Update;
            DeletePermissionName = PermissionNames.Finding_Delete;
        }

        protected override IQueryable<Finding> CreateFilteredQuery(PagedFindingResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Title != null && x.Title.Contains(input.Keyword)) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)) ||
                    (x.RevisionNote != null && x.RevisionNote.Contains(input.Keyword)))
                .WhereIf(!input.Title.IsNullOrWhiteSpace(), x => x.Title != null && x.Title.Contains(input.Title))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(!input.RevisionNote.IsNullOrWhiteSpace(), x => x.RevisionNote != null && x.RevisionNote.Contains(input.RevisionNote))
                .WhereIf(input.DetectedDate.HasValue, x => x.DetectedDate == input.DetectedDate.Value)
                .WhereIf(input.DueDate.HasValue, x => x.DueDate == input.DueDate.Value)
                .WhereIf(input.ClosedDate.HasValue, x => x.ClosedDate == input.ClosedDate.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (FindingStatus)input.Status.Value)
                .WhereIf(input.DetectedDateFrom.HasValue, x => x.DetectedDate >= input.DetectedDateFrom.Value)
                .WhereIf(input.DetectedDateTo.HasValue, x => x.DetectedDate <= input.DetectedDateTo.Value)
                .WhereIf(input.DueDateFrom.HasValue, x => x.DueDate >= input.DueDateFrom.Value)
                .WhereIf(input.DueDateTo.HasValue, x => x.DueDate <= input.DueDateTo.Value)
                .WhereIf(input.ClosedDateFrom.HasValue, x => x.ClosedDate >= input.ClosedDateFrom.Value)
                .WhereIf(input.ClosedDateTo.HasValue, x => x.ClosedDate <= input.ClosedDateTo.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (FindingStatus)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (FindingStatus)input.StatusNot.Value)
                .WhereIf(input.AuditId.HasValue, x => x.AuditId == input.AuditId.Value)
                .WhereIf(input.FindingLevelId.HasValue, x => x.FindingLevelId == input.FindingLevelId.Value)
                .WhereIf(input.DepartmentId.HasValue, x => x.DepartmentId == input.DepartmentId.Value)
                .WhereIf(input.RequirementReferenceId.HasValue, x => x.RequirementReferenceId == input.RequirementReferenceId.Value);
        }

        public override async Task<FindingDto> CreateAsync(CreateFindingDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Finding", result);

            // Frontend creates records with status pre-set without going through ChangeStatusAsync,
            // so mirror on-field-change here whenever the initial status isn't the default. Otherwise
            // status-driven flows (e.g. approval) never fire on plain Create.
            if (result.Status != (int)FindingStatus.Open)
                await _flowEngine.TriggerAsync("on-field-change", "Finding", result);
            return result;
        }

        public override async Task<FindingDto> UpdateAsync(FindingDto input)
        {
            // State machine: validate status transition + log
            var existing = await Repository.GetAsync(input.Id);
            var statusChanged = (int)existing.Status != input.Status;
            if (statusChanged)
            {
                var fromStatus = existing.Status.ToString();
                var toStatus = ((FindingStatus)input.Status).ToString();
                ValidateStatusTransition(existing.Status, (FindingStatus)input.Status);

                // Log status change
                await _statusChangeLogRepo.InsertAsync(new StatusChangeLog
                {
                    EntityType = "Finding",
                    EntityId = input.Id.ToString(),
                    FromStatus = fromStatus,
                    ToStatus = toStatus,
                    Action = "Update",
                    ChangedByUserId = AbpSession.UserId
                });
            }

            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Finding", result);

            // Frontend updates status via plain UpdateAsync (not ChangeStatusAsync) — fire
            // on-field-change so status-driven flows pick up the transition.
            if (statusChanged)
                await _flowEngine.TriggerAsync("on-field-change", "Finding", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Finding", new { Id = input.Id });
        }

        // Onay adimini tamamlayan rol genelde Update degil ChangeStatus yetkisine sahip olur;
        // RequireAllPermissions=false ile ikisinden biri yeterli (geriye donuk uyumlu).
        [Abp.Authorization.AbpAuthorize(PermissionNames.Finding_ChangeStatus, PermissionNames.Finding_Update, RequireAllPermissions = false)]
        public async Task<FindingDto> ChangeStatusAsync(long id, ChangeStatusInput input)
        {
            var entity = await Repository.GetAsync(id);
            var currentStatus = entity.Status.ToString();

            // Find valid transition
            var transitions = new (string From, string To, string Action, bool Readonly)[]
            {
            ("Open", "ActionPlanned", "Approve", false),
            ("ActionPlanned", "InProgress", "Approve", false),
            ("InProgress", "PendingClosure", "Approve", false),
            ("PendingClosure", "Closed", "Approve", true),
            ("PendingClosure", "InProgress", "Revise", false)
            };

            var transition = transitions.FirstOrDefault(t =>
                (t.From == "*" || t.From == currentStatus) && t.Action == input.Action);

            if (transition == default)
                throw new Abp.UI.UserFriendlyException($"Invalid action '{input.Action}' from status '{currentStatus}'");

            // Validate required fields per transition
            if (input.Action == "Revise" && (input.ActionData == null || !input.ActionData.ContainsKey("revisionNote") || string.IsNullOrWhiteSpace(input.ActionData["revisionNote"])))
                throw new Abp.UI.UserFriendlyException("Revise requires: revisionNote");
            // Bu gecisler icin bagli kayit on kosulu yok

            var fromStatus = currentStatus;

            // Apply new status
            entity.Status = (FindingStatus)Enum.Parse(typeof(FindingStatus), transition.To);
            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Cancel pending ApprovalRecords when the entity is cancelled — otherwise the records
            // sit forever in approvers' inboxes pointing to a cancelled request.
            if (input.Action == "Cancel")
            {
                var pending = _approvalRepo.GetAll()
                    .Where(a => a.EntityType == "Finding" && a.EntityId == id.ToString() && a.Status == "Pending")
                    .ToList();
                foreach (var pendingRec in pending)
                {
                    pendingRec.Status = "Cancelled";
                    pendingRec.ActionTaken = "Cancel";
                    pendingRec.ActionDate = DateTime.UtcNow;
                    pendingRec.Comment = "Entity cancelled by submitter.";
                    await _approvalRepo.UpdateAsync(pendingRec);
                }
            }

            // Log status change
            await _statusChangeLogRepo.InsertAsync(new Entities.StatusChangeLog
            {
                EntityType = "Finding",
                EntityId = id.ToString(),
                FromStatus = fromStatus,
                ToStatus = transition.To,
                Action = input.Action,
                Comment = input.ActionData != null && input.ActionData.ContainsKey("comment") ? input.ActionData["comment"] : null,
                ChangedByUserId = AbpSession.UserId
            });

            var result = MapToEntityDto(entity);

            // Trigger flow: on-status-change (always)
            await _flowEngine.TriggerAsync("on-field-change", "Finding", result);

            // Trigger named flow events
            if (input.Action == "Approve")
                await _flowEngine.TriggerAsync("submit-for-approval", "Finding", result);
            return result;
        }

        private void ValidateStatusTransition(FindingStatus from, FindingStatus to)
        {
            var allowed = new (string From, string To)[]
            {
                ("Open", "ActionPlanned"),
                ("ActionPlanned", "InProgress"),
                ("InProgress", "PendingClosure"),
                ("PendingClosure", "Closed"),
                ("PendingClosure", "InProgress")
            };

            var isValid = allowed.Any(t =>
                (t.From == "*" || t.From == from.ToString()) &&
                t.To == to.ToString());

            if (!isValid)
                throw new Abp.UI.UserFriendlyException($"Invalid status transition from {from} to {to}");
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.Finding_Read)]
        public List<GroupCountDto> GetGroupedCount(FindingGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Status", "AuditId", "FindingLevelId", "DepartmentId", "RequirementReferenceId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "Status":
                    return query
                        .GroupBy(x => x.Status)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                case "AuditId":
                    return query
                        .GroupBy(x => new { Key = x.AuditId, Label = x.Audit == null ? null : x.Audit.AuditNumber })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key == null ? null : g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "FindingLevelId":
                    return query
                        .GroupBy(x => new { Key = x.FindingLevelId, Label = x.FindingLevel == null ? null : x.FindingLevel.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key == null ? null : g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "DepartmentId":
                    return query
                        .GroupBy(x => new { Key = x.DepartmentId, Label = x.Department == null ? null : x.Department.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key == null ? null : g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "RequirementReferenceId":
                    return query
                        .GroupBy(x => new { Key = x.RequirementReferenceId, Label = x.RequirementReference == null ? null : x.RequirementReference.Title })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key == null ? null : g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.Finding_Read)]
        public decimal? GetStats(FindingStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "DetectedDate", "DueDate", "ClosedDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    case "DetectedDate|DueDate":
                        return (decimal?)query
                            .Where(x => x.DetectedDate != null && x.DueDate != null)
                            .Select(x => EF.Functions.DateDiffDay(x.DetectedDate.Value, x.DueDate.Value))
                            .DefaultIfEmpty()
                            .Average();
                    case "DetectedDate|ClosedDate":
                        return (decimal?)query
                            .Where(x => x.DetectedDate != null && x.ClosedDate != null)
                            .Select(x => EF.Functions.DateDiffDay(x.DetectedDate.Value, x.ClosedDate.Value))
                            .DefaultIfEmpty()
                            .Average();
                    case "DueDate|DetectedDate":
                        return (decimal?)query
                            .Where(x => x.DueDate != null && x.DetectedDate != null)
                            .Select(x => EF.Functions.DateDiffDay(x.DueDate.Value, x.DetectedDate.Value))
                            .DefaultIfEmpty()
                            .Average();
                    case "DueDate|ClosedDate":
                        return (decimal?)query
                            .Where(x => x.DueDate != null && x.ClosedDate != null)
                            .Select(x => EF.Functions.DateDiffDay(x.DueDate.Value, x.ClosedDate.Value))
                            .DefaultIfEmpty()
                            .Average();
                    case "ClosedDate|DetectedDate":
                        return (decimal?)query
                            .Where(x => x.ClosedDate != null && x.DetectedDate != null)
                            .Select(x => EF.Functions.DateDiffDay(x.ClosedDate.Value, x.DetectedDate.Value))
                            .DefaultIfEmpty()
                            .Average();
                    case "ClosedDate|DueDate":
                        return (decimal?)query
                            .Where(x => x.ClosedDate != null && x.DueDate != null)
                            .Select(x => EF.Functions.DateDiffDay(x.ClosedDate.Value, x.DueDate.Value))
                            .DefaultIfEmpty()
                            .Average();
                    default: return null;
                }
            }

            var allowedNumeric = new string[0];
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.Finding_Read)]
        public async Task<FindingReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.CorrectiveActions)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new FindingReportDto
            {
                Data = ObjectMapper.Map<FindingDto>(root),
                CorrectiveActions = ObjectMapper.Map<List<CorrectiveActionDto>>(
                    root.CorrectiveActions == null ? new List<CorrectiveAction>() : root.CorrectiveActions.ToList()),
                ApprovalHistory = ObjectMapper.Map<List<ApprovalRecordDto>>(
                    _approvalRepo.GetAll()
                        .Where(a => a.EntityName == "Finding" && a.EntityId == Convert.ToInt64(id))
                        .OrderBy(a => a.Id).ToList()),
            };
        }

    }
}
