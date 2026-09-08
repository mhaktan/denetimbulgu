using System;
using Abp.Application.Services.Dto;

namespace DenetimBulgu.AuditTeamMembers.Dto
{
    public class PagedAuditTeamMemberResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? AuditId { get; set; }
        public long? EmployeeId { get; set; }
        public int? Role { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string RoleIn { get; set; }
        public int? RoleNot { get; set; }
    }
}
