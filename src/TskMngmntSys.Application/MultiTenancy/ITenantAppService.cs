using Abp.Application.Services;
using TskMngmntSys.MultiTenancy.Dto;

namespace TskMngmntSys.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

