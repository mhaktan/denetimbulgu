using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.StateMachine.Dto;
using DenetimBulgu.Audits.Dto;

namespace DenetimBulgu.Audits
{
    public interface IAuditAppService : IAsyncCrudAppService<
        AuditDto,
        long,
        PagedAuditResultRequestDto,
        CreateAuditDto,
        AuditDto>
    {
        Task<AuditDto> ChangeStatusAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(AuditGroupedCountInput input);
        decimal? GetStats(AuditStatsInput input);
        Task<AuditReportDto> GetReportData(long id);
    }
}
