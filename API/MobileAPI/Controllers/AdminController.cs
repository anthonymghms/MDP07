using DatabaseMigration.Model;
using DataContract.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServiceLayer.Authentication;
using ServiceLayer.EmailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DatabaseMigration;
using Microsoft.EntityFrameworkCore;

namespace MobileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly DrowsinessDetectionContext _drowsinessDetectionContext;

        public AdminController(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager, IEmailService emailService,
            SignInManager<AppUser> signInManager, IConfiguration configuration,
            DrowsinessDetectionContext drowsinessDetectionContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
            _drowsinessDetectionContext = drowsinessDetectionContext;
        }
        [ApiKeyAuthFilter]
        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] RegisterUserRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "Failed to delete user" });
                }
                return StatusCode(StatusCodes.Status200OK,
                new Response { Status = "Success", Message = "User deleted" });
            }
            return StatusCode(StatusCodes.Status403Forbidden,
                new Response { Status = "Error", Message = "User does not exist!" });

        }
        [ApiKeyAuthFilter]
        [HttpPost("PurgeAllUsers")]
        public IActionResult PurgeAllUsers()
        {
            var users = _drowsinessDetectionContext.Users;
            _drowsinessDetectionContext.RemoveRange(users);
            _drowsinessDetectionContext.SaveChanges();
            return StatusCode(StatusCodes.Status200OK,
                new Response { Status = "Success", Message = "Users purged Successfully" });
        }
        [HttpGet("Test")]
        public IActionResult Test()
        {
            return StatusCode(StatusCodes.Status200OK,
                new Response { Status = "Success", Message = "Admin API working" });
        }
    }
}
