using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace DenetimBulgu.Departments.Dto
{
    [AutoMapFrom(typeof(Entities.Department))]
    public class DepartmentDto : EntityDto<long>
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public long DepartmentId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}