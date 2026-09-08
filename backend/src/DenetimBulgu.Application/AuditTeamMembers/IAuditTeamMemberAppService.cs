using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.AuditTeamMembers.Dto;

namespace DenetimBulgu.AuditTeamMembers
{
    public interface IAuditTeamMemberAppService : IAsyncCrudAppService<
        AuditTeamMemberDto,
        long,
        PagedAuditTeamMemberResultRequestDto,
        CreateAuditTeamMemberDto,
        AuditTeamMemberDto>
    {
        List<GroupCountDto> GetGroupedCount(AuditTeamMemberGroupedCountInput input);
    }
}
