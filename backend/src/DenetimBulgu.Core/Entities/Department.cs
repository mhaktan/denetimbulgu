using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace DenetimBulgu.Entities
{
    [Table("Departments")]
    public class Department : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public long DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department ParentDepartment { get; set; }

        public virtual ICollection<Department> Departments { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        public virtual ICollection<Audit> Audits { get; set; }

        public virtual ICollection<Finding> Findings { get; set; }

    }
}