using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace DenetimBulgu.RequirementReferences.Dto
{
    [AutoMapTo(typeof(Entities.RequirementReference))]
    public class CreateRequirementReferenceDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        [MaxLength(300)]
        public string Title { get; set; }

        public int SourceStandard { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

    }
}