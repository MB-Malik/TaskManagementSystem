using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using TskMngmntSys.Configuration;
using TskMngmntSys.EntityFrameworkCore;
using TskMngmntSys.Migrator.DependencyInjection;

namespace TskMngmntSys.Migrator
{
    [DependsOn(typeof(TskMngmntSysEntityFrameworkModule))]
    public class TskMngmntSysMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public TskMngmntSysMigratorModule(TskMngmntSysEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(TskMngmntSysMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                TskMngmntSysConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(TskMngmntSysMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
