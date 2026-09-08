using System;
using System.Collections.Generic;
using DenetimBulgu.CorrectiveActions.Dto;
using DenetimBulgu.Approvals.Dto;

namespace DenetimBulgu.Findings.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class FindingReportDto
    {
        public FindingDto Data { get; set; }
        public List<CorrectiveActionDto> CorrectiveActions { get; set; }
        public List<ApprovalRecordDto> ApprovalHistory { get; set; }
    }
}
