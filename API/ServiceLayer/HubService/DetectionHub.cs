using System;
using System.Threading.Tasks;
using DatabaseMigration.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace ServiceLayer.HubService
{
    public class DetectionHub : Hub
    {
        private readonly UserConnectionManager _userConnectionManager;
        private readonly UserManager<AppUser> _userManager;

        public DetectionHub(UserConnectionManager userConnectionManager, UserManager<AppUser> userManager)
        {
            _userConnectionManager = userConnectionManager;
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await _userManager.FindByNameAsync(Context.User.Identity.Name);
            var userId = user.Id;
            _userConnectionManager.AddConnection(userId, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await _userManager.FindByNameAsync(Context.User.Identity.Name);
            var userId = user.Id;
            _userConnectionManager.RemoveConnection(userId);

            await base.OnDisconnectedAsync(exception);
        }

    }

}
