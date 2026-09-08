using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace DenetimBulgu.Entities
{
    [Table("ActionProgressNotes")]
    public class ActionProgressNote : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(2000)]
        public string Note { get; set; }

        public DateTime NoteDate { get; set; }

        public long CorrectiveActionId { get; set; }

        [ForeignKey(nameof(CorrectiveActionId))]
        public virtual CorrectiveAction CorrectiveAction { get; set; }

        public long EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }

    }
}