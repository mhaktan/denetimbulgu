using AutoMapper;
using DenetimBulgu.Approvals.Dto;
using DenetimBulgu.Entities;

namespace DenetimBulgu.Approvals
{
    public class ApprovalMapProfile : Profile
    {
        public ApprovalMapProfile()
        {
            CreateMap<ApprovalRecord, ApprovalRecordDto>();
            CreateMap<StatusChangeLog, StatusChangeLogDto>();
        }
    }
}
