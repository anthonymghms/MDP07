using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLayer.PythonService;
using ServiceLayer.Authentication;
using Microsoft.AspNetCore.SignalR;

namespace MobileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthFilter]
    public class PythonController : ControllerBase
    {
        private IPythonService _pythonService;
        private readonly IHubContext<DetectionHub> _hubContext;

        public PythonController(IPythonService pythonService, IHubContext<DetectionHub> hubContext)
        {
            _pythonService = pythonService;
            _hubContext = hubContext;
        }

        [HttpPost("StartDetection")]
        public async Task<IActionResult> StartDetection()
        {
            await _pythonService.StartExecutionAsync();
            return Ok("Started detecting");
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
