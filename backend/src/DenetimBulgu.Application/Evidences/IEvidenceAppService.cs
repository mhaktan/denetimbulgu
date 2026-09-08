using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.Evidences.Dto;

namespace DenetimBulgu.Evidences
{
    public interface IEvidenceAppService : IAsyncCrudAppService<
        EvidenceDto,
        long,
        PagedEvidenceResultRequestDto,
        CreateEvidenceDto,
        EvidenceDto>
    {
        List<GroupCountDto> GetGroupedCount(EvidenceGroupedCountInput input);
    }
}
