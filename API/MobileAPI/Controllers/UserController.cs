using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DataContract.Request;
using DatabaseMigration;
using ServiceLayer.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using DatabaseMigration.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Common;

namespace MobileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DrowsinessDetectionContext _dbContext;
        public UserController(UserManager<AppUser> userManager, DrowsinessDetectionContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpPost("EnableTwoFactorAuth")]
        public async Task<IActionResult> EnableTwoFactorAuth(bool enableTwoFactorAuth)
        {
            try
            {
                var username = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);
                if(user == null) return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User not found" });
                user.TwoFactorEnabled = enableTwoFactorAuth;
                await _userManager.UpdateAsync(user);
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = enableTwoFactorAuth ? "Two-Factor Auth was enabled" : "Two-Factor Auth was disabled" });
            } 
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }

        }

        [HttpPost("UpdateSettings")]
        public async Task<IActionResult> UpdateSettings([FromBody] UserSettingsRequest request)
        {
            try
            {
                var username = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);
                if(user == null) return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User not found" });
                var userSettings = await _dbContext.UserConfig.FirstOrDefaultAsync(x => x.UserId == user.Id);
                userSettings.AlertType = request?.AlertType ?? userSettings.AlertType;
                userSettings.AlertVolume = request?.AlertVolume ?? userSettings.AlertVolume;
                userSettings.LocationSharing = request?.LocationSharing ?? userSettings.LocationSharing;
                userSettings.DarkMode = request?.DarkMode ?? userSettings.DarkMode;
                userSettings.NotificationsEnabled = request?.NotificationsEnabled ?? userSettings.NotificationsEnabled;
                userSettings.TwoFactorAuthEnabled = request?.TwoFactorAuthEnabled ?? userSettings.TwoFactorAuthEnabled;
                userSettings.Username = request?.Username ?? userSettings.Username;
                userSettings.FirstName = request?.FirstName ?? userSettings.FirstName;
                userSettings.LastName = request?.LastName ?? userSettings.LastName;
                userSettings.PhoneNumber = request?.PhoneNumber ?? userSettings.PhoneNumber;
                userSettings.Email = request?.Email ?? user.Email;
                userSettings.AlertLevel = request?.AlertLevel ?? userSettings.AlertLevel;
                userSettings.IpCamAddress = request?.IpCamAddress ?? userSettings.IpCamAddress;
                userSettings.IpEspAddress = request?.IpEspAddress ?? userSettings.IpEspAddress;

                _dbContext.UserConfig.Update(userSettings);
                await _dbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Settings updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }
        [HttpGet("GetSettings")]
        public async Task<IActionResult> GetUserSettings()
        {
            try
            {
                var username = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);
                if(user == null) return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User not found" });
                var settings = _dbContext.UserConfig.FirstOrDefaultAsync(x => x.UserId == user.Id);
                return StatusCode(StatusCodes.Status200OK, new JsonResult(new { Status = "Success", Message = "Settings retrieved successfully", Data = settings }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }
    }
}
