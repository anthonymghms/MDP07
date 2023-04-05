using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLayer.PythonService;
using ServiceLayer.Authentication;

namespace MobileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthFilter]
    public class PythonController : ControllerBase
    {
        private PythonService _pythonService;

        public PythonController(PythonService pythonService)
        {
            _pythonService = pythonService;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            string scriptPath = "path/to/python/script.py";
            var task = _pythonService.ExecuteAsync(scriptPath);
            task.Wait();

            if (task.Status == TaskStatus.RanToCompletion)
            {
                string output = task.Result;
                return Ok(output);
            }
            else
            {
                return StatusCode(500);
            }
        }
    }
}
