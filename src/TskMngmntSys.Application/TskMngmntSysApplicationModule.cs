using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using TskMngmntSys.Authorization;

namespace TskMngmntSys
{
    [DependsOn(
        typeof(TskMngmntSysCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class TskMngmntSysApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<TskMngmntSysAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(TskMngmntSysApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
