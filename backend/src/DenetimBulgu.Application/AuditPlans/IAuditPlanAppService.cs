using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.StateMachine.Dto;
using DenetimBulgu.AuditPlans.Dto;

namespace DenetimBulgu.AuditPlans
{
    public interface IAuditPlanAppService : IAsyncCrudAppService<
        AuditPlanDto,
        long,
        PagedAuditPlanResultRequestDto,
        CreateAuditPlanDto,
        AuditPlanDto>
    {
        Task<AuditPlanDto> ChangeStatusAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(AuditPlanGroupedCountInput input);
        decimal? GetStats(AuditPlanStatsInput input);
        Task<AuditPlanReportDto> GetReportData(long id);
    }
}
