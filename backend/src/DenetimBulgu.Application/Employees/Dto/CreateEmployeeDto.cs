using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace DenetimBulgu.Employees.Dto
{
    [AutoMapTo(typeof(Entities.Employee))]
    public class CreateEmployeeDto
    {
        [Required]
        [MaxLength(50)]
        public string RegistrationNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        public bool IsActive { get; set; }

        public long UserId { get; set; }

        public long DepartmentId { get; set; }

    }
}