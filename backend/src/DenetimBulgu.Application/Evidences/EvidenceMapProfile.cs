using AutoMapper;
using DenetimBulgu.Entities;
using DenetimBulgu.Evidences.Dto;

namespace DenetimBulgu.Evidences
{
    public class EvidenceMapProfile : Profile
    {
        public EvidenceMapProfile()
        {
            CreateMap<Evidence, EvidenceDto>();
            CreateMap<CreateEvidenceDto, Evidence>();
            CreateMap<EvidenceDto, Evidence>();
        }
    }
}
