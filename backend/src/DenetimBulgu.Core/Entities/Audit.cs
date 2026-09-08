using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace DenetimBulgu.Entities
{
    // State Machine: status — Planned → InProgress → Completed
    // Initial: Planned | Transitions: Planned→InProgress[Start], InProgress→Completed[Complete]
    [Table("Audits")]
    public class Audit : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(50)]
        public string AuditNumber { get; set; }

        public DateTime PlannedDate { get; set; }

        public DateTime? ActualDate { get; set; }

        public AuditStatus Status { get; set; }

        public long AuditPlanId { get; set; }

        [ForeignKey(nameof(AuditPlanId))]
        public virtual AuditPlan AuditPlan { get; set; }

        public long AuditTypeId { get; set; }

        [ForeignKey(nameof(AuditTypeId))]
        public virtual AuditType AuditType { get; set; }

        public long DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; }

        public long EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }

        public virtual ICollection<AuditTeamMember> AuditTeamMembers { get; set; }

        public virtual ICollection<Finding> Findings { get; set; }

    }
}