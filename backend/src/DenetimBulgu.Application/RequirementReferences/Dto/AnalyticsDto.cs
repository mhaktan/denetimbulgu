using System;
using DenetimBulgu.Analytics.Dto;

namespace DenetimBulgu.RequirementReferences.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class RequirementReferenceGroupedCountInput : PagedRequirementReferenceResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class RequirementReferenceStatsInput : PagedRequirementReferenceResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
