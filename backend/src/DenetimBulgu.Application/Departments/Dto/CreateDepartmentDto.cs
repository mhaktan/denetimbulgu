using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace DenetimBulgu.Departments.Dto
{
    [AutoMapTo(typeof(Entities.Department))]
    public class CreateDepartmentDto
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public long DepartmentId { get; set; }

    }
}