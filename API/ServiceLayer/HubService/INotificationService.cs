using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.HubService
{
    public interface INotificationService
    {
        Task SendMessageToEmergencyContacts(string userId, string message);

        Task SendDetectionResult(string userId, string result);
    }
}
