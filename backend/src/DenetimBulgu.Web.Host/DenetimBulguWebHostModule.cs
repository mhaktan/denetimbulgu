using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using DenetimBulgu.EntityFrameworkCore;

namespace DenetimBulgu.Web.Host
{
    [DependsOn(typeof(DenetimBulguApplicationModule), typeof(DenetimBulguEntityFrameworkCoreModule), typeof(AbpAspNetCoreModule))]
    public class DenetimBulguWebHostModule : AbpModule
    {
        public override void PreInitialize()
        {
            // Expose all AppServices as dynamic API controllers
            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(DenetimBulguApplicationModule).GetAssembly(),
                    moduleName: "app",
                    useConventionalHttpVerbs: true
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DenetimBulguWebHostModule).GetAssembly());
        }
    }
}
