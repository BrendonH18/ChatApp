using server.Models;
using System.Collections.Generic;
using System.Linq;

namespace server.Hubs.HubManagement
{
    public interface IConnectionManagement
    {
        void UpdateUserConnection_Void(string connectionID, UserConnection userConnection);
        UserConnection GetUserConnection_UserConnection(string connectionID);
        void RemoveUserConnection_Void(string connectionID);

        List<UserConnection> GetUserConnectionsOnChannel_List(Channel channel);

        List<UserConnection> GetAllUserConnections_List();
        bool IsUserLoggedIn(User user);
    }
    public class ConnectionManagement : IConnectionManagement
    {
        public readonly Dictionary<string, UserConnection> _connections = new();

        public void UpdateUserConnection_Void(string connectionId, UserConnection userConnection)
        {
            if (userConnection.ConnectionId != connectionId) userConnection.ConnectionId = connectionId;
            _connections[connectionId] = userConnection;
        }
        public void RemoveUserConnection_Void(string connectionId)
        {
            _connections.Remove(connectionId);
        }
        public UserConnection GetUserConnection_UserConnection (string connectionId)
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
        public List<UserConnection> GetUserConnectionsOnChannel_List(Channel channel)
        {
            List<UserConnection> connections = new();
            connections = _connections.Values
                .Where(x=>x.Channel.Id == channel.Id && x.User.Id != 0)
                .Distinct()
                .ToList();
            return connections;
        }

        public List<UserConnection> GetAllUserConnections_List()
        {
            List<UserConnection> userConnections = _connections.Values.ToList();
            return userConnections;
        }

        public bool IsUserLoggedIn(User user)
        {
            List<UserConnection> connections = new();
            connections = _connections.Values
                .Where(x => x.User.IsPasswordValid == true && x.User.Username == user.Username)
                .ToList();
                //.Where(x => x.User.Username == user.Username);
            if (connections.Count == 0) return false;
            return true;
        }
    }
}
