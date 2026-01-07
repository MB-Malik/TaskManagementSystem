using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using TskMngmntSys.Configuration;

namespace TskMngmntSys.Web.Host.Startup
{
    [DependsOn(
       typeof(TskMngmntSysWebCoreModule))]
    public class TskMngmntSysWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public TskMngmntSysWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(TskMngmntSysWebHostModule).GetAssembly());
        }
    }
}
