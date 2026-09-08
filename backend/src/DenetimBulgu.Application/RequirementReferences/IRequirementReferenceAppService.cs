using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.RequirementReferences.Dto;

namespace DenetimBulgu.RequirementReferences
{
    public interface IRequirementReferenceAppService : IAsyncCrudAppService<
        RequirementReferenceDto,
        long,
        PagedRequirementReferenceResultRequestDto,
        CreateRequirementReferenceDto,
        RequirementReferenceDto>
    {
        List<GroupCountDto> GetGroupedCount(RequirementReferenceGroupedCountInput input);
        Task<RequirementReferenceReportDto> GetReportData(long id);
    }
}
