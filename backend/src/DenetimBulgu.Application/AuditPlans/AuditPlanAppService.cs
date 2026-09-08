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
using DenetimBulgu.AuditPlans.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.Audits.Dto;
using DenetimBulgu.Approvals.Dto;
using DenetimBulgu.Authorization;
using DenetimBulgu.Flows;

namespace DenetimBulgu.AuditPlans
{
    public class AuditPlanAppService : AsyncCrudAppService<
        AuditPlan,
        AuditPlanDto,
        long,
        PagedAuditPlanResultRequestDto,
        CreateAuditPlanDto,
        AuditPlanDto>,
        IAuditPlanAppService
    {
        private readonly IRepository<StatusChangeLog, long> _statusChangeLogRepo;
        private readonly IRepository<ApprovalRecord, Guid> _approvalRepo;
        private readonly IFlowEngine _flowEngine;

        public AuditPlanAppService(IRepository<AuditPlan, long> repository, IFlowEngine flowEngine, IRepository<StatusChangeLog, long> statusChangeLogRepo, IRepository<ApprovalRecord, Guid> approvalRepo)
            : base(repository)
        {
            _flowEngine = flowEngine;
            _statusChangeLogRepo = statusChangeLogRepo;
            _approvalRepo = approvalRepo;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.AuditPlan_Read;
            GetAllPermissionName = PermissionNames.AuditPlan_Read;
            CreatePermissionName = PermissionNames.AuditPlan_Create;
            UpdatePermissionName = PermissionNames.AuditPlan_Update;
            DeletePermissionName = PermissionNames.AuditPlan_Delete;
        }

        protected override IQueryable<AuditPlan> CreateFilteredQuery(PagedAuditPlanResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Period != null && x.Period.Contains(input.Keyword)) ||
                    (x.ScopeDescription != null && x.ScopeDescription.Contains(input.Keyword)))
                .WhereIf(!input.Period.IsNullOrWhiteSpace(), x => x.Period != null && x.Period.Contains(input.Period))
                .WhereIf(!input.ScopeDescription.IsNullOrWhiteSpace(), x => x.ScopeDescription != null && x.ScopeDescription.Contains(input.ScopeDescription))
                .WhereIf(input.Year.HasValue, x => x.Year == input.Year.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (AuditPlanStatus)input.Status.Value)
                .WhereIf(input.QualityManagerApproverId.HasValue, x => x.QualityManagerApproverId == input.QualityManagerApproverId.Value)
                .WhereIf(input.YearFrom.HasValue, x => x.Year >= input.YearFrom.Value)
                .WhereIf(input.YearTo.HasValue, x => x.Year <= input.YearTo.Value)
                .WhereIf(input.QualityManagerApproverIdFrom.HasValue, x => x.QualityManagerApproverId >= input.QualityManagerApproverIdFrom.Value)
                .WhereIf(input.QualityManagerApproverIdTo.HasValue, x => x.QualityManagerApproverId <= input.QualityManagerApproverIdTo.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (AuditPlanStatus)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (AuditPlanStatus)input.StatusNot.Value);
        }

        public override async Task<AuditPlanDto> CreateAsync(CreateAuditPlanDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "AuditPlan", result);

            // Frontend creates records with status pre-set without going through ChangeStatusAsync,
            // so mirror on-field-change here whenever the initial status isn't the default. Otherwise
            // status-driven flows (e.g. approval) never fire on plain Create.
            if (result.Status != (int)AuditPlanStatus.Draft)
                await _flowEngine.TriggerAsync("on-field-change", "AuditPlan", result);
            return result;
        }

        public override async Task<AuditPlanDto> UpdateAsync(AuditPlanDto input)
        {
            // State machine: validate status transition + log
            var existing = await Repository.GetAsync(input.Id);
            var statusChanged = (int)existing.Status != input.Status;
            if (statusChanged)
            {
                var fromStatus = existing.Status.ToString();
                var toStatus = ((AuditPlanStatus)input.Status).ToString();
                ValidateStatusTransition(existing.Status, (AuditPlanStatus)input.Status);

                // Log status change
                await _statusChangeLogRepo.InsertAsync(new StatusChangeLog
                {
                    EntityType = "AuditPlan",
                    EntityId = input.Id.ToString(),
                    FromStatus = fromStatus,
                    ToStatus = toStatus,
                    Action = "Update",
                    ChangedByUserId = AbpSession.UserId
                });
            }

            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "AuditPlan", result);

            // Frontend updates status via plain UpdateAsync (not ChangeStatusAsync) — fire
            // on-field-change so status-driven flows pick up the transition.
            if (statusChanged)
                await _flowEngine.TriggerAsync("on-field-change", "AuditPlan", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "AuditPlan", new { Id = input.Id });
        }

        // Onay adimini tamamlayan rol genelde Update degil ChangeStatus yetkisine sahip olur;
        // RequireAllPermissions=false ile ikisinden biri yeterli (geriye donuk uyumlu).
        [Abp.Authorization.AbpAuthorize(PermissionNames.AuditPlan_ChangeStatus, PermissionNames.AuditPlan_Update, RequireAllPermissions = false)]
        public async Task<AuditPlanDto> ChangeStatusAsync(long id, ChangeStatusInput input)
        {
            var entity = await Repository.GetAsync(id);
            var currentStatus = entity.Status.ToString();

            // Find valid transition
            var transitions = new (string From, string To, string Action, bool Readonly)[]
            {
            ("Draft", "Approved", "Approve", false)
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
            entity.Status = (AuditPlanStatus)Enum.Parse(typeof(AuditPlanStatus), transition.To);
            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Cancel pending ApprovalRecords when the entity is cancelled — otherwise the records
            // sit forever in approvers' inboxes pointing to a cancelled request.
            if (input.Action == "Cancel")
            {
                var pending = _approvalRepo.GetAll()
                    .Where(a => a.EntityType == "AuditPlan" && a.EntityId == id.ToString() && a.Status == "Pending")
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
                EntityType = "AuditPlan",
                EntityId = id.ToString(),
                FromStatus = fromStatus,
                ToStatus = transition.To,
                Action = input.Action,
                Comment = input.ActionData != null && input.ActionData.ContainsKey("comment") ? input.ActionData["comment"] : null,
                ChangedByUserId = AbpSession.UserId
            });

            var result = MapToEntityDto(entity);

            // Trigger flow: on-status-change (always)
            await _flowEngine.TriggerAsync("on-field-change", "AuditPlan", result);

            return result;
        }

        private void ValidateStatusTransition(AuditPlanStatus from, AuditPlanStatus to)
        {
            var allowed = new (string From, string To)[]
            {
                ("Draft", "Approved")
            };

            var isValid = allowed.Any(t =>
                (t.From == "*" || t.From == from.ToString()) &&
                t.To == to.ToString());

            if (!isValid)
                throw new Abp.UI.UserFriendlyException($"Invalid status transition from {from} to {to}");
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.AuditPlan_Read)]
        public List<GroupCountDto> GetGroupedCount(AuditPlanGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Status" };
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
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.AuditPlan_Read)]
        public decimal? GetStats(AuditPlanStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new string[0];
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "Year", "QualityManagerApproverId" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "Year": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Year)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Year)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Year)
                            : query.Average(x => (decimal?)x.Year);
                        case "QualityManagerApproverId": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.QualityManagerApproverId)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.QualityManagerApproverId)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.QualityManagerApproverId)
                            : query.Average(x => (decimal?)x.QualityManagerApproverId);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.AuditPlan_Read)]
        public async Task<AuditPlanReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.Audits)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new AuditPlanReportDto
            {
                Data = ObjectMapper.Map<AuditPlanDto>(root),
                Audits = ObjectMapper.Map<List<AuditDto>>(
                    root.Audits == null ? new List<Audit>() : root.Audits.ToList()),
                ApprovalHistory = ObjectMapper.Map<List<ApprovalRecordDto>>(
                    _approvalRepo.GetAll()
                        .Where(a => a.EntityName == "AuditPlan" && a.EntityId == Convert.ToInt64(id))
                        .OrderBy(a => a.Id).ToList()),
            };
        }

    }
}
