using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace TskMngmntSys.Authorization
{
    public class TskMngmntSysAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

            var tasks = context.CreatePermission("Pages.Tasks", L("Tasks"));

            tasks.CreateChildPermission("Pages.Tasks.Create", L("CreateTask"));
            tasks.CreateChildPermission("Pages.Tasks.Edit", L("EditTask"));
            tasks.CreateChildPermission("Pages.Tasks.Delete", L("DeleteTask"));
            tasks.CreateChildPermission("Pages.Tasks.View", L("ViewTask"));

        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, TskMngmntSysConsts.LocalizationSourceName);
        }
    }
}
