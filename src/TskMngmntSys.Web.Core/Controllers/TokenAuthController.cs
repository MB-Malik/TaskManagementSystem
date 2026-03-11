using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
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
    [ApiController]
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
            if (model == null)
                throw new UserFriendlyException("Invalid request.");

            if (string.IsNullOrWhiteSpace(model.UserNameOrEmailAddress))
                throw new UserFriendlyException("Username or Email is required.");

            if (string.IsNullOrWhiteSpace(model.Password))
                throw new UserFriendlyException("Password is required.");

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

            if (loginResult.User == null)
                throw new UserFriendlyException("User login failed.");

            if (loginResult.User.IsTwoFactorEnabled)
            {
                return new AuthenticateResultModel
                {
                    RequiresTwoFactor = true,
                    UserId = loginResult.User.Id,
                    TenancyName = tenancyName
                };
            }

            if (!(loginResult.Identity is ClaimsIdentity claimsIdentity))
                throw new UserFriendlyException("Failed to create identity.");

            var jwtClaims = CreateJwtClaims(claimsIdentity);
            var accessToken = CreateAccessToken(jwtClaims);

            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                UserId = loginResult.User.Id,
                RequiresTwoFactor = false,
                TenancyName = tenancyName
            };
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<AuthenticateResultModel> VerifyTwoFactor([FromBody] Verify2FaDto input)
        {
            if (input == null)
                throw new UserFriendlyException("Invalid request.");

            if (input.UserId <= 0)
                throw new UserFriendlyException("Invalid user id.");

            if (string.IsNullOrWhiteSpace(input.Code))
                throw new UserFriendlyException("2FA code is required.");

            if (string.IsNullOrWhiteSpace(input.TenancyName))
                throw new UserFriendlyException("Tenancy name is required.");

            var tenant = _tenantCache.GetOrNull(input.TenancyName);

            if (tenant == null)
                throw new UserFriendlyException("Invalid tenancy name.");

            using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
            {
                var user = await _userManager.GetUserByIdAsync(input.UserId);

                if (user == null)
                    throw new UserFriendlyException("User not found.");

                var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                    user,
                    TokenOptions.DefaultAuthenticatorProvider,
                    input.Code
                );

                if (!isValid)
                    throw new UserFriendlyException("Invalid 2FA code.");

                var principal = await _claimsPrincipalFactory.CreateAsync(user);

                if (!(principal.Identity is ClaimsIdentity identity))
                    throw new UserFriendlyException("Failed to generate claims identity.");

                var accessToken = CreateAccessToken(CreateJwtClaims(identity));

                return new AuthenticateResultModel
                {
                    AccessToken = accessToken,
                    EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                    ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                    UserId = user.Id,
                    RequiresTwoFactor = false,
                    TenancyName = input.TenancyName
                };
            }
        }

        [HttpPost]
        [AbpAuthorize]
        public async Task<IActionResult> UpdateTwoFactor([FromBody] UpdateTwoFactorDto input)
        {
            if (input == null)
                throw new UserFriendlyException("Invalid request.");

            var currentUserId = AbpSession.GetUserId();
            var tenantId = AbpSession.TenantId;

            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var user = await _userManager.GetUserByIdAsync(currentUserId);

                if (user == null)
                    throw new AbpAuthorizationException("Unauthorized access.");

                user.IsTwoFactorEnabled = input.IsTwoFactorEnabled;

                if (input.IsTwoFactorEnabled)
                {
                    await _userManager.ResetAuthenticatorKeyAsync(user);
                }

                await _userManager.UpdateAsync(user);
            }

            return Ok(new
            {
                Success = true,
                UserId = currentUserId,
                TwoFactorEnabled = input.IsTwoFactorEnabled
            });
        }

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
                return null;

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();

            var nameIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (nameIdClaim == null)
                throw new UserFriendlyException("Invalid identity claims.");

            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            });

            return claims;
        }

        private string GetEncryptedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken);
        }
    }
}