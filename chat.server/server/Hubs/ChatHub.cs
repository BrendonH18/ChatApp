using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq;
using server.Hubs.HubSupport;
using server.Models;

namespace server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ISessionFactory myFactory;
        private readonly IAppConnection _connections;
        private readonly string _botUser;
        private readonly List<string> _rooms;
        private readonly User _user;
        private readonly Channel _channel;
        private readonly LoginManagement _loginManagement;
        private readonly DatabaseQueries _db;

        public ChatHub(IAppConnection connections, ISessionFactory factory)
        {
            myFactory = factory;
            _botUser = "ChatBot";
            _connections = connections;
            _user = new User { Id = 0, IsPasswordValid = false, LoginType = "", Password = "", Username = "" };
            _channel = new Channel { Id = 0, Name = "" };
            _loginManagement = new LoginManagement(factory);
            _db = new DatabaseQueries(factory);
            _
        }
        public void ReturnLoginAttempt(User user)
        {
            User loginResponse = _loginManagement.CreateLoginResponse(user);
            _connections.UpdateConnection(Context.ConnectionId, new UserConnection { User = loginResponse, Channel = _channel });
            //_connections[Context.ConnectionId] = new UserConnection { User = loginResponse, Channel = _channel };
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedUser", loginResponse);
        }



        public void ReturnAvailableChannels()
        {
            List<Channel> channels = _db.QueryDBforChannels();
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedAvailableChannels", channels);
        }

        

        public async Task JoinChannel(Channel newChannel)
        {
            UserConnection userConnection = _connections.GetConnection(Context.ConnectionId);
            await Clients.Client(Context.ConnectionId).SendAsync("ReturnedMessage", "Reset");
            await SendStoredMessagesToCurrentClient(newChannel);
            if (userConnection == null) userConnection = new UserConnection { User = _user, Channel = _channel };
            SwitchSignalRChannelFromTo(userConnection.Channel, newChannel);
            if (userConnection.User.Id != 0) SendNewUserUpdatesToChannel(userConnection.Channel, newChannel, userConnection.User.Username);
            _connections.UpdateConnection(Context.ConnectionId, new UserConnection { User = userConnection.User, Channel = newChannel });
            SendUsersInChannel(newChannel);
            SendUsersInChannel(userConnection.Channel);
        }
        public async void SendNewUserUpdatesToChannel(Channel oldChannel, Channel newChannel, string username)
        {
            await SendMessageToChannel(newChannel, $"{username} has entered {newChannel.Name}", true);
            await SendMessageToChannel(oldChannel, $"{username} has left {oldChannel.Name}", true);
        }
        public async void SwitchSignalRChannelFromTo (Channel oldChannel, Channel newChannel)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, oldChannel.Name);
            await Groups.AddToGroupAsync(Context.ConnectionId, newChannel.Name);
        }
        public Task SendStoredMessagesToCurrentClient(Channel channel)
        {
            List<Message> messages = RetrieveMessagesFromDB(channel);
            messages.ForEach(async m =>
            {
                m.User.Password = "";
                await Clients.Client(Context.ConnectionId).SendAsync("ReturnedMessage", m);
            });
            return Task.CompletedTask;
        }

        public Task SendMessageToChannel(Channel channel, string text, bool isBot = false)
        {
            //_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);
            UserConnection userConnection = _connections.GetConnection(Context.ConnectionId);

            Message message = new Message();
            message.Created_on = DateTime.UtcNow;
            message.User = userConnection.User;
            message.Channel = channel;
            message.Text = text;

            if (!isBot) 
                CreateMessageInDB(message);

            return Clients.Group(channel.Name).SendAsync("ReturnedMessage", message);
        }

        public void SendMessage(string message)
        {
            UserConnection userConnection = _connections.GetConnection(Context.ConnectionId);
            //_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);
            SendMessageToChannel(userConnection.Channel, message);
        }

        public void UpdatePassword(string newPassword)
        {
            

            bool results_bool = UpdatePasswordInDB(newPassword);
            string results_string = results_bool ? "Update Successful" : "Update Unsuccessful";
            Clients.Caller.SendAsync("ReturnedPasswordUpdate", results_string);
        }

        //Server Code

        public bool UpdatePasswordInDB(string newPassword)
        {
            try
            {
                UserConnection userConnection = _connections.GetConnection(Context.ConnectionId);
                //_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);
                if (userConnection == null ) return false;

                newPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                using (var session = myFactory.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.Query<User>()
                            .Where(x => x.Username == userConnection.User.Username)
                            .Update(x => new User { Username = userConnection.User.Username, Password = newPassword });
                        transaction.Commit();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        

        private User RetrieveCredential(string username)
        {
            using (var session = myFactory.OpenSession())
            {
                var loCredential = session.Query<User>()
                    .SingleOrDefault(x => x.Username == username);
                session.Flush();
                return loCredential;
            }
        }

        public User CreateUserInDB(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            
            using (var session = myFactory.OpenSession())
            {
                session.Save(user);
                session.Flush();
            }
            return user;
        }


        

        private void SendUsersInChannel(Channel channel)
        {
            var connectedUsers = _connections.GetConnectionsOnChannel(channel);
                
                //_connections
                //.Where(x => x.Value.Channel.Id == channel.Id && x.Value.User.Id != 0)
                //.ToList();


            Clients.Group(channel.Name).SendAsync("ReturnedConnectedUsers", connectedUsers);
        }

        public void CreateMessageInDB(Message message)
        {
            using (var session = myFactory.OpenSession())
            {
                session.Save(message);
                session.Flush(); // New
            }
        }

        public List<Message> RetrieveMessagesFromDB(Channel channel)
        {
            var roomMessages = new List<Message>();
            using ( var session = myFactory.OpenSession())
            {
                roomMessages = session.Query<Message>()
                    .Where(m => m.Channel.Name == channel.Name)
                    .ToList();
            }
            return roomMessages;
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            //try
            //{
            //    UserConnection userConnection = _connections.GetConnection(Context.ConnectionId);
            //    //_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);
            //    if (userConnection.User.IsPasswordValid == true)
            //        SendMessageToChannel(userConnection.Channel, $"{userConnection.User.Username} has left the {userConnection.Channel.Name}", true);
            //    _connections.RemoveConnection(Context.ConnectionId);
            //    //_connections.Remove(Context.ConnectionId);
            //    if (userConnection.Channel != null)
            //        SendUsersInChannel(userConnection.Channel);
            //    return base.OnDisconnectedAsync(exception);
            //}
            //catch (Exception)
            //{
            //    return Task.CompletedTask;
            //    //throw;
            //}
            return Task.CompletedTask;
        }
    }
}
