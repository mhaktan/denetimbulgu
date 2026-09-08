using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace DenetimBulgu
{
    [DependsOn(typeof(DenetimBulguCoreModule), typeof(AbpAutoMapperModule))]
    public class DenetimBulguApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg =>
            {
                cfg.AddMaps(typeof(DenetimBulguApplicationModule).GetAssembly());
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DenetimBulguApplicationModule).GetAssembly());
        }
    }
}
