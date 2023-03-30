using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthFilter]
    public class EmergencyContactController : ControllerBase
    {

    }
}
