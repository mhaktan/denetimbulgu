using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DenetimBulgu.ActionProgressNotes.Dto
{
    [AutoMapFrom(typeof(Entities.ActionProgressNote))]
    public class ActionProgressNoteDto : EntityDto<long>
    {
        public string Note { get; set; }

        public DateTime NoteDate { get; set; }

        public long CorrectiveActionId { get; set; }

        public long EmployeeId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}