using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TskMngmntSys.Authentication.Dto;
using TskMngmntSys.Authentication.JwtBearer;
using TskMngmntSys.Authorization;
using TskMngmntSys.Authorization.Users;
using TskMngmntSys.Models.TokenAuth;
using TskMngmntSys.MultiTenancy;

namespace TskMngmntSys.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TokenAuthController : TskMngmntSysControllerBase
    {
        private readonly LogInManager _logInManager;
        private readonly ITenantCache _tenantCache;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly TokenAuthConfiguration _configuration;
        private readonly UserManager _userManager;
        private readonly UserClaimsPrincipalFactory _claimsPrincipalFactory;
        private readonly IUnitOfWorkManager _unitOfWorkManager;


        public TokenAuthController(
            LogInManager logInManager,
            ITenantCache tenantCache,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            TokenAuthConfiguration configuration,
            UserManager userManager,
            UserClaimsPrincipalFactory claimsPrincipalFactory,
             IUnitOfWorkManager unitOfWorkManager)
        {
            _logInManager = logInManager;
            _tenantCache = tenantCache;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _configuration = configuration;
            _userManager = userManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _unitOfWorkManager = unitOfWorkManager;

        }
        [AllowAnonymous] 
        [HttpPost]
        public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
        {
            var tenancyName = model.TenancyName ?? GetTenancyNameOrNull();

            var loginResult = await _logInManager.LoginAsync(
                model.UserNameOrEmailAddress,
                model.Password,
                tenancyName
            );

            if (loginResult.Result != AbpLoginResultType.Success)
            {
                throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                    loginResult.Result,
                    model.UserNameOrEmailAddress,
                    tenancyName
                );
            }

            // ABP CLASSIC 2FA CHECK
            if (loginResult.User.IsTwoFactorEnabled)
            {
                return new AuthenticateResultModel
                {
                    RequiresTwoFactor = true,
                    UserId = loginResult.User.Id
                };
            }

            var claimsIdentity = loginResult.Identity as ClaimsIdentity;
            var accessToken = CreateAccessToken(claimsIdentity.Claims);

            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                UserId = loginResult.User.Id,
                RequiresTwoFactor = false
            };
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<AuthenticateResultModel> VerifyTwoFactor([FromBody] Verify2FaDto input)
        {
            using (_unitOfWorkManager.Current.SetTenantId(
         _tenantCache.Get(input.TenancyName)?.Id))
            {
                var user = await _userManager.GetUserByIdAsync(input.UserId);

                var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                    user,
                    TokenOptions.DefaultAuthenticatorProvider,
                    input.Code
                );

                if (!isValid)
                {
                    throw new UserFriendlyException("Invalid 2FA code");
                }

                var identity = await _claimsPrincipalFactory.CreateAsync(user);
                var claimsIdentity = identity.Identity as ClaimsIdentity;

                var accessToken = CreateAccessToken(claimsIdentity.Claims);

                return new AuthenticateResultModel
                {
                    AccessToken = accessToken,
                    EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                    ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                    UserId = user.Id,
                    RequiresTwoFactor = false
                };
            }

        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Users)]
        public async Task<IActionResult> UpdateTwoFactor([FromBody] UpdateTwoFactorDto input)
        {
            if (input == null)
                return BadRequest("Invalid input");

            using (_unitOfWorkManager.Current.SetTenantId(input.TenantId))
            {
                var user = await _userManager.GetUserByIdAsync(input.UserId);
                if (user == null)
                    throw new Abp.AbpException($"There is no user with id: {input.UserId} in tenant: {input.TenantId}");

                if (!input.IsTwoFactorEnabled)
                {
                    user.IsTwoFactorEnabled = false;
                    await _userManager.ResetAuthenticatorKeyAsync(user); 
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    user.IsTwoFactorEnabled = true;
                    await _userManager.ResetAuthenticatorKeyAsync(user);
                    await _userManager.UpdateAsync(user);
                }

                return Ok(new { Success = true, UserId = user.Id, TwoFactorEnabled = user.IsTwoFactorEnabled });
            }
        }

        



        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }

        private string GetEncryptedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken);
        }
    }
}
