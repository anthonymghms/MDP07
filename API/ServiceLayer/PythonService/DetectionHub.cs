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
        private readonly UserConnectionManager _userConnectionManager;

        public DetectionHub(UserConnectionManager userConnectionManager)
        {
            _userConnectionManager = userConnectionManager;
        }

        public override async Task OnConnectedAsync()
        {
            string userId = Context.GetHttpContext().Request.Query["userId"];
            _userConnectionManager.AddConnection(userId, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userId = Context.GetHttpContext().Request.Query["userId"];
            _userConnectionManager.RemoveConnection(userId);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToUser(string userId, string message)
        {
            string connectionId = _userConnectionManager.GetConnectionId(userId);

            if (!string.IsNullOrEmpty(connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
            }
        }
        public async Task SendDetectionResult(string result)
        {
            await Clients.All.SendAsync("DetectionResult", result);
        }
    }

}
