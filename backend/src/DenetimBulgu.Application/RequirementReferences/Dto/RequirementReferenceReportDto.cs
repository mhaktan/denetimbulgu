using System;
using System.Collections.Generic;
using DenetimBulgu.Findings.Dto;

namespace DenetimBulgu.RequirementReferences.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class RequirementReferenceReportDto
    {
        public RequirementReferenceDto Data { get; set; }
        public List<FindingDto> Findings { get; set; }
    }
}
