using System;
using Abp.Application.Services.Dto;

namespace DenetimBulgu.ActionProgressNotes.Dto
{
    public class PagedActionProgressNoteResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? CorrectiveActionId { get; set; }
        public long? EmployeeId { get; set; }
        public string Note { get; set; }
        public DateTime? NoteDate { get; set; }
        public DateTime? NoteDateFrom { get; set; }
        public DateTime? NoteDateTo { get; set; }
    }
}
