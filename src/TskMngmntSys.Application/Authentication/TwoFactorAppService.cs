using Abp.Application.Services;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using QRCoder;
using System;
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

        public async Task<AuthenticatorSetupDto> GetAuthenticatorSetupInfo()
        {
            var userId = AbpSession.GetUserId();

            if (userId <= 0)
                throw new UserFriendlyException("Invalid user session.");

            var user = await _userManager.GetUserByIdAsync(userId);

            if (user == null)
                throw new UserFriendlyException("User not found.");

            if (user.IsTwoFactorEnabled)
                throw new UserFriendlyException("Two-factor authentication is already enabled.");

            var key = await _userManager.GetAuthenticatorKeyAsync(user);

            if (string.IsNullOrWhiteSpace(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var appName = "TskMngmntSys";
            var email = Uri.EscapeDataString(user.EmailAddress);

            var authenticatorUri =
                $"otpauth://totp/{appName}:{email}?secret={key}&issuer={appName}&digits=6";

            var qrCodeBase64 = GenerateQrCode(authenticatorUri);

            return new AuthenticatorSetupDto
            {
                SharedKey = key,
                AuthenticatorUri = authenticatorUri,
                QrCodeImage = qrCodeBase64
            };
        }

        public async Task EnableAuthenticator(EnableAuthenticatorDto input)
        {
            if (input == null)
                throw new UserFriendlyException("Invalid request.");

            if (string.IsNullOrWhiteSpace(input.Code))
                throw new UserFriendlyException("Authentication code is required.");

            if (input.Code.Length != 6)
                throw new UserFriendlyException("Authentication code must be 6 digits.");

            var userId = AbpSession.GetUserId();

            if (userId <= 0)
                throw new UserFriendlyException("Invalid user session.");

            var user = await _userManager.GetUserByIdAsync(userId);

            if (user == null)
                throw new UserFriendlyException("User not found.");

            if (user.IsTwoFactorEnabled)
                throw new UserFriendlyException("Two-factor authentication is already enabled.");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                TokenOptions.DefaultAuthenticatorProvider,
                input.Code
            );

            if (!isValid)
                throw new UserFriendlyException("Invalid authentication code.");

            await _userManager.SetTwoFactorEnabledAsync(user, true);
        }

        public async Task DisableTwoFactor()
        {
            var userId = AbpSession.GetUserId();

            if (userId <= 0)
                throw new UserFriendlyException("Invalid user session.");

            var user = await _userManager.GetUserByIdAsync(userId);

            if (user == null)
                throw new UserFriendlyException("User not found.");

            if (!user.IsTwoFactorEnabled)
                throw new UserFriendlyException("Two-factor authentication is already disabled.");

            await _userManager.SetTwoFactorEnabledAsync(user, false);
        }

        private string GenerateQrCode(string uri)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);

                using (var qrCode = new PngByteQRCode(qrData))
                {
                    var qrCodeBytes = qrCode.GetGraphic(20);

                    return "data:image/png;base64," + Convert.ToBase64String(qrCodeBytes);
                }
            }
        }
    }
}