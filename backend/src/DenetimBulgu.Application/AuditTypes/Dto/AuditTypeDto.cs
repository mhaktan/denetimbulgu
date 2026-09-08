using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DenetimBulgu.AuditTypes.Dto
{
    [AutoMapFrom(typeof(Entities.AuditType))]
    public class AuditTypeDto : EntityDto<long>
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}