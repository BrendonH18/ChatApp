﻿using System;
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

        public void ConnectionSetup()
        {
            _connectionManagement.UpdateUserConnection_Void(Context.ConnectionId, new UserConnection { Channel = _channel, User = _user });
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
            user.LoginType = "Update";
            user = _loginManagement.CreateLoginResponse(user);
            if (user.IsPasswordValid == false)
            {
                Clients.Caller.SendAsync("ReturnedUpdatePassword", false);
                return;
            }
            user = _queryManagement.UpdatePasswordForUser(user);
            Clients.Caller.SendAsync("ReturnedUpdatePassword", true);
        }

        public void ReturnLoginAttempt(User user)
        {
            User loginResponse = _loginManagement.CreateLoginResponse(user);
            _connectionManagement.UpdateUserConnection_Void(Context.ConnectionId, new UserConnection { User = loginResponse, Channel = _channel });
            SendConnectedUsers();
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
            await Clients.Caller.SendAsync("ReturnedChannel", newChannel); //NEW
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

        public void Logout()
        {
            UserConnection userConnection = _connectionManagement.GetUserConnection_UserConnection(Context.ConnectionId);
            if (userConnection == null) return;
            if (userConnection.User.IsPasswordValid == true)
                SendMessageToChannel(new Message { Text = $"{userConnection.User.Username} has left {userConnection.Channel.Name}", Channel = userConnection.Channel, isBot = true });
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedUser", _user);
            userConnection.User = _user;
            _connectionManagement.UpdateUserConnection_Void(Context.ConnectionId, userConnection);
            SendConnectedUsers();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Logout();
            _connectionManagement.RemoveUserConnection_Void(Context.ConnectionId);
            return Task.CompletedTask;
        }
    }
}
