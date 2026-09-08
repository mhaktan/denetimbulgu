using System;
using Abp.Application.Services.Dto;

namespace DenetimBulgu.FindingLevels.Dto
{
    public class PagedFindingLevelResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? DefaultClosureDays { get; set; }
        public int? DefaultClosureDaysFrom { get; set; }
        public int? DefaultClosureDaysTo { get; set; }
    }
}
