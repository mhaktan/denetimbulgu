using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace DenetimBulgu.ActionProgressNotes.Dto
{
    [AutoMapTo(typeof(Entities.ActionProgressNote))]
    public class CreateActionProgressNoteDto
    {
        [Required]
        [MaxLength(2000)]
        public string Note { get; set; }

        public DateTime NoteDate { get; set; }

        public long CorrectiveActionId { get; set; }

        public long EmployeeId { get; set; }

    }
}