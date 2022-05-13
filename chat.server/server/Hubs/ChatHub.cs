using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NHibernate;
using server.Hubs.HubSupport;
using server.Models;

namespace server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IAppConnection _connectionManagement;
        private readonly User _user;
        private readonly Channel _channel;
        private readonly LoginManagement _loginManagement;
        private readonly QueryManagement _queryManagement;
        private readonly ChannelManagement _channelManagement;

        public ChatHub(IAppConnection connectionManagement, ISessionFactory factory)
        {
            
            _connectionManagement = connectionManagement;
            _user = new User { Id = 0, IsPasswordValid = false, LoginType = "", Password = "", Username = "" };
            _channel = new Channel { Id = 0, Name = "" };
            _queryManagement = new QueryManagement(factory);
            _loginManagement = new LoginManagement(_queryManagement);
            _channelManagement = new ChannelManagement();
        }

        public void ReturnAvailableChannels()
        {
            List<Channel> channels = _queryManagement.ReturnAvailableChannels_List();
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedAvailableChannels", channels);
        }

        public void ReturnLoginAttempt(User user)
        {
            User loginResponse = _loginManagement.CreateLoginResponse(user);
            if (user.LoginType == "Update" && user.IsPasswordValid == true)
                loginResponse = _queryManagement.UpdatePasswordForUser(user);
            _connectionManagement.UpdateUserConnection_Void(Context.ConnectionId, new UserConnection { User = loginResponse, Channel = _channel });
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedUser", loginResponse);
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
                await SendMessageToChannel(new Message { isBot = true, Text = $"{userConnection.User.Username} has entered {newChannel.Name}", Channel = newChannel});
                await SendMessageToChannel(new Message { isBot = true, Text = $"{userConnection.User.Username} has left {userConnection.Channel.Name}", Channel = userConnection.Channel});
            }
            _connectionManagement.UpdateUserConnection_Void(Context.ConnectionId, new UserConnection { User = userConnection.User, Channel = newChannel });
            SendConnectedUsersInChannel(newChannel);
            SendConnectedUsersInChannel(userConnection.Channel);
        }

        private void SendConnectedUsersInChannel(Channel channel)
        {
            var connectedUsers = _connectionManagement.GetUserConnectionsOnChannel_List(channel);
            Clients.Group(channel.Name).SendAsync("ReturnedConnectedUsers", connectedUsers);
        }


        public Task SendMessageToChannel(Message paramMessage)
        {
            UserConnection userConnection = _connectionManagement.GetUserConnection_UserConnection(Context.ConnectionId);
            Message loMessage = _channelManagement.FormatNewMessage(paramMessage, userConnection);
            if (!loMessage.isBot)
                _queryManagement.InsertMessage(loMessage);
            return Clients.Group(loMessage.Channel.Name).SendAsync("ReturnedMessage", loMessage);
        }

        //public User UpdateUserPassword(User paramUser)
        //{
        //    UserConnection userConnection = _connectionManagement.GetUserConnection_UserConnection(Context.ConnectionId);
        //    User loUser = _queryManagement.UpdatePasswordForUser(paramUser);
        //    return loUser;
            //string results_string = results_bool ? "Update Successful" : "Update Unsuccessful";
            //Clients.Caller.SendAskync("ReturnedPasswordUpdate", results_string);
        //}

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
