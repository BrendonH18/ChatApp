using server.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Hubs.Services
{
    public interface IConnectionService
    {
        public Task GetUser_Username(string userName, out User user);
        public Task RemoveUserFromAll (User userName);
        public List<User> GetUsersOnChannel_List(Channel channel);
        public List<User> GetAllUsers_List();
        public bool IsUserLoggedIn(User user);
        public Task AddUserToChannel(User user, Channel channel);
        public Task RemoveUserFromChannel(User user, Channel channel);
        public Task AddUserToServer(User user);
        public Task RemoveUserFromServer(User user);

    }
    public class ConnectionService : IConnectionService
    {
        public readonly Dictionary<string, User> _connectedUsers = new();
        public readonly Dictionary<string, List<User>> _usersByChannel = new();

        public Task RemoveUserFromAll(User user)
        {
            //remove from Channels stored in User
            //remove in general

            //var userChannels = _channels
                //.Where(x => x.Value.Contains(user))
                //.ToList();
            //RemoveUserFromChannel(user);
            //_connections.Remove(connectionId);
            return Task.CompletedTask;
        }

        public Task AddUserToServer(User user)
        {
            _connectedUsers.Add(user.Username, user);
            return Task.CompletedTask;
        }

        public Task RemoveUserFromServer(User user)
        {
            _connectedUsers.Remove(user.Username);
            return Task.CompletedTask;
        }

        public Task GetUser_Username (string userName, out User user)
        {
            _connectedUsers.TryGetValue(userName, out user);
            return Task.CompletedTask;
        }
        //public List<UserConnection> GetConnectionsOnChannel(Channel channel)
        //{
        //    Array<string, UserConnection> connections = new List<string, UserConnection>();
        //    connections = _connections
        //        .Where(x => x.Value.Channel.Id == channel.Id && x.Value.User.Id != 0)
        //        .ToList();
        //}
        public Task AddUserToChannel(User user, Channel channel)
        {
            GetUser_Username(user.Username, out User objUser);
            if (objUser == null) return Task.CompletedTask;
            objUser.Channels.Add(channel);
            var isKeyFound = _usersByChannel.ContainsKey(channel.Name);
            if (!isKeyFound) _usersByChannel[channel.Name] = new List<User>();
            _usersByChannel[channel.Name].Add(objUser);
            // Perhaps an update function versus all the other functions
            return Task.CompletedTask;
        }

        public Task RemoveUserFromChannel(User user, Channel channel)
        {
            GetUser_Username(user.Username, out User objUser);
            if (objUser == null) return Task.CompletedTask;
            objUser.Channels.Remove(channel);
            _usersByChannel[channel.Name].Remove(objUser);
            // Perhaps an update function versus all the other functions
            return Task.CompletedTask;
        }


        public List<User> GetUsersOnChannel_List(Channel channel)
        {
            //List<UserConnection> connections = new();
            //connections = _connectedUsers.Values
            //    .Where(x=>x.Channel.Id == channel.Id && x.User.Id != 0)
            //    .Distinct()
            //    .ToList();
            
            return _usersByChannel[channel.Name];
        }

        public List<User> GetAllUsers_List()
        {
            return _connectedUsers.Values.ToList();
        }

        public bool IsUserLoggedIn(User user)
        {
            var keys = _connectedUsers.Keys
                .Where(x => x == user.Username)
                .ToList();
                //.Where(x => x.User.Username == user.Username);
            if (keys.Count == 0) return false;
            return true;
        }
    }
}
