using DatabaseMigration.Model;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace MobileAPI.Auth
{
    public class CustomEmailTokenProvider : IUserTwoFactorTokenProvider<AppUser>
    {
        private readonly IDistributedCache _cache;
        private readonly IDataProtector _protector;

        public CustomEmailTokenProvider(IDistributedCache cache, IDataProtector protector)
        {
            _cache = cache;
            _protector = protector;
        }

        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<AppUser> manager, AppUser user)
        {
            return Task.FromResult(true);
        }

        public Task<string> GenerateAsync(string purpose, UserManager<AppUser> manager, AppUser user)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            var protectedOtp = _protector.Protect(otp);
            return Task.FromResult(protectedOtp);
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<AppUser> manager, AppUser user)
        {
            // validate the otp
            return Task.FromResult(true);
        }
    }
}
