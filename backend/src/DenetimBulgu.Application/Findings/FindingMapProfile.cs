using AutoMapper;
using DenetimBulgu.Entities;
using DenetimBulgu.Findings.Dto;

namespace DenetimBulgu.Findings
{
    public class FindingMapProfile : Profile
    {
        public FindingMapProfile()
        {
            CreateMap<Finding, FindingDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()));
            CreateMap<CreateFindingDto, Finding>();
            CreateMap<FindingDto, Finding>();
        }
    }
}
