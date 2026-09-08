using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace DenetimBulgu.Evidences.Dto
{
    [AutoMapTo(typeof(Entities.Evidence))]
    public class CreateEvidenceDto
    {
        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(300)]
        public string FileName { get; set; }

        public DateTime UploadDate { get; set; }

        public long CorrectiveActionId { get; set; }

        public long EmployeeId { get; set; }

    }
}