using System;
using Abp.Application.Services.Dto;

namespace DenetimBulgu.Departments.Dto
{
    public class PagedDepartmentResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? DepartmentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
