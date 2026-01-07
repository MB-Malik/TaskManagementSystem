using System.Threading.Tasks;
using Abp.Application.Services;
using TskMngmntSys.Authorization.Accounts.Dto;

namespace TskMngmntSys.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
