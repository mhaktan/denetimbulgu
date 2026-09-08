using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace DenetimBulgu.Entities
{
    [Table("RequirementReferences")]
    public class RequirementReference : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        [MaxLength(300)]
        public string Title { get; set; }

        public RequirementReferenceSourceStandard SourceStandard { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        public virtual ICollection<Finding> Findings { get; set; }

    }
}