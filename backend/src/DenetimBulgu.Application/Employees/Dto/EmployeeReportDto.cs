using System;
using System.Collections.Generic;
using DenetimBulgu.Audits.Dto;
using DenetimBulgu.AuditTeamMembers.Dto;
using DenetimBulgu.CorrectiveActions.Dto;
using DenetimBulgu.ActionProgressNotes.Dto;
using DenetimBulgu.Evidences.Dto;

namespace DenetimBulgu.Employees.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class EmployeeReportDto
    {
        public EmployeeDto Data { get; set; }
        public List<AuditDto> Audits { get; set; }
        public List<AuditTeamMemberDto> AuditTeamMembers { get; set; }
        public List<CorrectiveActionDto> CorrectiveActions { get; set; }
        public List<ActionProgressNoteDto> ActionProgressNotes { get; set; }
        public List<EvidenceDto> Evidences { get; set; }
    }
}
