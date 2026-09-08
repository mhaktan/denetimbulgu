using AutoMapper;
using DenetimBulgu.Entities;
using DenetimBulgu.ActionProgressNotes.Dto;

namespace DenetimBulgu.ActionProgressNotes
{
    public class ActionProgressNoteMapProfile : Profile
    {
        public ActionProgressNoteMapProfile()
        {
            CreateMap<ActionProgressNote, ActionProgressNoteDto>();
            CreateMap<CreateActionProgressNoteDto, ActionProgressNote>();
            CreateMap<ActionProgressNoteDto, ActionProgressNote>();
        }
    }
}
