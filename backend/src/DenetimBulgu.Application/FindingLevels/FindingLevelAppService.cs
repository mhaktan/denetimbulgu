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
using DenetimBulgu.FindingLevels.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.Findings.Dto;
using DenetimBulgu.Authorization;
using DenetimBulgu.Flows;

namespace DenetimBulgu.FindingLevels
{
    public class FindingLevelAppService : AsyncCrudAppService<
        FindingLevel,
        FindingLevelDto,
        long,
        PagedFindingLevelResultRequestDto,
        CreateFindingLevelDto,
        FindingLevelDto>,
        IFindingLevelAppService
    {
        private readonly IFlowEngine _flowEngine;

        public FindingLevelAppService(IRepository<FindingLevel, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.FindingLevel_Read;
            GetAllPermissionName = PermissionNames.FindingLevel_Read;
            CreatePermissionName = PermissionNames.FindingLevel_Create;
            UpdatePermissionName = PermissionNames.FindingLevel_Update;
            DeletePermissionName = PermissionNames.FindingLevel_Delete;
        }

        protected override IQueryable<FindingLevel> CreateFilteredQuery(PagedFindingLevelResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Code != null && x.Code.Contains(input.Keyword)) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)))
                .WhereIf(!input.Code.IsNullOrWhiteSpace(), x => x.Code != null && x.Code.Contains(input.Code))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(input.DefaultClosureDays.HasValue, x => x.DefaultClosureDays == input.DefaultClosureDays.Value)
                .WhereIf(input.DefaultClosureDaysFrom.HasValue, x => x.DefaultClosureDays >= input.DefaultClosureDaysFrom.Value)
                .WhereIf(input.DefaultClosureDaysTo.HasValue, x => x.DefaultClosureDays <= input.DefaultClosureDaysTo.Value);
        }

        public override async Task<FindingLevelDto> CreateAsync(CreateFindingLevelDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "FindingLevel", result);
            return result;
        }

        public override async Task<FindingLevelDto> UpdateAsync(FindingLevelDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "FindingLevel", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "FindingLevel", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.FindingLevel_Read)]
        public decimal? GetStats(FindingLevelStatsInput input)
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

            var allowedNumeric = new[] { "DefaultClosureDays" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "DefaultClosureDays": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.DefaultClosureDays)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.DefaultClosureDays)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.DefaultClosureDays)
                            : query.Average(x => (decimal?)x.DefaultClosureDays);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.FindingLevel_Read)]
        public async Task<FindingLevelReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.Findings)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new FindingLevelReportDto
            {
                Data = ObjectMapper.Map<FindingLevelDto>(root),
                Findings = ObjectMapper.Map<List<FindingDto>>(
                    root.Findings == null ? new List<Finding>() : root.Findings.ToList()),
            };
        }

    }
}
