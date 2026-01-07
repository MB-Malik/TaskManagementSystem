using Abp.MultiTenancy;
using TskMngmntSys.Authorization.Users;

namespace TskMngmntSys.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
