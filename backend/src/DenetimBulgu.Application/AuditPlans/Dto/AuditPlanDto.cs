using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DenetimBulgu.AuditPlans.Dto
{
    [AutoMapFrom(typeof(Entities.AuditPlan))]
    public class AuditPlanDto : EntityDto<long>
    {
        public int Year { get; set; }

        public string Period { get; set; }

        public string ScopeDescription { get; set; }

        public int Status { get; set; }

        public long? QualityManagerApproverId { get; set; }

        /// <summary>
        /// String form of the status — used by flow conditions (triggerData.statusName equals "PendingX").
        /// </summary>
        public string StatusName { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}