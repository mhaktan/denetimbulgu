using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.Departments.Dto;

namespace DenetimBulgu.Departments
{
    public interface IDepartmentAppService : IAsyncCrudAppService<
        DepartmentDto,
        long,
        PagedDepartmentResultRequestDto,
        CreateDepartmentDto,
        DepartmentDto>
    {
        List<GroupCountDto> GetGroupedCount(DepartmentGroupedCountInput input);
        Task<DepartmentReportDto> GetReportData(long id);
    }
}
