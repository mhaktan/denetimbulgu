using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.AuditTypes.Dto;

namespace DenetimBulgu.AuditTypes
{
    public interface IAuditTypeAppService : IAsyncCrudAppService<
        AuditTypeDto,
        long,
        PagedAuditTypeResultRequestDto,
        CreateAuditTypeDto,
        AuditTypeDto>
    {
        Task<AuditTypeReportDto> GetReportData(long id);
    }
}
