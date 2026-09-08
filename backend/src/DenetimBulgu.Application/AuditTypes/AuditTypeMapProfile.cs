using AutoMapper;
using DenetimBulgu.Entities;
using DenetimBulgu.AuditTypes.Dto;

namespace DenetimBulgu.AuditTypes
{
    public class AuditTypeMapProfile : Profile
    {
        public AuditTypeMapProfile()
        {
            CreateMap<AuditType, AuditTypeDto>();
            CreateMap<CreateAuditTypeDto, AuditType>();
            CreateMap<AuditTypeDto, AuditType>();
        }
    }
}
