using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ServiceLayer.PythonService;
using ServiceLayer.Authentication;
using Microsoft.AspNetCore.SignalR;
using Common;
using DatabaseMigration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DatabaseMigration.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace MobileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthFilter]
    public class PythonController : ControllerBase
    {
        private readonly IPythonService _pythonService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<DetectionHub> _hubContext;
        private readonly DrowsinessDetectionContext _context;
        public PythonController(IPythonService pythonService, IHubContext<DetectionHub> hubContext,
            DrowsinessDetectionContext context, UserManager<AppUser> userManager)
        {
            _pythonService = pythonService;
            _hubContext = hubContext;
            _context = context;
            _userManager = userManager;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpGet("StartDetection")]
        public async Task<IActionResult> StartDetection()
        {
            var username = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            var userSettings = await _context.UserConfig.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var ipCamAddress = userSettings.IpCamAddress;
            var earThreshold = "";
            var waitTime = "";
            if(userSettings?.AlertLevel == "Low")
            {
                earThreshold = "0.12";
                waitTime = "1.5";
            }
            else if (userSettings?.AlertLevel == "Moderate")
            {
                earThreshold = "0.18";
                waitTime = "1.2";
            }
            else if(userSettings?.AlertLevel == "High")
            {
                earThreshold = "0.21";
                waitTime = "0.8";
            }
            await _pythonService.StartExecutionAsync(ipCamAddress, earThreshold, waitTime);
            return Ok(new Response{Status = "Success", Message = "Started detecting" });
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage(string message)
        {
            try
            {
                await _pythonService.SendMessage(message);
                return Ok("Message sent");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
