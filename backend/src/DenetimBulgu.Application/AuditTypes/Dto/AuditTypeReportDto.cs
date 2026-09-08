using System;
using System.Collections.Generic;
using DenetimBulgu.Audits.Dto;

namespace DenetimBulgu.AuditTypes.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class AuditTypeReportDto
    {
        public AuditTypeDto Data { get; set; }
        public List<AuditDto> Audits { get; set; }
    }
}
