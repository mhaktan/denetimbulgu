using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace DenetimBulgu.Audits.Dto
{
    [AutoMapTo(typeof(Entities.Audit))]
    public class CreateAuditDto
    {
        [Required]
        [MaxLength(50)]
        public string AuditNumber { get; set; }

        public DateTime PlannedDate { get; set; }

        public DateTime? ActualDate { get; set; }

        public int Status { get; set; }

        public long AuditPlanId { get; set; }

        public long AuditTypeId { get; set; }

        public long DepartmentId { get; set; }

        public long EmployeeId { get; set; }

    }
}