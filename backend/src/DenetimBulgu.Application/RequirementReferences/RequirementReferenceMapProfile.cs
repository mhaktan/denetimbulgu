using AutoMapper;
using DenetimBulgu.Entities;
using DenetimBulgu.RequirementReferences.Dto;

namespace DenetimBulgu.RequirementReferences
{
    public class RequirementReferenceMapProfile : Profile
    {
        public RequirementReferenceMapProfile()
        {
            CreateMap<RequirementReference, RequirementReferenceDto>();
            CreateMap<CreateRequirementReferenceDto, RequirementReference>();
            CreateMap<RequirementReferenceDto, RequirementReference>();
        }
    }
}
