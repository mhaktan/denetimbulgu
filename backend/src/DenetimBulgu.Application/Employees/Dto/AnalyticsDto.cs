using System;
using DenetimBulgu.Analytics.Dto;

namespace DenetimBulgu.Employees.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class EmployeeGroupedCountInput : PagedEmployeeResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class EmployeeStatsInput : PagedEmployeeResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
