using System;
using System.Collections.Generic;
using DenetimBulgu.Findings.Dto;

namespace DenetimBulgu.FindingLevels.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class FindingLevelReportDto
    {
        public FindingLevelDto Data { get; set; }
        public List<FindingDto> Findings { get; set; }
    }
}
