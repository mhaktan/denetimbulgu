using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace DenetimBulgu.AuditPlans.Dto
{
    [AutoMapTo(typeof(Entities.AuditPlan))]
    public class CreateAuditPlanDto
    {
        public int Year { get; set; }

        [Required]
        [MaxLength(50)]
        public string Period { get; set; }

        [Required]
        [MaxLength(1000)]
        public string ScopeDescription { get; set; }

        public int Status { get; set; }

        public long? QualityManagerApproverId { get; set; }

    }
}