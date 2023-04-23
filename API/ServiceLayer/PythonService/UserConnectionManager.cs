using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.PythonService
{
    public class UserConnectionManager
    {
        private readonly ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();

        public void AddConnection(string userId, string connectionId)
        {
            _userConnections.TryAdd(userId, connectionId);
        }

        public void RemoveConnection(string userId)
        {
            _userConnections.TryRemove(userId, out _);
        }

        public string GetConnectionId(string userId)
        {
            _userConnections.TryGetValue(userId, out string connectionId);
            return connectionId;
        }
    }
}
