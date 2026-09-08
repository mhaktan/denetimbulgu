using System;
using DenetimBulgu.Analytics.Dto;

namespace DenetimBulgu.AuditTeamMembers.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class AuditTeamMemberGroupedCountInput : PagedAuditTeamMemberResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class AuditTeamMemberStatsInput : PagedAuditTeamMemberResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
