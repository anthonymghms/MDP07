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
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DrowsinessDetectionContext _context;
        public UserController(UserManager<AppUser> userManager, DrowsinessDetectionContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("EnableTwoFactorAuth")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        public async Task<IActionResult> UpdateSettings([FromBody] UserSettingsRequest request)
        {
            try
            {
                var username = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);
                if(user == null) return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User not found" });
                user.UserConfig = new UserConfig
                {
                    UserId = user.Id,
                    User = user,
                    AlertType = request.AlertType,
                    AlertVolume = request.AlertVolume,
                    LocationSharing = request.LocationSharing,
                    DarkMode = request.DarkMode,
                    NotificationsEnabled = request.NotificationsEnabled,
                    TwoFactorAuthEnabled = request.TwoFactorAuthEnabled,
                    Username = request.Username,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email
                };
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Settings updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ex.Message });
            }
        }
        //[HttpPost("AddEmergencyContact/{Id}")]
        //public IActionResult AddEmergencyContact([FromRoute] Guid Id, [FromBody] EmergencyContactRequest request)
        //{
        //    try
        //    {
        //        var user = _dbContext.Users.FirstOrDefault(x => x.UserId == Id);
        //        var emergencyContactId = Guid.NewGuid();
        //        user.EmergencyContacts.Add(new UserEmergencyContact
        //        {
        //            UserId = Id,
        //            User = user,
        //            EmergencyContactId = emergencyContactId,
        //            EmergencyContact = new EmergencyContact
        //            {
        //                EmergencyContactId = emergencyContactId,
        //                EmergencyContactUserId = request.EmergencyContactUserId,
        //                CreationDate = DateTime.Now,
        //                LastModifiedDate = DateTime.Now,
        //                IsDeleted = false,
        //                IsDisabled = false
        //            }
        //        });
        //        _dbContext.Entry(user).State = EntityState.Modified;
        //        _dbContext.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
        //    }
        //    return Ok(new JsonResult(new { Success = true, Message = "Emergency Contact Added Successfully" }));
        //}
    }
}
