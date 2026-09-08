using AutoMapper;
using DenetimBulgu.Entities;
using DenetimBulgu.Audits.Dto;

namespace DenetimBulgu.Audits
{
    public class AuditMapProfile : Profile
    {
        public AuditMapProfile()
        {
            CreateMap<Audit, AuditDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()));
            CreateMap<CreateAuditDto, Audit>();
            CreateMap<AuditDto, Audit>();
        }
    }
}
