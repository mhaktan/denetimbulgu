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
using DenetimBulgu.Audits.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.AuditTeamMembers.Dto;
using DenetimBulgu.Findings.Dto;
using DenetimBulgu.Approvals.Dto;
using DenetimBulgu.Authorization;
using DenetimBulgu.Flows;

namespace DenetimBulgu.Audits
{
    public class AuditAppService : AsyncCrudAppService<
        Audit,
        AuditDto,
        long,
        PagedAuditResultRequestDto,
        CreateAuditDto,
        AuditDto>,
        IAuditAppService
    {
        private readonly IRepository<StatusChangeLog, long> _statusChangeLogRepo;
        private readonly IRepository<ApprovalRecord, Guid> _approvalRepo;
        private readonly IFlowEngine _flowEngine;

        public AuditAppService(IRepository<Audit, long> repository, IFlowEngine flowEngine, IRepository<StatusChangeLog, long> statusChangeLogRepo, IRepository<ApprovalRecord, Guid> approvalRepo)
            : base(repository)
        {
            _flowEngine = flowEngine;
            _statusChangeLogRepo = statusChangeLogRepo;
            _approvalRepo = approvalRepo;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Audit_Read;
            GetAllPermissionName = PermissionNames.Audit_Read;
            CreatePermissionName = PermissionNames.Audit_Create;
            UpdatePermissionName = PermissionNames.Audit_Update;
            DeletePermissionName = PermissionNames.Audit_Delete;
        }

        protected override IQueryable<Audit> CreateFilteredQuery(PagedAuditResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.AuditNumber != null && x.AuditNumber.Contains(input.Keyword)))
                .WhereIf(!input.AuditNumber.IsNullOrWhiteSpace(), x => x.AuditNumber != null && x.AuditNumber.Contains(input.AuditNumber))
                .WhereIf(input.PlannedDate.HasValue, x => x.PlannedDate == input.PlannedDate.Value)
                .WhereIf(input.ActualDate.HasValue, x => x.ActualDate == input.ActualDate.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (AuditStatus)input.Status.Value)
                .WhereIf(input.PlannedDateFrom.HasValue, x => x.PlannedDate >= input.PlannedDateFrom.Value)
                .WhereIf(input.PlannedDateTo.HasValue, x => x.PlannedDate <= input.PlannedDateTo.Value)
                .WhereIf(input.ActualDateFrom.HasValue, x => x.ActualDate >= input.ActualDateFrom.Value)
                .WhereIf(input.ActualDateTo.HasValue, x => x.ActualDate <= input.ActualDateTo.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (AuditStatus)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (AuditStatus)input.StatusNot.Value)
                .WhereIf(input.AuditPlanId.HasValue, x => x.AuditPlanId == input.AuditPlanId.Value)
                .WhereIf(input.AuditTypeId.HasValue, x => x.AuditTypeId == input.AuditTypeId.Value)
                .WhereIf(input.DepartmentId.HasValue, x => x.DepartmentId == input.DepartmentId.Value)
                .WhereIf(input.EmployeeId.HasValue, x => x.EmployeeId == input.EmployeeId.Value);
        }

        public override async Task<AuditDto> CreateAsync(CreateAuditDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Audit", result);

            // Frontend creates records with status pre-set without going through ChangeStatusAsync,
            // so mirror on-field-change here whenever the initial status isn't the default. Otherwise
            // status-driven flows (e.g. approval) never fire on plain Create.
            if (result.Status != (int)AuditStatus.Planned)
                await _flowEngine.TriggerAsync("on-field-change", "Audit", result);
            return result;
        }

        public override async Task<AuditDto> UpdateAsync(AuditDto input)
        {
            // State machine: validate status transition + log
            var existing = await Repository.GetAsync(input.Id);
            var statusChanged = (int)existing.Status != input.Status;
            if (statusChanged)
            {
                var fromStatus = existing.Status.ToString();
                var toStatus = ((AuditStatus)input.Status).ToString();
                ValidateStatusTransition(existing.Status, (AuditStatus)input.Status);

                // Log status change
                await _statusChangeLogRepo.InsertAsync(new StatusChangeLog
                {
                    EntityType = "Audit",
                    EntityId = input.Id.ToString(),
                    FromStatus = fromStatus,
                    ToStatus = toStatus,
                    Action = "Update",
                    ChangedByUserId = AbpSession.UserId
                });
            }

            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Audit", result);

            // Frontend updates status via plain UpdateAsync (not ChangeStatusAsync) — fire
            // on-field-change so status-driven flows pick up the transition.
            if (statusChanged)
                await _flowEngine.TriggerAsync("on-field-change", "Audit", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Audit", new { Id = input.Id });
        }

        // Onay adimini tamamlayan rol genelde Update degil ChangeStatus yetkisine sahip olur;
        // RequireAllPermissions=false ile ikisinden biri yeterli (geriye donuk uyumlu).
        [Abp.Authorization.AbpAuthorize(PermissionNames.Audit_ChangeStatus, PermissionNames.Audit_Update, RequireAllPermissions = false)]
        public async Task<AuditDto> ChangeStatusAsync(long id, ChangeStatusInput input)
        {
            var entity = await Repository.GetAsync(id);
            var currentStatus = entity.Status.ToString();

            // Find valid transition
            var transitions = new (string From, string To, string Action, bool Readonly)[]
            {
            ("Planned", "InProgress", "Start", false),
            ("InProgress", "Completed", "Complete", false)
            };

            var transition = transitions.FirstOrDefault(t =>
                (t.From == "*" || t.From == currentStatus) && t.Action == input.Action);

            if (transition == default)
                throw new Abp.UI.UserFriendlyException($"Invalid action '{input.Action}' from status '{currentStatus}'");

            // Validate required fields per transition
            // No required fields for any transition
            // Bu gecisler icin bagli kayit on kosulu yok

            var fromStatus = currentStatus;

            // Apply new status
            entity.Status = (AuditStatus)Enum.Parse(typeof(AuditStatus), transition.To);
            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Cancel pending ApprovalRecords when the entity is cancelled — otherwise the records
            // sit forever in approvers' inboxes pointing to a cancelled request.
            if (input.Action == "Cancel")
            {
                var pending = _approvalRepo.GetAll()
                    .Where(a => a.EntityType == "Audit" && a.EntityId == id.ToString() && a.Status == "Pending")
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
                EntityType = "Audit",
                EntityId = id.ToString(),
                FromStatus = fromStatus,
                ToStatus = transition.To,
                Action = input.Action,
                Comment = input.ActionData != null && input.ActionData.ContainsKey("comment") ? input.ActionData["comment"] : null,
                ChangedByUserId = AbpSession.UserId
            });

            var result = MapToEntityDto(entity);

            // Trigger flow: on-status-change (always)
            await _flowEngine.TriggerAsync("on-field-change", "Audit", result);

            return result;
        }

        private void ValidateStatusTransition(AuditStatus from, AuditStatus to)
        {
            var allowed = new (string From, string To)[]
            {
                ("Planned", "InProgress"),
                ("InProgress", "Completed")
            };

            var isValid = allowed.Any(t =>
                (t.From == "*" || t.From == from.ToString()) &&
                t.To == to.ToString());

            if (!isValid)
                throw new Abp.UI.UserFriendlyException($"Invalid status transition from {from} to {to}");
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.Audit_Read)]
        public List<GroupCountDto> GetGroupedCount(AuditGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Status", "AuditPlanId", "AuditTypeId", "DepartmentId", "EmployeeId" };
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
                case "AuditPlanId":
                    return query
                        .GroupBy(x => new { Key = x.AuditPlanId, Label = x.AuditPlan == null ? null : x.AuditPlan.ScopeDescription })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key == null ? null : g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "AuditTypeId":
                    return query
                        .GroupBy(x => new { Key = x.AuditTypeId, Label = x.AuditType == null ? null : x.AuditType.Name })
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
                case "EmployeeId":
                    return query
                        .GroupBy(x => new { Key = x.EmployeeId, Label = x.Employee == null ? null : x.Employee.FullName })
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

        [Abp.Authorization.AbpAuthorize(PermissionNames.Audit_Read)]
        public decimal? GetStats(AuditStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "PlannedDate", "ActualDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    case "PlannedDate|ActualDate":
                        return (decimal?)query
                            .Where(x => x.PlannedDate != null && x.ActualDate != null)
                            .Select(x => EF.Functions.DateDiffDay(x.PlannedDate.Value, x.ActualDate.Value))
                            .DefaultIfEmpty()
                            .Average();
                    case "ActualDate|PlannedDate":
                        return (decimal?)query
                            .Where(x => x.ActualDate != null && x.PlannedDate != null)
                            .Select(x => EF.Functions.DateDiffDay(x.ActualDate.Value, x.PlannedDate.Value))
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
        [Abp.Authorization.AbpAuthorize(PermissionNames.Audit_Read)]
        public async Task<AuditReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.AuditTeamMembers)
                .Include(x => x.Findings)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new AuditReportDto
            {
                Data = ObjectMapper.Map<AuditDto>(root),
                AuditTeamMembers = ObjectMapper.Map<List<AuditTeamMemberDto>>(
                    root.AuditTeamMembers == null ? new List<AuditTeamMember>() : root.AuditTeamMembers.ToList()),
                Findings = ObjectMapper.Map<List<FindingDto>>(
                    root.Findings == null ? new List<Finding>() : root.Findings.ToList()),
                ApprovalHistory = ObjectMapper.Map<List<ApprovalRecordDto>>(
                    _approvalRepo.GetAll()
                        .Where(a => a.EntityName == "Audit" && a.EntityId == Convert.ToInt64(id))
                        .OrderBy(a => a.Id).ToList()),
            };
        }

    }
}
