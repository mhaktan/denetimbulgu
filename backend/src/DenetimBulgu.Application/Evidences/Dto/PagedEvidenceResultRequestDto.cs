using System;
using Abp.Application.Services.Dto;

namespace DenetimBulgu.Evidences.Dto
{
    public class PagedEvidenceResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? CorrectiveActionId { get; set; }
        public long? EmployeeId { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? UploadDateFrom { get; set; }
        public DateTime? UploadDateTo { get; set; }
    }
}
