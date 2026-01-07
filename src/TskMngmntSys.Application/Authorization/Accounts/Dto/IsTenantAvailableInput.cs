using System.ComponentModel.DataAnnotations;
using Abp.MultiTenancy;

namespace TskMngmntSys.Authorization.Accounts.Dto
{
    public class IsTenantAvailableInput
    {
        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }
    }
}
