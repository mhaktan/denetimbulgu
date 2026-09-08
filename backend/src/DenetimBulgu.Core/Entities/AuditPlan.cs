using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace DenetimBulgu.Entities
{
    // State Machine: status — Draft → Approved
    // Initial: Draft | Transitions: Draft→Approved[Approve]
    [Table("AuditPlans")]
    public class AuditPlan : FullAuditedEntity<long>
    {
        public int Year { get; set; }

        [Required]
        [MaxLength(50)]
        public string Period { get; set; }

        [Required]
        [MaxLength(1000)]
        public string ScopeDescription { get; set; }

        public AuditPlanStatus Status { get; set; }

        public long? QualityManagerApproverId { get; set; }

        public virtual ICollection<Audit> Audits { get; set; }

    }
}