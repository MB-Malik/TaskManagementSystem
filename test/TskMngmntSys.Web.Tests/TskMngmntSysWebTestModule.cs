using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using TskMngmntSys.EntityFrameworkCore;
using TskMngmntSys.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace TskMngmntSys.Web.Tests
{
    [DependsOn(
        typeof(TskMngmntSysWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class TskMngmntSysWebTestModule : AbpModule
    {
        public TskMngmntSysWebTestModule(TskMngmntSysEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(TskMngmntSysWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(TskMngmntSysWebMvcModule).Assembly);
        }
    }
}