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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
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
                userSettings = new UserConfig
                {
                    UserId = user.Id,
                    User = user,
                    AlertType = request?.AlertType ?? user.UserConfig.AlertType,
                    AlertVolume = request?.AlertVolume ?? user.UserConfig.AlertVolume,
                    LocationSharing = request?.LocationSharing ?? user.UserConfig.LocationSharing,
                    DarkMode = request?.DarkMode ?? user.UserConfig.DarkMode,
                    NotificationsEnabled = request?.NotificationsEnabled ?? user.UserConfig.NotificationsEnabled,
                    TwoFactorAuthEnabled = request?.TwoFactorAuthEnabled ?? user.UserConfig.TwoFactorAuthEnabled,
                    Username = request?.Username ?? user.UserConfig.Username,
                    FirstName = request?.FirstName ?? user.UserConfig.FirstName,
                    LastName = request?.LastName ?? user.UserConfig.LastName,
                    PhoneNumber = request?.PhoneNumber ?? user.UserConfig.PhoneNumber,
                    Email = request?.Email ?? user.UserConfig.Email
                };
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
