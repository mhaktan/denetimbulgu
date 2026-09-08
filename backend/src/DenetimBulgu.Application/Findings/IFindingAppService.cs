using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.StateMachine.Dto;
using DenetimBulgu.Findings.Dto;

namespace DenetimBulgu.Findings
{
    public interface IFindingAppService : IAsyncCrudAppService<
        FindingDto,
        long,
        PagedFindingResultRequestDto,
        CreateFindingDto,
        FindingDto>
    {
        Task<FindingDto> ChangeStatusAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(FindingGroupedCountInput input);
        decimal? GetStats(FindingStatsInput input);
        Task<FindingReportDto> GetReportData(long id);
    }
}
