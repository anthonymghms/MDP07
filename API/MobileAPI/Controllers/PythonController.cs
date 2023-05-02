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
using ServiceLayer.HubService;

namespace MobileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthFilter]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    public class PythonController : ControllerBase
    {
        private readonly IPythonService _pythonService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<DetectionHub> _hubContext;
        private readonly DrowsinessDetectionContext _context;
        private readonly INotificationService _notificationService;
        public PythonController(IPythonService pythonService, IHubContext<DetectionHub> hubContext,
            DrowsinessDetectionContext context, UserManager<AppUser> userManager,
            INotificationService notificationService)
        {
            _pythonService = pythonService;
            _hubContext = hubContext;
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }
        [HttpGet("StartDetection")]
        public async Task<IActionResult> StartDetection()
        {
            var username = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            var userSettings = await _context.UserConfig.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var ipCamAddress = userSettings.IpCamAddress;
            var ipEspAddress = userSettings.IpEspAddress;
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
            await _pythonService.StartExecutionAsync(ipCamAddress, earThreshold, waitTime, user.Id, ipEspAddress);
            return Ok(new Response{Status = "Success", Message = "Started detecting" });
        }
        [HttpGet("StopDetection")]
        public async Task<IActionResult> StopDetection()
        {
            var username = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            var userSettings = await _context.UserConfig.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var ipCamAddress = userSettings.IpCamAddress;
            var ipEspAddress = userSettings.IpEspAddress;
            await _pythonService.StopExecutionAsync(ipEspAddress);
            return Ok(new Response { Status = "Success", Message = "Stopped detecting" });
        }
        [HttpGet("RestartDetection")]
        public async Task<IActionResult> RestartDetection()
        {
            var username = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            var userSettings = await _context.UserConfig.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var ipCamAddress = userSettings.IpCamAddress;
            var ipEspAddress = userSettings.IpEspAddress;
            var earThreshold = "";
            var waitTime = "";
            if (userSettings?.AlertLevel == "Low")
            {
                earThreshold = "0.12";
                waitTime = "1.5";
            }
            else if (userSettings?.AlertLevel == "Moderate")
            {
                earThreshold = "0.18";
                waitTime = "1.2";
            }
            else if (userSettings?.AlertLevel == "High")
            {
                earThreshold = "0.21";
                waitTime = "0.8";
            }
            await _pythonService.RestartExecutionAsync(ipCamAddress, earThreshold, waitTime, user.Id, ipEspAddress);
            return Ok(new Response { Status = "Success", Message = "Restarted detecting" });
        }
        [HttpGet("StopVibration")]
        public async Task<IActionResult> StopVibration()
        {
            var username = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            var userSettings = await _context.UserConfig.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var ipEspAddress = userSettings.IpEspAddress;
            await _pythonService.StopVibration(ipEspAddress);
            return Ok(new Response { Status = "Sucess", Message = $"Stopped Vibrating" });
        }
    } 
}
