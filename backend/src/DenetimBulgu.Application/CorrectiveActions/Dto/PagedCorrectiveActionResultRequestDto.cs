using System;
using Abp.Application.Services.Dto;

namespace DenetimBulgu.CorrectiveActions.Dto
{
    public class PagedCorrectiveActionResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? FindingId { get; set; }
        public long? EmployeeId { get; set; }
        public string RootCauseAnalysis { get; set; }
        public string ActionDescription { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? ActualClosureDate { get; set; }
        public int? Status { get; set; }
        public DateTime? TargetDateFrom { get; set; }
        public DateTime? TargetDateTo { get; set; }
        public DateTime? ActualClosureDateFrom { get; set; }
        public DateTime? ActualClosureDateTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
