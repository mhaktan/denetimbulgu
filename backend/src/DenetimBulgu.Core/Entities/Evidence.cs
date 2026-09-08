using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace DenetimBulgu.Entities
{
    [Table("Evidences")]
    public class Evidence : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(300)]
        public string FileName { get; set; }

        public DateTime UploadDate { get; set; }

        public long CorrectiveActionId { get; set; }

        [ForeignKey(nameof(CorrectiveActionId))]
        public virtual CorrectiveAction CorrectiveAction { get; set; }

        public long EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }

    }
}