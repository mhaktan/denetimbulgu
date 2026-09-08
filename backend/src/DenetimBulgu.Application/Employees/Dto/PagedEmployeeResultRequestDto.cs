using System;
using Abp.Application.Services.Dto;

namespace DenetimBulgu.Employees.Dto
{
    public class PagedEmployeeResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? UserId { get; set; }
        public long? DepartmentId { get; set; }
        public string RegistrationNumber { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool? IsActive { get; set; }
    }
}
