using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace TskMngmntSys.Controllers
{
    public abstract class TskMngmntSysControllerBase: AbpController
    {
        protected TskMngmntSysControllerBase()
        {
            LocalizationSourceName = TskMngmntSysConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
