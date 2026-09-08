using System;
using Abp.Application.Services.Dto;

namespace DenetimBulgu.Audits.Dto
{
    public class PagedAuditResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? AuditPlanId { get; set; }
        public long? AuditTypeId { get; set; }
        public long? DepartmentId { get; set; }
        public long? EmployeeId { get; set; }
        public string AuditNumber { get; set; }
        public DateTime? PlannedDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public int? Status { get; set; }
        public DateTime? PlannedDateFrom { get; set; }
        public DateTime? PlannedDateTo { get; set; }
        public DateTime? ActualDateFrom { get; set; }
        public DateTime? ActualDateTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
