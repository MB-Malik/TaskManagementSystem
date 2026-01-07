using Abp.Authorization;
using TskMngmntSys.Authorization.Roles;
using TskMngmntSys.Authorization.Users;

namespace TskMngmntSys.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
