using Common;
using DatabaseMigration;
using DatabaseMigration.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Authentication;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MobileAPI.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class EmergencyContactController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DrowsinessDetectionContext _dbContext;
        public EmergencyContactController(UserManager<AppUser> userManager, DrowsinessDetectionContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddEmergencyContact(string ecUsername)
        {
            try
            {
                var username = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);
                var emergencyContact = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == ecUsername);
                var isAlreadyEmergencyContact = await _dbContext.EmergencyContact.FirstOrDefaultAsync(x => x.EmergencyContactUserId == Guid.Parse(emergencyContact.Id) && x.UserId == user.Id);
                if (isAlreadyEmergencyContact != null) return StatusCode(409, new JsonResult(new { Success = false, message = "User is already an emergency contact" }));
                
                var emergencyContactId = Guid.NewGuid();
                _dbContext.EmergencyContact.Add(new EmergencyContact
                {
                    Id = emergencyContactId,
                    UserId = user.Id,
                    EmergencyContactUserId = Guid.Parse(emergencyContact.Id),
                    User = user,
                    CreationDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    IsDeleted = false,
                    IsDisabled = false
                });
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
            }
            return Ok(new JsonResult(new { Success = true, Message = "Emergency Contact Added Successfully" }));
        }
        [HttpPost("Remove")]
        public async Task<IActionResult> RemoveEmergencyContact(string ecUsername)
        {
            try
            {
                var username = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);

                var emergencyContact = _dbContext.Users.FirstOrDefault(x => x.UserName == ecUsername);
                var emergencyContactId = _dbContext.EmergencyContact.FirstOrDefault(x => x.EmergencyContactUserId == Guid.Parse(emergencyContact.Id) && x.UserId == user.Id).Id;

                var emergencyContactToRemove = _dbContext.EmergencyContact.FirstOrDefault(x => x.Id == emergencyContactId);
                _dbContext.EmergencyContact.Remove(emergencyContactToRemove);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
            }
            return Ok(new JsonResult(new { Success = true, Message = "Emergency Contact Removed Successfully" }));
        }
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _dbContext.Users.ToListAsync();
                return Ok(new JsonResult(new { Success = true, Message = "Users Retrieved Successfully", Data = users }));
            }
            catch (Exception e)
            {
                return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
            }
        }
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(string search)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == search || x.PhoneNumber == search);
                if (user == null) return NotFound(new Response { Status = "Not Found", Message = $"User with username or phone number {search} was not found" });
                return Ok(new JsonResult(new { Success = true, Message = "User Retrieved Successfully", Data = new { user.FirstName, user.LastName, user.UserName, user.PhoneNumber }}));
            }
            catch (Exception e)
            {
                return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
            }
        }
        [HttpGet("GetRoadGuards")]
        public async Task<IActionResult> GetRoadGuards()
        {
            try
            {
                var username = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);
                var emergencyContacts = _dbContext.EmergencyContact.Where(x => x.UserId == user.Id).ToList();
                var emergencyContactUsers = new List<object>();
                foreach (var emergencyContact in emergencyContacts)
                {
                    var emergencyContactUser = _dbContext.Users.FirstOrDefault(x => x.Id == emergencyContact.EmergencyContactUserId.ToString());
                    emergencyContactUsers.Add(new {emergencyContactUser.UserName, emergencyContactUser.FirstName, emergencyContactUser.LastName, emergencyContactUser.PhoneNumber });
                }
                return Ok(new JsonResult(new { Success = true, Message = "Emergency Contacts Retrieved Successfully", Data = emergencyContactUsers }));
            }
            catch (Exception e)
            {
                return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
            }
        }
        [HttpGet("GetRoadGuardees")]
        public async Task<IActionResult> GetRoadGuardees()
        {
            try
            {
                var username = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);
                var emergencyContacts = _dbContext.EmergencyContact.Where(x => x.EmergencyContactUserId == Guid.Parse(user.Id)).ToList();
                var emergencyContactUsers = new List<AppUser>();
                foreach (var emergencyContact in emergencyContacts)
                {
                    var emergencyContactUser = _dbContext.Users.FirstOrDefault(x => x.Id == emergencyContact.UserId);
                    emergencyContactUsers.Add(emergencyContactUser);
                }
                return Ok(new JsonResult(new { Success = true, Message = "Emergency Contacts Retrieved Successfully", Data = emergencyContactUsers }));
            }
            catch (Exception e)
            {
                return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
            }
        }
    }
}
