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
using DenetimBulgu.RequirementReferences.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.Findings.Dto;
using DenetimBulgu.Authorization;
using DenetimBulgu.Flows;

namespace DenetimBulgu.RequirementReferences
{
    public class RequirementReferenceAppService : AsyncCrudAppService<
        RequirementReference,
        RequirementReferenceDto,
        long,
        PagedRequirementReferenceResultRequestDto,
        CreateRequirementReferenceDto,
        RequirementReferenceDto>,
        IRequirementReferenceAppService
    {
        private readonly IFlowEngine _flowEngine;

        public RequirementReferenceAppService(IRepository<RequirementReference, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.RequirementReference_Read;
            GetAllPermissionName = PermissionNames.RequirementReference_Read;
            CreatePermissionName = PermissionNames.RequirementReference_Create;
            UpdatePermissionName = PermissionNames.RequirementReference_Update;
            DeletePermissionName = PermissionNames.RequirementReference_Delete;
        }

        protected override IQueryable<RequirementReference> CreateFilteredQuery(PagedRequirementReferenceResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Code != null && x.Code.Contains(input.Keyword)) ||
                    (x.Title != null && x.Title.Contains(input.Keyword)) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)))
                .WhereIf(!input.Code.IsNullOrWhiteSpace(), x => x.Code != null && x.Code.Contains(input.Code))
                .WhereIf(!input.Title.IsNullOrWhiteSpace(), x => x.Title != null && x.Title.Contains(input.Title))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(input.SourceStandard.HasValue, x => x.SourceStandard == (RequirementReferenceSourceStandard)input.SourceStandard.Value)
                .WhereIf(!input.SourceStandardIn.IsNullOrWhiteSpace(), x => input.SourceStandardIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (RequirementReferenceSourceStandard)int.Parse(v.Trim()))
                    .Contains(x.SourceStandard))
                .WhereIf(input.SourceStandardNot.HasValue, x => x.SourceStandard != (RequirementReferenceSourceStandard)input.SourceStandardNot.Value);
        }

        public override async Task<RequirementReferenceDto> CreateAsync(CreateRequirementReferenceDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "RequirementReference", result);
            return result;
        }

        public override async Task<RequirementReferenceDto> UpdateAsync(RequirementReferenceDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "RequirementReference", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "RequirementReference", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.RequirementReference_Read)]
        public List<GroupCountDto> GetGroupedCount(RequirementReferenceGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "SourceStandard" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "SourceStandard":
                    return query
                        .GroupBy(x => x.SourceStandard)
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

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.RequirementReference_Read)]
        public async Task<RequirementReferenceReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.Findings)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new RequirementReferenceReportDto
            {
                Data = ObjectMapper.Map<RequirementReferenceDto>(root),
                Findings = ObjectMapper.Map<List<FindingDto>>(
                    root.Findings == null ? new List<Finding>() : root.Findings.ToList()),
            };
        }

    }
}
