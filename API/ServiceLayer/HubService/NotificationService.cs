using Common;
using DatabaseMigration;
using DatabaseMigration.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer.HubService
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<DetectionHub> _hubContext;
        private readonly UserConnectionManager _userConnectionManager;
        private readonly DrowsinessDetectionContext _context;

        public NotificationService(IHubContext<DetectionHub> hubContext, UserConnectionManager userConnectionManager)
        {
            _hubContext = hubContext;
            _userConnectionManager = userConnectionManager;
        }

        public async Task SendMessageToEmergencyContacts(string userId, string message)
        {
            var emergencyContacts = _context.EmergencyContact.Where(e => e.UserId == userId).ToList();
            foreach (var contact in emergencyContacts)
            {
                //_context.AlertMessage.Add(new AlertMessage
                //{
                //    Id = Guid.NewGuid(),
                //    UserId = userId,
                //    ToUserId = contact.UserId,
                //    MessageDateTime = DateTime.Now,
                //    MessageText = message,
                //    IsRead = false
                //});
                string connectionId = _userConnectionManager.GetConnectionId(contact.UserId);
                if (!string.IsNullOrEmpty(connectionId))
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("Notification", message);
                }
            }
        }

        public async Task SendDetectionResult(string userId, string result)
        {
            //await _context.UserAlert.AddAsync(new UserAlert
            //{
            //    Id = Guid.NewGuid(),
            //    UserId = userId,
            //    AlertMessage = $"Detected {result}",
            //    AlertSeverity = "High",
            //    AlertDateTime = DateTime.Now,
            //});
            //await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("DetectionResult", result);
            string connectionId = _userConnectionManager.GetConnectionId(userId);
            if (!string.IsNullOrEmpty(connectionId))
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("DetectionResult", result);
            }
        }
    }
}
