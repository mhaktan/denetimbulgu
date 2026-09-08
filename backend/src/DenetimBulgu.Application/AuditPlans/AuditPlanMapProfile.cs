using AutoMapper;
using DenetimBulgu.Entities;
using DenetimBulgu.AuditPlans.Dto;

namespace DenetimBulgu.AuditPlans
{
    public class AuditPlanMapProfile : Profile
    {
        public AuditPlanMapProfile()
        {
            CreateMap<AuditPlan, AuditPlanDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()));
            CreateMap<CreateAuditPlanDto, AuditPlan>();
            CreateMap<AuditPlanDto, AuditPlan>();
        }
    }
}
