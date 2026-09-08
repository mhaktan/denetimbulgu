using System;
using System.Collections.Generic;
using DenetimBulgu.Departments.Dto;
using DenetimBulgu.Employees.Dto;
using DenetimBulgu.Audits.Dto;
using DenetimBulgu.Findings.Dto;

namespace DenetimBulgu.Departments.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class DepartmentReportDto
    {
        public DepartmentDto Data { get; set; }
        public List<DepartmentDto> Departments { get; set; }
        public List<EmployeeDto> Employees { get; set; }
        public List<AuditDto> Audits { get; set; }
        public List<FindingDto> Findings { get; set; }
    }
}
