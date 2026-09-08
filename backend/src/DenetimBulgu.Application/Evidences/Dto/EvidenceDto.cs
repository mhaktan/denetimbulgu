using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DenetimBulgu.Evidences.Dto
{
    [AutoMapFrom(typeof(Entities.Evidence))]
    public class EvidenceDto : EntityDto<long>
    {
        public string Description { get; set; }

        public string FileName { get; set; }

        public DateTime UploadDate { get; set; }

        public long CorrectiveActionId { get; set; }

        public long EmployeeId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}