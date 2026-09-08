using AutoMapper;
using DenetimBulgu.Entities;
using DenetimBulgu.FindingLevels.Dto;

namespace DenetimBulgu.FindingLevels
{
    public class FindingLevelMapProfile : Profile
    {
        public FindingLevelMapProfile()
        {
            CreateMap<FindingLevel, FindingLevelDto>();
            CreateMap<CreateFindingLevelDto, FindingLevel>();
            CreateMap<FindingLevelDto, FindingLevel>();
        }
    }
}
