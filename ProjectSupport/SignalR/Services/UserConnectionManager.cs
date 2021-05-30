using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.SignalR.Services
{
    public class UserConnectionManager : IUserConnectionManager
    {
        private static ConcurrentDictionary<string, List<string>> userConnectionMap = new ConcurrentDictionary<string, List<string>>();

        public void KeepUserConnection(string userId, string connectionId)
        {

            if (!userConnectionMap.ContainsKey(userId))
            {
                userConnectionMap[userId] = new List<string>();
            }
            userConnectionMap[userId].Add(connectionId);

        }
        public void RemoveUserConnection(string connectionId)
        {
            //This method will remove the connectionId of user

            foreach (var userId in userConnectionMap.Keys)
            {
                if (userConnectionMap.ContainsKey(userId))
                {
                    if (userConnectionMap[userId].Contains(connectionId))
                    {
                        userConnectionMap[userId].Remove(connectionId);
                        break;
                    }
                }
            }

        }
        public List<string> GetUserConnections(string userId)
        {
            var conn = new List<string>();

            var success = userConnectionMap.TryGetValue(userId, out conn);
            return success ? conn : null;
        }
    }
}
