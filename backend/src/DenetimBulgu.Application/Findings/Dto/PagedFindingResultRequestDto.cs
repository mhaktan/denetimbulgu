using System;
using Abp.Application.Services.Dto;

namespace DenetimBulgu.Findings.Dto
{
    public class PagedFindingResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? AuditId { get; set; }
        public long? FindingLevelId { get; set; }
        public long? DepartmentId { get; set; }
        public long? RequirementReferenceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DetectedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? Status { get; set; }
        public string RevisionNote { get; set; }
        public DateTime? DetectedDateFrom { get; set; }
        public DateTime? DetectedDateTo { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public DateTime? ClosedDateFrom { get; set; }
        public DateTime? ClosedDateTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
