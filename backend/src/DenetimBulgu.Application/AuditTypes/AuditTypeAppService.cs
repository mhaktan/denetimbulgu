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
using DenetimBulgu.AuditTypes.Dto;
using DenetimBulgu.Analytics.Dto;
using DenetimBulgu.Audits.Dto;
using DenetimBulgu.Approvals.Dto;
using DenetimBulgu.Authorization;
using DenetimBulgu.Flows;

namespace DenetimBulgu.AuditTypes
{
    public class AuditTypeAppService : AsyncCrudAppService<
        AuditType,
        AuditTypeDto,
        long,
        PagedAuditTypeResultRequestDto,
        CreateAuditTypeDto,
        AuditTypeDto>,
        IAuditTypeAppService
    {
        private readonly IFlowEngine _flowEngine;

        public AuditTypeAppService(IRepository<AuditType, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.AuditType_Read;
            GetAllPermissionName = PermissionNames.AuditType_Read;
            CreatePermissionName = PermissionNames.AuditType_Create;
            UpdatePermissionName = PermissionNames.AuditType_Update;
            DeletePermissionName = PermissionNames.AuditType_Delete;
        }

        protected override IQueryable<AuditType> CreateFilteredQuery(PagedAuditTypeResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Code != null && x.Code.Contains(input.Keyword)) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)))
                .WhereIf(!input.Code.IsNullOrWhiteSpace(), x => x.Code != null && x.Code.Contains(input.Code))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description));
        }

        public override async Task<AuditTypeDto> CreateAsync(CreateAuditTypeDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "AuditType", result);
            return result;
        }

        public override async Task<AuditTypeDto> UpdateAsync(AuditTypeDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "AuditType", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "AuditType", new { Id = input.Id });
        }
        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.AuditType_Read)]
        public async Task<AuditTypeReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.Audits)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new AuditTypeReportDto
            {
                Data = ObjectMapper.Map<AuditTypeDto>(root),
                Audits = ObjectMapper.Map<List<AuditDto>>(
                    root.Audits == null ? new List<Audit>() : root.Audits.ToList()),
            };
        }

    }
}
