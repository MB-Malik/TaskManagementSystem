using System.Threading.Tasks;
using TskMngmntSys.Models.TokenAuth;
using TskMngmntSys.Web.Controllers;
using Shouldly;
using Xunit;

namespace TskMngmntSys.Web.Tests.Controllers
{
    public class HomeController_Tests: TskMngmntSysWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}