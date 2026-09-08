using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace DenetimBulgu.AuditTeamMembers.Dto
{
    [AutoMapTo(typeof(Entities.AuditTeamMember))]
    public class CreateAuditTeamMemberDto
    {
        public int Role { get; set; }

        public long AuditId { get; set; }

        public long EmployeeId { get; set; }

    }
}