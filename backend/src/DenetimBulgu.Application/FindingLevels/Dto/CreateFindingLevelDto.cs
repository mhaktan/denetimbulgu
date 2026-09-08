using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace DenetimBulgu.FindingLevels.Dto
{
    [AutoMapTo(typeof(Entities.FindingLevel))]
    public class CreateFindingLevelDto
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public int DefaultClosureDays { get; set; }

    }
}