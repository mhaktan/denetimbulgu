using AutoMapper;
using DenetimBulgu.Entities;
using DenetimBulgu.CorrectiveActions.Dto;

namespace DenetimBulgu.CorrectiveActions
{
    public class CorrectiveActionMapProfile : Profile
    {
        public CorrectiveActionMapProfile()
        {
            CreateMap<CorrectiveAction, CorrectiveActionDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()));
            CreateMap<CreateCorrectiveActionDto, CorrectiveAction>();
            CreateMap<CorrectiveActionDto, CorrectiveAction>();
        }
    }
}
