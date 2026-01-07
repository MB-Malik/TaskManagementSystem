using System.Threading.Tasks;
using TskMngmntSys.Configuration.Dto;

namespace TskMngmntSys.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
