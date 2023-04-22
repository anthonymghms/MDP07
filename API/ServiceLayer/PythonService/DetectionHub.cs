using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
namespace ServiceLayer.PythonService
{
    public class DetectionHub : Hub
    {
        public async Task SendDetectionResult(string result)
        {
            await Clients.All.SendAsync("DetectionResult", result);
        }
    }

}
