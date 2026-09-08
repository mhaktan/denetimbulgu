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
using DenetimBulgu.Employees.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.Audits.Dto;
using DenetimBulgu.AuditTeamMembers.Dto;
using DenetimBulgu.CorrectiveActions.Dto;
using DenetimBulgu.ActionProgressNotes.Dto;
using DenetimBulgu.Evidences.Dto;
using DenetimBulgu.Authorization;
using DenetimBulgu.Flows;

namespace DenetimBulgu.Employees
{
    public class EmployeeAppService : AsyncCrudAppService<
        Employee,
        EmployeeDto,
        long,
        PagedEmployeeResultRequestDto,
        CreateEmployeeDto,
        EmployeeDto>,
        IEmployeeAppService
    {
        private readonly IFlowEngine _flowEngine;

        public EmployeeAppService(IRepository<Employee, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Employee_Read;
            GetAllPermissionName = PermissionNames.Employee_Read;
            CreatePermissionName = PermissionNames.Employee_Create;
            UpdatePermissionName = PermissionNames.Employee_Update;
            DeletePermissionName = PermissionNames.Employee_Delete;
        }

        protected override IQueryable<Employee> CreateFilteredQuery(PagedEmployeeResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.RegistrationNumber != null && x.RegistrationNumber.Contains(input.Keyword)) ||
                    (x.FullName != null && x.FullName.Contains(input.Keyword)) ||
                    (x.Email != null && x.Email.Contains(input.Keyword)))
                .WhereIf(!input.RegistrationNumber.IsNullOrWhiteSpace(), x => x.RegistrationNumber != null && x.RegistrationNumber.Contains(input.RegistrationNumber))
                .WhereIf(!input.FullName.IsNullOrWhiteSpace(), x => x.FullName != null && x.FullName.Contains(input.FullName))
                .WhereIf(!input.Email.IsNullOrWhiteSpace(), x => x.Email != null && x.Email.Contains(input.Email))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value)
                .WhereIf(input.DepartmentId.HasValue, x => x.DepartmentId == input.DepartmentId.Value);
        }

        public override async Task<EmployeeDto> CreateAsync(CreateEmployeeDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Employee", result);
            return result;
        }

        public override async Task<EmployeeDto> UpdateAsync(EmployeeDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Employee", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Employee", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.Employee_Read)]
        public List<GroupCountDto> GetGroupedCount(EmployeeGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "UserId", "DepartmentId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "UserId":
                    return query
                        .GroupBy(x => new { Key = x.UserId, Label = x.User == null ? null : x.User.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key == null ? null : g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "DepartmentId":
                    return query
                        .GroupBy(x => new { Key = x.DepartmentId, Label = x.Department == null ? null : x.Department.Name })
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
        [Abp.Authorization.AbpAuthorize(PermissionNames.Employee_Read)]
        public async Task<EmployeeReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.Audits)
                .Include(x => x.AuditTeamMembers)
                .Include(x => x.CorrectiveActions)
                .Include(x => x.ActionProgressNotes)
                .Include(x => x.Evidences)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new EmployeeReportDto
            {
                Data = ObjectMapper.Map<EmployeeDto>(root),
                Audits = ObjectMapper.Map<List<AuditDto>>(
                    root.Audits == null ? new List<Audit>() : root.Audits.ToList()),
                AuditTeamMembers = ObjectMapper.Map<List<AuditTeamMemberDto>>(
                    root.AuditTeamMembers == null ? new List<AuditTeamMember>() : root.AuditTeamMembers.ToList()),
                CorrectiveActions = ObjectMapper.Map<List<CorrectiveActionDto>>(
                    root.CorrectiveActions == null ? new List<CorrectiveAction>() : root.CorrectiveActions.ToList()),
                ActionProgressNotes = ObjectMapper.Map<List<ActionProgressNoteDto>>(
                    root.ActionProgressNotes == null ? new List<ActionProgressNote>() : root.ActionProgressNotes.ToList()),
                Evidences = ObjectMapper.Map<List<EvidenceDto>>(
                    root.Evidences == null ? new List<Evidence>() : root.Evidences.ToList()),
            };
        }

    }
}
