using System;
using System.Collections.Generic;
using DenetimBulgu.Audits.Dto;
using DenetimBulgu.Approvals.Dto;

namespace DenetimBulgu.AuditPlans.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class AuditPlanReportDto
    {
        public AuditPlanDto Data { get; set; }
        public List<AuditDto> Audits { get; set; }
        public List<ApprovalRecordDto> ApprovalHistory { get; set; }
    }
}
