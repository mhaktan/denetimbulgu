using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace DenetimBulgu.Entities
{
    [Table("AuditTeamMembers")]
    public class AuditTeamMember : FullAuditedEntity<long>
    {
        public AuditTeamMemberRole Role { get; set; }

        public long AuditId { get; set; }

        [ForeignKey(nameof(AuditId))]
        public virtual Audit Audit { get; set; }

        public long EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }

    }
}