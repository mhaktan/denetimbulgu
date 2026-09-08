using System;
using Abp.Application.Services.Dto;

namespace DenetimBulgu.RequirementReferences.Dto
{
    public class PagedRequirementReferenceResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public int? SourceStandard { get; set; }
        public string Description { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string SourceStandardIn { get; set; }
        public int? SourceStandardNot { get; set; }
    }
}
