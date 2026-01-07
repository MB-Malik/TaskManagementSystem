using System.Threading.Tasks;
using Abp.Application.Services;
using TskMngmntSys.Sessions.Dto;

namespace TskMngmntSys.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
