using DatabaseMigration.Model;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MobileAPI.Auth
{
    public class CustomEmailTokenProvider: IUserTwoFactorTokenProvider<AppUser>
    {
        private readonly IDataProtector _protector;
        private readonly IDistributedCache _cache;
        private readonly ILogger<CustomEmailTokenProvider> _logger;


        public CustomEmailTokenProvider(IDataProtectionProvider dataProtectionProvider, IDistributedCache cache, ILogger<CustomEmailTokenProvider> logger)
        {
            _protector = dataProtectionProvider.CreateProtector("CustomEmailTokenProvider");
            _cache = cache;
            _logger = logger;
        }

        public Task<string> GenerateAsync(string purpose, UserManager<AppUser> manager, AppUser user)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            var userId = user.UserName;
            var expiration = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds();
            var protectedData = _protector.Protect($"{userId}:{otp}:{expiration}");
             _cache.SetStringAsync(otp, protectedData);

            _logger.LogInformation($"Token: {otp}, User ID: {userId}");

            return Task.FromResult(otp);
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<AppUser> manager, AppUser user)
        {
            _logger.LogInformation($"Token: {token}, User ID: {user.UserName}");
            return Task.FromResult(true);
        }

        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<AppUser> manager, AppUser user)
        {
            return Task.FromResult(true);
        }
    }


}
