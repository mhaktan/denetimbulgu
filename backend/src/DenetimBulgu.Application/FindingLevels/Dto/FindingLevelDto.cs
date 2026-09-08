using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DenetimBulgu.FindingLevels.Dto
{
    [AutoMapFrom(typeof(Entities.FindingLevel))]
    public class FindingLevelDto : EntityDto<long>
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int DefaultClosureDays { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}