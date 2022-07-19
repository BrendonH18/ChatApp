using server.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Hubs.Services
{
    public interface IConnectionService
    {
        public User GetUser_Username(string userName);
        public Task RemoveUserFromAll (User userName);
        public List<User> GetUsersOnChannel_List(Channel channel);
        public List<User> GetAllUsers_List();
        public bool IsUserLoggedIn(User user);
        public Task AddUserToChannel(User user, Channel channel);
        public Task RemoveUserFromChannel(User user, Channel channel);
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
        public User GetUser_Username (string userName)
        {
            _connectedUsers.TryGetValue(userName, out User user);
            return user;
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
            _usersByChannel[channel.Name].Add(user);
            var userObj = GetUser_Username(user.Username);
            userObj.Channels.Add(channel);
            // Perhaps an update function versus all the other functions
            return Task.CompletedTask;
        }

        public Task RemoveUserFromChannel(User user, Channel channel)
        {
            var usersOnChannel = _usersByChannel[channel.Name].Remove(user);
            var userObj = GetUser_Username(user.Username);
            userObj.Channels.Remove(channel);
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
