using System;
using System.Collections.Generic;
using DenetimBulgu.AuditTeamMembers.Dto;
using DenetimBulgu.Findings.Dto;
using DenetimBulgu.Approvals.Dto;

namespace DenetimBulgu.Audits.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class AuditReportDto
    {
        public AuditDto Data { get; set; }
        public List<AuditTeamMemberDto> AuditTeamMembers { get; set; }
        public List<FindingDto> Findings { get; set; }
        public List<ApprovalRecordDto> ApprovalHistory { get; set; }
    }
}
