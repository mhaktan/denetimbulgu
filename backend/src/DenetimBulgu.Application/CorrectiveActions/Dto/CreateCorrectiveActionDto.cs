using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace DenetimBulgu.CorrectiveActions.Dto
{
    [AutoMapTo(typeof(Entities.CorrectiveAction))]
    public class CreateCorrectiveActionDto
    {
        [Required]
        [MaxLength(2000)]
        public string RootCauseAnalysis { get; set; }

        [Required]
        [MaxLength(2000)]
        public string ActionDescription { get; set; }

        public DateTime TargetDate { get; set; }

        public DateTime? ActualClosureDate { get; set; }

        public int Status { get; set; }

        public long FindingId { get; set; }

        public long EmployeeId { get; set; }

    }
}