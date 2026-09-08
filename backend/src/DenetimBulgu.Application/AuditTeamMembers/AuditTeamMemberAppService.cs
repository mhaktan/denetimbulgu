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
using DenetimBulgu.AuditTeamMembers.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.Authorization;
using DenetimBulgu.Flows;

namespace DenetimBulgu.AuditTeamMembers
{
    public class AuditTeamMemberAppService : AsyncCrudAppService<
        AuditTeamMember,
        AuditTeamMemberDto,
        long,
        PagedAuditTeamMemberResultRequestDto,
        CreateAuditTeamMemberDto,
        AuditTeamMemberDto>,
        IAuditTeamMemberAppService
    {
        private readonly IFlowEngine _flowEngine;

        public AuditTeamMemberAppService(IRepository<AuditTeamMember, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.AuditTeamMember_Read;
            GetAllPermissionName = PermissionNames.AuditTeamMember_Read;
            CreatePermissionName = PermissionNames.AuditTeamMember_Create;
            UpdatePermissionName = PermissionNames.AuditTeamMember_Update;
            DeletePermissionName = PermissionNames.AuditTeamMember_Delete;
        }

        protected override IQueryable<AuditTeamMember> CreateFilteredQuery(PagedAuditTeamMemberResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword))
                .WhereIf(input.Role.HasValue, x => x.Role == (AuditTeamMemberRole)input.Role.Value)
                .WhereIf(!input.RoleIn.IsNullOrWhiteSpace(), x => input.RoleIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (AuditTeamMemberRole)int.Parse(v.Trim()))
                    .Contains(x.Role))
                .WhereIf(input.RoleNot.HasValue, x => x.Role != (AuditTeamMemberRole)input.RoleNot.Value)
                .WhereIf(input.AuditId.HasValue, x => x.AuditId == input.AuditId.Value)
                .WhereIf(input.EmployeeId.HasValue, x => x.EmployeeId == input.EmployeeId.Value);
        }

        public override async Task<AuditTeamMemberDto> CreateAsync(CreateAuditTeamMemberDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "AuditTeamMember", result);
            return result;
        }

        public override async Task<AuditTeamMemberDto> UpdateAsync(AuditTeamMemberDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "AuditTeamMember", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "AuditTeamMember", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.AuditTeamMember_Read)]
        public List<GroupCountDto> GetGroupedCount(AuditTeamMemberGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Role", "AuditId", "EmployeeId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "Role":
                    return query
                        .GroupBy(x => x.Role)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                case "AuditId":
                    return query
                        .GroupBy(x => new { Key = x.AuditId, Label = x.Audit == null ? null : x.Audit.AuditNumber })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key == null ? null : g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "EmployeeId":
                    return query
                        .GroupBy(x => new { Key = x.EmployeeId, Label = x.Employee == null ? null : x.Employee.FullName })
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

    }
}
