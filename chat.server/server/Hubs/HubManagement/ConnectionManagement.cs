using server.Models;
using System.Collections.Generic;

namespace server.Hubs.HubSupport
{
    public interface IAppConnection
    {
        void UpdateConnection(string connectionID, UserConnection userConnection);
        UserConnection GetConnection(string connectionID);
        void RemoveConnection(string connectionID);

        List<UserConnection> GetConnectionsOnChannel(Channel channel);
    }
    public class ConnectionManagement : IAppConnection
    {
        public readonly Dictionary<string, UserConnection> _connections = new Dictionary<string, UserConnection>();

        public void UpdateConnection(string connectionId, UserConnection userConnection)
        {
            _connections[connectionId] = userConnection;
        }
        public void RemoveConnection(string connectionId)
        {
            _connections.Remove(connectionId);
        }
        public UserConnection GetConnection (string connectionId)
        {
            _connections.TryGetValue(connectionId, out UserConnection userConnection);
            return userConnection;
        }
        //public List<UserConnection> GetConnectionsOnChannel(Channel channel)
        //{
        //    Array<string, UserConnection> connections = new List<string, UserConnection>();
        //    connections = _connections
        //        .Where(x => x.Value.Channel.Id == channel.Id && x.Value.User.Id != 0)
        //        .ToList();
        //}
        public List<UserConnection> GetConnectionsOnChannel(Channel channel)
        {
            List<UserConnection> connections = new List<UserConnection>();
            return connections;
        }
    }
}
