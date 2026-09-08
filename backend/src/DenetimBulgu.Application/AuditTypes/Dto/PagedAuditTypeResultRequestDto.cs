using System;
using Abp.Application.Services.Dto;

namespace DenetimBulgu.AuditTypes.Dto
{
    public class PagedAuditTypeResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
