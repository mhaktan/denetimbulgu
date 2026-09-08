using System;
using System.Collections.Generic;
using DenetimBulgu.ActionProgressNotes.Dto;
using DenetimBulgu.Evidences.Dto;
using DenetimBulgu.Approvals.Dto;

namespace DenetimBulgu.CorrectiveActions.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class CorrectiveActionReportDto
    {
        public CorrectiveActionDto Data { get; set; }
        public List<ActionProgressNoteDto> ActionProgressNotes { get; set; }
        public List<EvidenceDto> Evidences { get; set; }
        public List<ApprovalRecordDto> ApprovalHistory { get; set; }
    }
}
