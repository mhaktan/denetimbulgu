using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace DenetimBulgu.Entities
{
    [Table("FindingLevels")]
    public class FindingLevel : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public int DefaultClosureDays { get; set; }

        public virtual ICollection<Finding> Findings { get; set; }

    }
}