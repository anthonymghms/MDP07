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

        [HttpPost("Add/{Id}")]
        public async Task<IActionResult> AddEmergencyContact([FromRoute] Guid Id)
        {
            try
            {
                var username = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);
                var emergencyContact = _dbContext.Users.FirstOrDefault(x => x.Id == Id.ToString());
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
        [HttpPost("Remove/{Id}")]
        public async Task<IActionResult> RemoveEmergencyContact([FromRoute] Guid Id)
        {
            try
            {
                var username = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);
                var emergencyContact = _dbContext.Users.FirstOrDefault(x => x.Id == Id.ToString());
                var emergencyContactId = Guid.NewGuid();
                var emergencyContactToRemove = _dbContext.EmergencyContact.FirstOrDefault(x => x.EmergencyContactUserId == Id && x.UserId == user.Id);
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [HttpGet("GetMyEmergencyContacts")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetMyEmergencyContacts()
        {
            try
            {
                var username = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);
                var emergencyContacts = _dbContext.EmergencyContact.Where(x => x.UserId == user.Id).ToList();
                var emergencyContactUsers = new List<AppUser>();
                foreach (var emergencyContact in emergencyContacts)
                {
                    var emergencyContactUser = _dbContext.Users.FirstOrDefault(x => x.Id == emergencyContact.EmergencyContactUserId.ToString());
                    emergencyContactUsers.Add(emergencyContactUser);
                }
                return Ok(new JsonResult(new { Success = true, Message = "Emergency Contacts Retrieved Successfully", Data = emergencyContactUsers }));
            }
            catch (Exception e)
            {
                return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
            }
        }
        [HttpGet("GetUsersWhereIAmEmergencyContact")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUsersWhereIAmEmergencyContact()
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
