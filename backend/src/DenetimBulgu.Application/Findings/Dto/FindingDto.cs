using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DenetimBulgu.Findings.Dto
{
    [AutoMapFrom(typeof(Entities.Finding))]
    public class FindingDto : EntityDto<long>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DetectedDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ClosedDate { get; set; }

        public int Status { get; set; }

        public string RevisionNote { get; set; }

        /// <summary>
        /// String form of the status — used by flow conditions (triggerData.statusName equals "PendingX").
        /// </summary>
        public string StatusName { get; set; }

        public long AuditId { get; set; }

        public long FindingLevelId { get; set; }

        public long DepartmentId { get; set; }

        public long RequirementReferenceId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}