using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.Employees.Dto;

namespace DenetimBulgu.Employees
{
    public interface IEmployeeAppService : IAsyncCrudAppService<
        EmployeeDto,
        long,
        PagedEmployeeResultRequestDto,
        CreateEmployeeDto,
        EmployeeDto>
    {
        List<GroupCountDto> GetGroupedCount(EmployeeGroupedCountInput input);
        Task<EmployeeReportDto> GetReportData(long id);
    }
}
