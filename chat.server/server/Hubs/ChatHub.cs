using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NHibernate;
using server.Hubs.Services;
using server.Models;

namespace server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IConnectionService _connectionService;
        private readonly ILoginService _loginService;
        private readonly IQueryService _queryService;
        private readonly IChannelService _channelService;
        private readonly ISetupService _setupService;

        public ChatHub(IConnectionService connectionService, ISessionFactory factory)
        {
            
            _connectionService = connectionService;
            _setupService = new SetupService(connectionService);
            _queryService = new QueryService(factory);
            _loginService = new LoginService(_queryService,connectionService, _setupService);
            _channelService = new ChannelService(_queryService);
        }

        public override Task OnConnectedAsync()
        {
            _setupService.StoreInitialConnectionData(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public void ConnectionSetup()
        {
            ReturnAvailableChannels();
        }

        public void ReturnAvailableChannels()
        {
            List<Channel> channels = _queryService.ReturnAvailableChannels_List();
            Clients.Caller.SendAsync("ReturnedAvailableChannels", channels);
        }

        public void UpdatePassword(User user)
        {
            UpdatePasswordResponse response = _loginService.HandleUpdatePassword(user, Context.ConnectionId);
            Clients.Caller.SendAsync("ReturnedUpdatePassword", response);
        }

        public void ReturnLoginAttempt(User user)
        {
            UserConnection response = _loginService.HandleReturnLoginAttempt(user, Context.ConnectionId);
            SendConnectedUsers();
            Clients.Caller.SendAsync("ReturnedUser", response.User);
            Clients.Caller.SendAsync("ReturnedChannel", response.Channel);
        }

        public async Task JoinChannel(Channel channel)
        {
            _setupService.StoreInitialConnectionData(Context.ConnectionId);
            UserConnection userConnection = _connectionService.GetUserConnection_UserConnection(Context.ConnectionId);
            List<Message> messages = _queryService.ReturnMessagesByChannel_List(channel);
            await Clients.Caller.SendAsync("ReturnedMessage", "Reset");
            messages.ForEach(async m =>
            {
                m.User.Password = "";
                await Clients.Caller.SendAsync("ReturnedMessage", m);
            });
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userConnection.Channel.Name);
            await Groups.AddToGroupAsync(Context.ConnectionId, channel.Name);
            List<Message> messages1 = _channelService.HandleCreateKnockMessages(userConnection, channel);
            messages1.ForEach(async m =>
            {
                await SendMessageToChannel(m);
            });
            _connectionService.UpdateUserConnection_Void(Context.ConnectionId, new UserConnection { User = userConnection.User, Channel = channel });
            await Clients.Caller.SendAsync("ReturnedChannel", channel);
            await Clients.Caller.SendAsync("ReturnedUser", userConnection.User);
            SendConnectedUsers();
        }

        private void SendConnectedUsers()
        {
            List<UserConnection> connectedUsers = _connectionService.GetAllUserConnections_List();
            Clients.All.SendAsync("ReturnedConnectedUsers", connectedUsers);
        }


        public Task SendMessageToChannel(Message message)
        {
            UserConnection userConnection = _connectionService.GetUserConnection_UserConnection(Context.ConnectionId);
            if (userConnection == null ) return Task.CompletedTask;
            if (!userConnection.User.IsPasswordValid ) return Task.CompletedTask;
            Message response = _channelService.HandleNewMessage(message, userConnection);
            return Clients.Group(message.Channel.Name).SendAsync("ReturnedMessage", response);
        }

        public void Logout()
        {
            _setupService.StoreInitialConnectionData(Context.ConnectionId);
            UserConnection userConnection = _connectionService.GetUserConnection_UserConnection(Context.ConnectionId);
            List<Message> messages = _channelService.HandleCreateKnockMessages(userConnection, new Channel { Id = 0, Name = "" });
            messages.ForEach(m =>
            {
                SendMessageToChannel(m);
            });
            userConnection = _setupService.HandleLogout(userConnection, Context.ConnectionId);
            SendConnectedUsers();
            Clients.Caller.SendAsync("ReturnedUser", userConnection.User);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Logout();
            _connectionService.RemoveUserConnection_Void(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
