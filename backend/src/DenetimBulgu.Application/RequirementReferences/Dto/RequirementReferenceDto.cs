using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DenetimBulgu.RequirementReferences.Dto
{
    [AutoMapFrom(typeof(Entities.RequirementReference))]
    public class RequirementReferenceDto : EntityDto<long>
    {
        public string Code { get; set; }

        public string Title { get; set; }

        public int SourceStandard { get; set; }

        public string Description { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}