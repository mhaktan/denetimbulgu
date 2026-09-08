using AutoMapper;
using DenetimBulgu.Entities;
using DenetimBulgu.Departments.Dto;

namespace DenetimBulgu.Departments
{
    public class DepartmentMapProfile : Profile
    {
        public DepartmentMapProfile()
        {
            CreateMap<Department, DepartmentDto>();
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<DepartmentDto, Department>();
        }
    }
}
