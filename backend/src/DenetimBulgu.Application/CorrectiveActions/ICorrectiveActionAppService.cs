using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.StateMachine.Dto;
using DenetimBulgu.CorrectiveActions.Dto;

namespace DenetimBulgu.CorrectiveActions
{
    public interface ICorrectiveActionAppService : IAsyncCrudAppService<
        CorrectiveActionDto,
        long,
        PagedCorrectiveActionResultRequestDto,
        CreateCorrectiveActionDto,
        CorrectiveActionDto>
    {
        Task<CorrectiveActionDto> ChangeStatusAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(CorrectiveActionGroupedCountInput input);
        decimal? GetStats(CorrectiveActionStatsInput input);
        Task<CorrectiveActionReportDto> GetReportData(long id);
    }
}
