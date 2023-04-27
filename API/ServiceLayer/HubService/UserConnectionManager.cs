using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.HubService
{
    public class UserConnectionManager
    {
        private readonly ConcurrentDictionary<string, string> _userConnections;
        public UserConnectionManager() {
            _userConnections = new ConcurrentDictionary<string, string>();
        }
        public void AddConnection(string userId, string connectionId)
        {
            _userConnections.AddOrUpdate(userId, connectionId, (key, oldValue) => connectionId);
        }

        public void RemoveConnection(string userId)
        {
            _userConnections.Remove(userId, out _);
        }

        public string GetConnectionId(string userId)
        {
            _userConnections.TryGetValue(userId, out string connectionId);
            return connectionId;
        }
    }
}
