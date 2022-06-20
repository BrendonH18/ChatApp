using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NHibernate;
using server.Hubs.HubManagement;
using server.Models;

namespace server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IConnectionManagement _connectionManagement;
        private readonly User _user;
        private readonly Channel _channel;
        private readonly LoginManagement _loginManagement;
        private readonly IQueryManagement _queryManagement;
        private readonly ChannelManagement _channelManagement;

        public ChatHub(IConnectionManagement connectionManagement, ISessionFactory factory)
        {
            
            _connectionManagement = connectionManagement;
            _user = new User { Id = 0, IsPasswordValid = false, LoginType = "", Password = "", Username = "" };
            _channel = new Channel { Id = 0, Name = "" };
            _queryManagement = new QueryManagement(factory);
            _loginManagement = new LoginManagement(_queryManagement);
            _channelManagement = new ChannelManagement();
        }

        public override Task OnConnectedAsync()
        {
            var x = _connectionManagement.GetUserConnection_UserConnection(Context.ConnectionId);
            
            if (_connectionManagement.GetUserConnection_UserConnection(Context.ConnectionId) == null)
                _connectionManagement.UpdateUserConnection_Void(Context.ConnectionId, new UserConnection { Channel = _channel, User = _user });
            return base.OnConnectedAsync();
        }

        public void ConnectionSetup()
        {
            ReturnAvailableChannels();
        }

        public void ReturnAvailableChannels()
        {
            List<Channel> channels = _queryManagement.ReturnAvailableChannels_List();
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedAvailableChannels", channels);
        }

        public void UpdatePassword(User user)
        {
            UserConnection userConnection = _connectionManagement.GetUserConnection_UserConnection(Context.ConnectionId);
            user.Username = userConnection.User.Username;
            user = _loginManagement.IsValidUser(user);
            UpdatePasswordResponse response = new();
            if (user.IsPasswordValid == false)
            {
                response.IsPasswordApproved = false;
                Clients.Caller.SendAsync("ReturnedUpdatePassword", response);
                return;
            }
            _queryManagement.UpdatePasswordForUser(user);
            response.IsPasswordApproved = true;
            Clients.Caller.SendAsync("ReturnedUpdatePassword", response);
        }

        public void ReturnLoginAttempt(User user)
        {
            UserConnection userConnection = _connectionManagement.GetUserConnection_UserConnection(Context.ConnectionId);
            if (userConnection == null) return;
            userConnection.User.LoginType = user.LoginType;
            if (userConnection.User.LoginType == "Guest" || _connectionManagement.IsUserLoggedIn(user) == false)
                userConnection.User = _loginManagement.CreateLoginResponse(user);
            _connectionManagement.UpdateUserConnection_Void(Context.ConnectionId, userConnection);
            SendConnectedUsers();
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedUser", userConnection.User);
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedChannel", userConnection.Channel);
        }

        public async Task JoinChannel(Channel newChannel)
        {
            UserConnection userConnection = _connectionManagement.GetUserConnection_UserConnection(Context.ConnectionId);
            await Clients.Client(Context.ConnectionId).SendAsync("ReturnedMessage", "Reset");
            List<Message> messages = _queryManagement.ReturnMessagesByChannel_List(newChannel);
            messages.ForEach(async m =>
            {
                m.User.Password = "";
                await Clients.Client(Context.ConnectionId).SendAsync("ReturnedMessage", m);
            });
            if (userConnection == null) 
                userConnection = new UserConnection { User = _user, Channel = _channel };
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userConnection.Channel.Name);
            await Groups.AddToGroupAsync(Context.ConnectionId, newChannel.Name);
            if (userConnection.User.Id != 0)
            {
                await SendMessageToChannel(new Message { IsBot = true, Text = $"{userConnection.User.Username} has entered {newChannel.Name}", Channel = newChannel});
                await SendMessageToChannel(new Message { IsBot = true, Text = $"{userConnection.User.Username} has left {userConnection.Channel.Name}", Channel = userConnection.Channel});
            }
            _connectionManagement.UpdateUserConnection_Void(Context.ConnectionId, new UserConnection { User = userConnection.User, Channel = newChannel });
            await Clients.Caller.SendAsync("ReturnedChannel", newChannel);
            await Clients.Caller.SendAsync("ReturnedUser", userConnection.User); //NEW
            //SendConnectedUsersInChannel(newChannel);
            SendConnectedUsers();
        }

        private void SendConnectedUsers()
        {
            List<UserConnection> connectedUsers = _connectionManagement.GetAllUserConnections_List();
            Clients.All.SendAsync("ReturnedConnectedUsers", connectedUsers);
        }


        public Task SendMessageToChannel(Message paramMessage)
        {
            UserConnection userConnection = _connectionManagement.GetUserConnection_UserConnection(Context.ConnectionId);
            if (userConnection == null ) return Task.CompletedTask;
            if (!userConnection.User.IsPasswordValid ) return Task.CompletedTask;
            Message loMessage = _channelManagement.FormatNewMessage(paramMessage, userConnection);
            if (!loMessage.IsBot)
                _queryManagement.InsertMessage(loMessage);
            return Clients.Group(loMessage.Channel.Name).SendAsync("ReturnedMessage", loMessage);
        }

        public void Logout()
        {
            UserConnection userConnection = _connectionManagement.GetUserConnection_UserConnection(Context.ConnectionId);
            if (userConnection == null) return;
            if (userConnection.User.IsPasswordValid == true)
                SendMessageToChannel(new Message { Text = $"{userConnection.User.Username} has left {userConnection.Channel.Name}", Channel = userConnection.Channel, IsBot = true });
            userConnection.User = _user;
            _connectionManagement.UpdateUserConnection_Void(Context.ConnectionId, userConnection);
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedUser", userConnection.User);
            SendConnectedUsers();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Logout();
            _connectionManagement.RemoveUserConnection_Void(Context.ConnectionId);
            SendConnectedUsers();
            return base.OnDisconnectedAsync(exception);
        }
    }
}
