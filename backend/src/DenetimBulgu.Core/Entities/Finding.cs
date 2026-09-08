using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace DenetimBulgu.Entities
{
    // State Machine: status — Open → ActionPlanned → InProgress → PendingClosure → Closed
    // Initial: Open | Transitions: Open→ActionPlanned[Approve], ActionPlanned→InProgress[Approve], InProgress→PendingClosure[Approve], PendingClosure→Closed[Approve], PendingClosure→InProgress[Revise]
    [Table("Findings")]
    public class Finding : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(300)]
        public string Title { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }

        public DateTime DetectedDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ClosedDate { get; set; }

        public FindingStatus Status { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public long AuditId { get; set; }

        [ForeignKey(nameof(AuditId))]
        public virtual Audit Audit { get; set; }

        public long FindingLevelId { get; set; }

        [ForeignKey(nameof(FindingLevelId))]
        public virtual FindingLevel FindingLevel { get; set; }

        public long DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; }

        public long RequirementReferenceId { get; set; }

        [ForeignKey(nameof(RequirementReferenceId))]
        public virtual RequirementReference RequirementReference { get; set; }

        public virtual ICollection<CorrectiveAction> CorrectiveActions { get; set; }

    }
}