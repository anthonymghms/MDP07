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

        public AdminController(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager, IEmailService emailService,
            SignInManager<AppUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
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
    }
}
