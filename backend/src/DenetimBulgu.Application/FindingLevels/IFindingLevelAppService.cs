using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.FindingLevels.Dto;

namespace DenetimBulgu.FindingLevels
{
    public interface IFindingLevelAppService : IAsyncCrudAppService<
        FindingLevelDto,
        long,
        PagedFindingLevelResultRequestDto,
        CreateFindingLevelDto,
        FindingLevelDto>
    {
        decimal? GetStats(FindingLevelStatsInput input);
        Task<FindingLevelReportDto> GetReportData(long id);
    }
}
