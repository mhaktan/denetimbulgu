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
using DenetimBulgu.Evidences.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.Authorization;
using DenetimBulgu.Flows;

namespace DenetimBulgu.Evidences
{
    public class EvidenceAppService : AsyncCrudAppService<
        Evidence,
        EvidenceDto,
        long,
        PagedEvidenceResultRequestDto,
        CreateEvidenceDto,
        EvidenceDto>,
        IEvidenceAppService
    {
        private readonly IFlowEngine _flowEngine;

        public EvidenceAppService(IRepository<Evidence, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Evidence_Read;
            GetAllPermissionName = PermissionNames.Evidence_Read;
            CreatePermissionName = PermissionNames.Evidence_Create;
            UpdatePermissionName = PermissionNames.Evidence_Update;
            DeletePermissionName = PermissionNames.Evidence_Delete;
        }

        protected override IQueryable<Evidence> CreateFilteredQuery(PagedEvidenceResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)) ||
                    (x.FileName != null && x.FileName.Contains(input.Keyword)))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(!input.FileName.IsNullOrWhiteSpace(), x => x.FileName != null && x.FileName.Contains(input.FileName))
                .WhereIf(input.UploadDate.HasValue, x => x.UploadDate == input.UploadDate.Value)
                .WhereIf(input.UploadDateFrom.HasValue, x => x.UploadDate >= input.UploadDateFrom.Value)
                .WhereIf(input.UploadDateTo.HasValue, x => x.UploadDate <= input.UploadDateTo.Value)
                .WhereIf(input.CorrectiveActionId.HasValue, x => x.CorrectiveActionId == input.CorrectiveActionId.Value)
                .WhereIf(input.EmployeeId.HasValue, x => x.EmployeeId == input.EmployeeId.Value);
        }

        public override async Task<EvidenceDto> CreateAsync(CreateEvidenceDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Evidence", result);
            return result;
        }

        public override async Task<EvidenceDto> UpdateAsync(EvidenceDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Evidence", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Evidence", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.Evidence_Read)]
        public List<GroupCountDto> GetGroupedCount(EvidenceGroupedCountInput input)
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

    }
}
