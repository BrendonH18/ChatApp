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
        private readonly IAppConnection _connectionsManagement;
        private readonly string _botUser;
        private readonly User _user;
        private readonly Channel _channel;
        private readonly LoginManagement _loginManagement;
        private readonly QueryManagement _queryManagement;
        private readonly ChannelManagement _channelManagement;

        public ChatHub(IAppConnection connections, ISessionFactory factory)
        {
            
            _botUser = "ChatBot";
            _connectionsManagement = connections; //ok
            _user = new User { Id = 0, IsPasswordValid = false, LoginType = "", Password = "", Username = "" };
            _channel = new Channel { Id = 0, Name = "" };
            _queryManagement = new QueryManagement(factory); // ok
            _loginManagement = new LoginManagement(_queryManagement);
            _channelManagement = new ChannelManagement();
        }
        public void ReturnLoginAttempt(User user)
        {
            User loginResponse = _loginManagement.CreateLoginResponse(user);
            _connectionsManagement.UpdateConnection(Context.ConnectionId, new UserConnection { User = loginResponse, Channel = _channel });
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedUser", loginResponse);
        }



        public void ReturnAvailableChannels()
        {
            List<Channel> channels = _queryManagement.QueryDBforChannels();
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedAvailableChannels", channels);
        }

        public async Task JoinChannel(Channel newChannel)
        {
            UserConnection userConnection = _connectionsManagement.GetConnection(Context.ConnectionId);
            await Clients.Client(Context.ConnectionId).SendAsync("ReturnedMessage", "Reset");
            List<Message> messages = _queryManagement.RetrieveMessagesFromDB(newChannel);
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
                await SendMessage($"{userConnection.User.Username} has entered {newChannel.Name}", newChannel, true);
                await SendMessage($"{userConnection.User.Username} has left {userConnection.Channel.Name}", userConnection.Channel, true);
            }
            _connectionsManagement.UpdateConnection(Context.ConnectionId, new UserConnection { User = userConnection.User, Channel = newChannel });
            SendUsersInChannel(newChannel);
            SendUsersInChannel(userConnection.Channel);
        }

        private void SendUsersInChannel(Channel channel)
        {
            var connectedUsers = _connectionsManagement.GetConnectionsOnChannel(channel);
            Clients.Group(channel.Name).SendAsync("ReturnedConnectedUsers", connectedUsers);
        }


        public Task SendMessage(string messageText, Channel specificChannel = null, bool isBot = false)
        {
            UserConnection userConnection = _connectionsManagement.GetConnection(Context.ConnectionId);
            Message message = _channelManagement.FormatNewMessage(messageText, userConnection, specificChannel);
            if (!isBot)
                _queryManagement.CreateMessageInDB(message);
            return Clients.Group(message.Channel.Name).SendAsync("ReturnedMessage", message);
        }

        public void UpdatePassword(string newPassword)
        {
            UserConnection userConnection = _connectionsManagement.GetConnection(Context.ConnectionId);
            bool results_bool = _queryManagement.UpdatePasswordInDB(newPassword, userConnection);
            //string results_string = results_bool ? "Update Successful" : "Update Unsuccessful";
            //Clients.Caller.SendAskync("ReturnedPasswordUpdate", results_string);
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
