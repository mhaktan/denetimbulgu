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
using DenetimBulgu.CorrectiveActions.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.StateMachine.Dto;
using DenetimBulgu.ActionProgressNotes.Dto;
using DenetimBulgu.Evidences.Dto;
using DenetimBulgu.Approvals.Dto;
using DenetimBulgu.Authorization;
using DenetimBulgu.Flows;

namespace DenetimBulgu.CorrectiveActions
{
    public class CorrectiveActionAppService : AsyncCrudAppService<
        CorrectiveAction,
        CorrectiveActionDto,
        long,
        PagedCorrectiveActionResultRequestDto,
        CreateCorrectiveActionDto,
        CorrectiveActionDto>,
        ICorrectiveActionAppService
    {
        private readonly IRepository<StatusChangeLog, long> _statusChangeLogRepo;
        private readonly IRepository<ApprovalRecord, Guid> _approvalRepo;
        private readonly IFlowEngine _flowEngine;

        public CorrectiveActionAppService(IRepository<CorrectiveAction, long> repository, IFlowEngine flowEngine, IRepository<StatusChangeLog, long> statusChangeLogRepo, IRepository<ApprovalRecord, Guid> approvalRepo)
            : base(repository)
        {
            _flowEngine = flowEngine;
            _statusChangeLogRepo = statusChangeLogRepo;
            _approvalRepo = approvalRepo;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.CorrectiveAction_Read;
            GetAllPermissionName = PermissionNames.CorrectiveAction_Read;
            CreatePermissionName = PermissionNames.CorrectiveAction_Create;
            UpdatePermissionName = PermissionNames.CorrectiveAction_Update;
            DeletePermissionName = PermissionNames.CorrectiveAction_Delete;
        }

        protected override IQueryable<CorrectiveAction> CreateFilteredQuery(PagedCorrectiveActionResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.RootCauseAnalysis != null && x.RootCauseAnalysis.Contains(input.Keyword)) ||
                    (x.ActionDescription != null && x.ActionDescription.Contains(input.Keyword)))
                .WhereIf(!input.RootCauseAnalysis.IsNullOrWhiteSpace(), x => x.RootCauseAnalysis != null && x.RootCauseAnalysis.Contains(input.RootCauseAnalysis))
                .WhereIf(!input.ActionDescription.IsNullOrWhiteSpace(), x => x.ActionDescription != null && x.ActionDescription.Contains(input.ActionDescription))
                .WhereIf(input.TargetDate.HasValue, x => x.TargetDate == input.TargetDate.Value)
                .WhereIf(input.ActualClosureDate.HasValue, x => x.ActualClosureDate == input.ActualClosureDate.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (CorrectiveActionStatus)input.Status.Value)
                .WhereIf(input.TargetDateFrom.HasValue, x => x.TargetDate >= input.TargetDateFrom.Value)
                .WhereIf(input.TargetDateTo.HasValue, x => x.TargetDate <= input.TargetDateTo.Value)
                .WhereIf(input.ActualClosureDateFrom.HasValue, x => x.ActualClosureDate >= input.ActualClosureDateFrom.Value)
                .WhereIf(input.ActualClosureDateTo.HasValue, x => x.ActualClosureDate <= input.ActualClosureDateTo.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (CorrectiveActionStatus)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (CorrectiveActionStatus)input.StatusNot.Value)
                .WhereIf(input.FindingId.HasValue, x => x.FindingId == input.FindingId.Value)
                .WhereIf(input.EmployeeId.HasValue, x => x.EmployeeId == input.EmployeeId.Value);
        }

        public override async Task<CorrectiveActionDto> CreateAsync(CreateCorrectiveActionDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "CorrectiveAction", result);

            // Frontend creates records with status pre-set without going through ChangeStatusAsync,
            // so mirror on-field-change here whenever the initial status isn't the default. Otherwise
            // status-driven flows (e.g. approval) never fire on plain Create.
            if (result.Status != (int)CorrectiveActionStatus.Open)
                await _flowEngine.TriggerAsync("on-field-change", "CorrectiveAction", result);
            return result;
        }

        public override async Task<CorrectiveActionDto> UpdateAsync(CorrectiveActionDto input)
        {
            // State machine: validate status transition + log
            var existing = await Repository.GetAsync(input.Id);
            var statusChanged = (int)existing.Status != input.Status;
            if (statusChanged)
            {
                var fromStatus = existing.Status.ToString();
                var toStatus = ((CorrectiveActionStatus)input.Status).ToString();
                ValidateStatusTransition(existing.Status, (CorrectiveActionStatus)input.Status);

                // Log status change
                await _statusChangeLogRepo.InsertAsync(new StatusChangeLog
                {
                    EntityType = "CorrectiveAction",
                    EntityId = input.Id.ToString(),
                    FromStatus = fromStatus,
                    ToStatus = toStatus,
                    Action = "Update",
                    ChangedByUserId = AbpSession.UserId
                });
            }

            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "CorrectiveAction", result);

            // Frontend updates status via plain UpdateAsync (not ChangeStatusAsync) — fire
            // on-field-change so status-driven flows pick up the transition.
            if (statusChanged)
                await _flowEngine.TriggerAsync("on-field-change", "CorrectiveAction", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "CorrectiveAction", new { Id = input.Id });
        }

        // Onay adimini tamamlayan rol genelde Update degil ChangeStatus yetkisine sahip olur;
        // RequireAllPermissions=false ile ikisinden biri yeterli (geriye donuk uyumlu).
        [Abp.Authorization.AbpAuthorize(PermissionNames.CorrectiveAction_ChangeStatus, PermissionNames.CorrectiveAction_Update, RequireAllPermissions = false)]
        public async Task<CorrectiveActionDto> ChangeStatusAsync(long id, ChangeStatusInput input)
        {
            var entity = await Repository.GetAsync(id);
            var currentStatus = entity.Status.ToString();

            // Find valid transition
            var transitions = new (string From, string To, string Action, bool Readonly)[]
            {
            ("Open", "InProgress", "Start", false),
            ("InProgress", "Completed", "Complete", false),
            ("*", "Cancelled", "Cancel", true)
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
            entity.Status = (CorrectiveActionStatus)Enum.Parse(typeof(CorrectiveActionStatus), transition.To);
            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Cancel pending ApprovalRecords when the entity is cancelled — otherwise the records
            // sit forever in approvers' inboxes pointing to a cancelled request.
            if (input.Action == "Cancel")
            {
                var pending = _approvalRepo.GetAll()
                    .Where(a => a.EntityType == "CorrectiveAction" && a.EntityId == id.ToString() && a.Status == "Pending")
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
                EntityType = "CorrectiveAction",
                EntityId = id.ToString(),
                FromStatus = fromStatus,
                ToStatus = transition.To,
                Action = input.Action,
                Comment = input.ActionData != null && input.ActionData.ContainsKey("comment") ? input.ActionData["comment"] : null,
                ChangedByUserId = AbpSession.UserId
            });

            var result = MapToEntityDto(entity);

            // Trigger flow: on-status-change (always)
            await _flowEngine.TriggerAsync("on-field-change", "CorrectiveAction", result);

            return result;
        }

        private void ValidateStatusTransition(CorrectiveActionStatus from, CorrectiveActionStatus to)
        {
            var allowed = new (string From, string To)[]
            {
                ("Open", "InProgress"),
                ("InProgress", "Completed"),
                ("*", "Cancelled")
            };

            var isValid = allowed.Any(t =>
                (t.From == "*" || t.From == from.ToString()) &&
                t.To == to.ToString());

            if (!isValid)
                throw new Abp.UI.UserFriendlyException($"Invalid status transition from {from} to {to}");
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.CorrectiveAction_Read)]
        public List<GroupCountDto> GetGroupedCount(CorrectiveActionGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Status", "FindingId", "EmployeeId" };
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
                case "FindingId":
                    return query
                        .GroupBy(x => new { Key = x.FindingId, Label = x.Finding == null ? null : x.Finding.Title })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "EmployeeId":
                    return query
                        .GroupBy(x => new { Key = x.EmployeeId, Label = x.Employee == null ? null : x.Employee.FullName })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.CorrectiveAction_Read)]
        public decimal? GetStats(CorrectiveActionStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "TargetDate", "ActualClosureDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    case "TargetDate|ActualClosureDate":
                    {
                        var pairsTargetDateActualClosureDate = query
                            .Where(x => x.ActualClosureDate != null)
                            .Select(x => new { A = x.TargetDate, B = x.ActualClosureDate.Value })
                            .ToList();
                        if (pairsTargetDateActualClosureDate.Count == 0) return null;
                        return (decimal)pairsTargetDateActualClosureDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    case "ActualClosureDate|TargetDate":
                    {
                        var pairsActualClosureDateTargetDate = query
                            .Where(x => x.ActualClosureDate != null)
                            .Select(x => new { A = x.ActualClosureDate.Value, B = x.TargetDate })
                            .ToList();
                        if (pairsActualClosureDateTargetDate.Count == 0) return null;
                        return (decimal)pairsActualClosureDateTargetDate.Average(p => (p.B - p.A).TotalDays);
                    }
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
        [Abp.Authorization.AbpAuthorize(PermissionNames.CorrectiveAction_Read)]
        public async Task<CorrectiveActionReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.ActionProgressNotes)
                .Include(x => x.Evidences)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new CorrectiveActionReportDto
            {
                Data = ObjectMapper.Map<CorrectiveActionDto>(root),
                ActionProgressNotes = ObjectMapper.Map<List<ActionProgressNoteDto>>(
                    root.ActionProgressNotes == null ? new List<ActionProgressNote>() : root.ActionProgressNotes.ToList()),
                Evidences = ObjectMapper.Map<List<EvidenceDto>>(
                    root.Evidences == null ? new List<Evidence>() : root.Evidences.ToList()),
                // ApprovalRecord alan adlari: EntityType (isim) ve EntityId (STRING).
                // Once EntityName/long varsayilmisti — CS1061 + CS0019 veriyordu.
                ApprovalHistory = ObjectMapper.Map<List<ApprovalRecordDto>>(
                    _approvalRepo.GetAll()
                        .Where(a => a.EntityType == "CorrectiveAction" && a.EntityId == id.ToString())
                        .OrderBy(a => a.StepIndex).ToList()),
            };
        }

    }
}
