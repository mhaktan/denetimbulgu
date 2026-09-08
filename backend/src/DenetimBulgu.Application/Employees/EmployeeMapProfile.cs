using AutoMapper;
using DenetimBulgu.Entities;
using DenetimBulgu.Employees.Dto;

namespace DenetimBulgu.Employees
{
    public class EmployeeMapProfile : Profile
    {
        public EmployeeMapProfile()
        {
            CreateMap<Employee, EmployeeDto>();
            CreateMap<CreateEmployeeDto, Employee>();
            CreateMap<EmployeeDto, Employee>();
        }
    }
}
