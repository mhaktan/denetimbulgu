using System;
using DenetimBulgu.Analytics.Dto;

namespace DenetimBulgu.Evidences.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class EvidenceGroupedCountInput : PagedEvidenceResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class EvidenceStatsInput : PagedEvidenceResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
