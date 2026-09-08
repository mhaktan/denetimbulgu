using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DenetimBulgu.Entities;
using DenetimBulgu.Departments.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.Departments.Dto;
using DenetimBulgu.Employees.Dto;
using DenetimBulgu.Audits.Dto;
using DenetimBulgu.Findings.Dto;
using DenetimBulgu.Authorization;
using DenetimBulgu.Flows;

namespace DenetimBulgu.Departments
{
    public class DepartmentAppService : AsyncCrudAppService<
        Department,
        DepartmentDto,
        long,
        PagedDepartmentResultRequestDto,
        CreateDepartmentDto,
        DepartmentDto>,
        IDepartmentAppService
    {
        private readonly IFlowEngine _flowEngine;

        public DepartmentAppService(IRepository<Department, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Department_Read;
            GetAllPermissionName = PermissionNames.Department_Read;
            CreatePermissionName = PermissionNames.Department_Create;
            UpdatePermissionName = PermissionNames.Department_Update;
            DeletePermissionName = PermissionNames.Department_Delete;
        }

        protected override IQueryable<Department> CreateFilteredQuery(PagedDepartmentResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Code != null && x.Code.Contains(input.Keyword)) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)))
                .WhereIf(!input.Code.IsNullOrWhiteSpace(), x => x.Code != null && x.Code.Contains(input.Code))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value)
                .WhereIf(input.DepartmentId.HasValue, x => x.DepartmentId == input.DepartmentId.Value);
        }

        public override async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Department", result);
            return result;
        }

        public override async Task<DepartmentDto> UpdateAsync(DepartmentDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Department", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Department", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.Department_Read)]
        public List<GroupCountDto> GetGroupedCount(DepartmentGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "DepartmentId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "DepartmentId":
                    return query
                        .GroupBy(x => new { Key = x.DepartmentId, Label = x.ParentDepartment == null ? null : x.ParentDepartment.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key == null ? null : g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                default:
                    return new List<GroupCountDto>();
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.Department_Read)]
        public async Task<DepartmentReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.Departments)
                .Include(x => x.Employees)
                .Include(x => x.Audits)
                .Include(x => x.Findings)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new DepartmentReportDto
            {
                Data = ObjectMapper.Map<DepartmentDto>(root),
                Departments = ObjectMapper.Map<List<DepartmentDto>>(
                    root.Departments == null ? new List<Department>() : root.Departments.ToList()),
                Employees = ObjectMapper.Map<List<EmployeeDto>>(
                    root.Employees == null ? new List<Employee>() : root.Employees.ToList()),
                Audits = ObjectMapper.Map<List<AuditDto>>(
                    root.Audits == null ? new List<Audit>() : root.Audits.ToList()),
                Findings = ObjectMapper.Map<List<FindingDto>>(
                    root.Findings == null ? new List<Finding>() : root.Findings.ToList()),
            };
        }

    }
}
