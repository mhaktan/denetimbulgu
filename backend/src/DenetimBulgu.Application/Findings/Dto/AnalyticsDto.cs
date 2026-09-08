using System;
using DenetimBulgu.Analytics.Dto;

namespace DenetimBulgu.Findings.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class FindingGroupedCountInput : PagedFindingResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class FindingStatsInput : PagedFindingResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
