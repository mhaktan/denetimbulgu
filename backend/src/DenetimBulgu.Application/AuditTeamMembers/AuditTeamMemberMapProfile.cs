using AutoMapper;
using DenetimBulgu.Entities;
using DenetimBulgu.AuditTeamMembers.Dto;

namespace DenetimBulgu.AuditTeamMembers
{
    public class AuditTeamMemberMapProfile : Profile
    {
        public AuditTeamMemberMapProfile()
        {
            CreateMap<AuditTeamMember, AuditTeamMemberDto>();
            CreateMap<CreateAuditTeamMemberDto, AuditTeamMember>();
            CreateMap<AuditTeamMemberDto, AuditTeamMember>();
        }
    }
}
