using Abp.Application.Services;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TskMngmntSys.Authentication.Dto;
using TskMngmntSys.Authorization.Users;

namespace TskMngmntSys.Authentication
{
    public class TwoFactorAppService : ApplicationService
    {
        private readonly UserManager _userManager;

        public TwoFactorAppService(UserManager userManager)
        {
            _userManager = userManager;
        }
        public async Task<string> GetAuthenticatorSetupInfo()
        {
            var userId = AbpSession.GetUserId();
            var user = await _userManager.GetUserByIdAsync(userId);

            await _userManager.ResetAuthenticatorKeyAsync(user);
            var key = await _userManager.GetAuthenticatorKeyAsync(user);

            var appName = "TskMngmntSys";
            var email = user.EmailAddress;

            var qrCodeUri =
                $"otpauth://totp/{appName}:{email}?secret={key}&issuer={appName}";

            return qrCodeUri;
        }

        public async Task EnableAuthenticator(EnableAuthenticatorDto input)
        {
            var userId = AbpSession.GetUserId();
            var user = await _userManager.GetUserByIdAsync(userId);

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                TokenOptions.DefaultAuthenticatorProvider,
                input.Code
            );

            if (!isValid)
            {
                throw new UserFriendlyException("Invalid authentication code");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
        }

    }
}
