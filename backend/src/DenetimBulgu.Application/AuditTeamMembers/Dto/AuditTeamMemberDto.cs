using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DenetimBulgu.AuditTeamMembers.Dto
{
    [AutoMapFrom(typeof(Entities.AuditTeamMember))]
    public class AuditTeamMemberDto : EntityDto<long>
    {
        public int Role { get; set; }

        public long AuditId { get; set; }

        public long EmployeeId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}