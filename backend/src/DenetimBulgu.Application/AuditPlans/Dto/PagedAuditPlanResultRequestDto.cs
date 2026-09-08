using System;
using Abp.Application.Services.Dto;

namespace DenetimBulgu.AuditPlans.Dto
{
    public class PagedAuditPlanResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public int? Year { get; set; }
        public string Period { get; set; }
        public string ScopeDescription { get; set; }
        public int? Status { get; set; }
        public long? QualityManagerApproverId { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public long? QualityManagerApproverIdFrom { get; set; }
        public long? QualityManagerApproverIdTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
