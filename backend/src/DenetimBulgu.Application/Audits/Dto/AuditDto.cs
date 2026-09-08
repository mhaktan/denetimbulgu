using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DenetimBulgu.Audits.Dto
{
    [AutoMapFrom(typeof(Entities.Audit))]
    public class AuditDto : EntityDto<long>
    {
        public string AuditNumber { get; set; }

        public DateTime PlannedDate { get; set; }

        public DateTime? ActualDate { get; set; }

        public int Status { get; set; }

        /// <summary>
        /// String form of the status — used by flow conditions (triggerData.statusName equals "PendingX").
        /// </summary>
        public string StatusName { get; set; }

        public long AuditPlanId { get; set; }

        public long AuditTypeId { get; set; }

        public long DepartmentId { get; set; }

        public long EmployeeId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}