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
using DenetimBulgu.ActionProgressNotes.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.Authorization;
using DenetimBulgu.Flows;

namespace DenetimBulgu.ActionProgressNotes
{
    public class ActionProgressNoteAppService : AsyncCrudAppService<
        ActionProgressNote,
        ActionProgressNoteDto,
        long,
        PagedActionProgressNoteResultRequestDto,
        CreateActionProgressNoteDto,
        ActionProgressNoteDto>,
        IActionProgressNoteAppService
    {
        private readonly IFlowEngine _flowEngine;

        public ActionProgressNoteAppService(IRepository<ActionProgressNote, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.ActionProgressNote_Read;
            GetAllPermissionName = PermissionNames.ActionProgressNote_Read;
            CreatePermissionName = PermissionNames.ActionProgressNote_Create;
            UpdatePermissionName = PermissionNames.ActionProgressNote_Update;
            DeletePermissionName = PermissionNames.ActionProgressNote_Delete;
        }

        protected override IQueryable<ActionProgressNote> CreateFilteredQuery(PagedActionProgressNoteResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Note != null && x.Note.Contains(input.Keyword)))
                .WhereIf(!input.Note.IsNullOrWhiteSpace(), x => x.Note != null && x.Note.Contains(input.Note))
                .WhereIf(input.NoteDate.HasValue, x => x.NoteDate == input.NoteDate.Value)
                .WhereIf(input.NoteDateFrom.HasValue, x => x.NoteDate >= input.NoteDateFrom.Value)
                .WhereIf(input.NoteDateTo.HasValue, x => x.NoteDate <= input.NoteDateTo.Value)
                .WhereIf(input.CorrectiveActionId.HasValue, x => x.CorrectiveActionId == input.CorrectiveActionId.Value)
                .WhereIf(input.EmployeeId.HasValue, x => x.EmployeeId == input.EmployeeId.Value);
        }

        public override async Task<ActionProgressNoteDto> CreateAsync(CreateActionProgressNoteDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "ActionProgressNote", result);
            return result;
        }

        public override async Task<ActionProgressNoteDto> UpdateAsync(ActionProgressNoteDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "ActionProgressNote", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "ActionProgressNote", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.ActionProgressNote_Read)]
        public List<GroupCountDto> GetGroupedCount(ActionProgressNoteGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "CorrectiveActionId", "EmployeeId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "CorrectiveActionId":
                    return query
                        .GroupBy(x => new { Key = x.CorrectiveActionId, Label = x.CorrectiveAction == null ? null : x.CorrectiveAction.ActionDescription })
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

    }
}
