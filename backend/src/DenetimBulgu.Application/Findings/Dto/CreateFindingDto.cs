using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace DenetimBulgu.Findings.Dto
{
    [AutoMapTo(typeof(Entities.Finding))]
    public class CreateFindingDto
    {
        [Required]
        [MaxLength(300)]
        public string Title { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }

        public DateTime DetectedDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ClosedDate { get; set; }

        public int Status { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public long AuditId { get; set; }

        public long FindingLevelId { get; set; }

        public long DepartmentId { get; set; }

        public long RequirementReferenceId { get; set; }

    }
}