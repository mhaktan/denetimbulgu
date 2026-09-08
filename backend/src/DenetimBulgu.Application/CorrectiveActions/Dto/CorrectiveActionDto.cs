using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DenetimBulgu.CorrectiveActions.Dto
{
    [AutoMapFrom(typeof(Entities.CorrectiveAction))]
    public class CorrectiveActionDto : EntityDto<long>
    {
        public string RootCauseAnalysis { get; set; }

        public string ActionDescription { get; set; }

        public DateTime TargetDate { get; set; }

        public DateTime? ActualClosureDate { get; set; }

        public int Status { get; set; }

        /// <summary>
        /// String form of the status — used by flow conditions (triggerData.statusName equals "PendingX").
        /// </summary>
        public string StatusName { get; set; }

        public long FindingId { get; set; }

        public long EmployeeId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}