using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace DenetimBulgu.Entities
{
    // State Machine: status — Open → InProgress → Completed → Cancelled
    // Initial: Open | Transitions: Open→InProgress[Start], InProgress→Completed[Complete], *→Cancelled[Cancel]
    [Table("CorrectiveActions")]
    public class CorrectiveAction : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(2000)]
        public string RootCauseAnalysis { get; set; }

        [Required]
        [MaxLength(2000)]
        public string ActionDescription { get; set; }

        public DateTime TargetDate { get; set; }

        public DateTime? ActualClosureDate { get; set; }

        public CorrectiveActionStatus Status { get; set; }

        public long FindingId { get; set; }

        [ForeignKey(nameof(FindingId))]
        public virtual Finding Finding { get; set; }

        public long EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }

        public virtual ICollection<ActionProgressNote> ActionProgressNotes { get; set; }

        public virtual ICollection<Evidence> Evidences { get; set; }

    }
}