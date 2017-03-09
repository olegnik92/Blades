using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blades.Web
{
    public class ClientConnectionsManager
    {
        public static ClientConnectionsManager Instance = new ClientConnectionsManager();

        private ClientConnectionsManager()
        {
            this.allConnections = new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, ClientConnection>>();
        }


        private ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, ClientConnection>> allConnections;
        public void AddConnection(ClientConnection connection)
        {
            ConcurrentDictionary<Guid, ClientConnection> userConnections;
            if (!allConnections.TryGetValue(connection.User.Id, out userConnections))
            {
                userConnections = new ConcurrentDictionary<Guid, ClientConnection>();
                allConnections[connection.User.Id] = userConnections;
            }

            userConnections[connection.Id] = connection;
        }

        public bool RemoveConnection(ClientConnection connection)
        {
            ConcurrentDictionary<Guid, ClientConnection> userConnections;
            if (!allConnections.TryGetValue(connection.User.Id, out userConnections))
            {
                return true;
            }


            return userConnections.TryRemove(connection.Id, out connection);
        }

        public List<ClientConnection> GetUserConnections(Guid userId)
        {
            return allConnections[userId].Select(p => p.Value).ToList();
        }


        public List<ClientConnection> GetAllConnections()
        {
            return allConnections.SelectMany(p => p.Value).Select(p => p.Value).ToList();
        }
    }
}
