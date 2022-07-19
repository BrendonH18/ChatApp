using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
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

        public ChatHub(IConnectionService connectionService, ISessionFactory factory, IConfiguration configuration)
        {
            
            _connectionService = connectionService;
            _setupService = new SetupService(connectionService);
            _queryService = new QueryService(factory);
            _loginService = new LoginService(_queryService,connectionService, _setupService, configuration);
            _channelService = new ChannelService(_queryService);
        }

        //public override Task OnConnectedAsync()
        //{
        //    _setupService.StoreInitialConnectionData(Context.ConnectionId);
        //    return base.OnConnectedAsync();
        //}

        public void ConnectionSetup()
        {
            //ReturnAvailableChannels();
        }

        public Task ReturnAvailableChannels()
        {
            //List<Channel> channels = _queryService.ReturnAvailableChannels_List();
            //Clients.Caller.SendAsync("ReturnedAvailableChannels", channels);
            return Task.CompletedTask;
        }

        public Task UpdatePassword(User user)
        {
            //UpdatePasswordResponse response = _loginService.HandleUpdatePassword(user, Context.ConnectionId);
            //Clients.Caller.SendAsync("ReturnedUpdatePassword", response);
            return Task.CompletedTask;
        }


        public void ReturnJWTTest()
        {

            Clients.Caller.SendAsync("ReturnedJWTTest", Context.User.Identity.IsAuthenticated);
        }


        public Task ReturnStartUpValidation(User user)
        {
            if (!Context.User.Identity.IsAuthenticated)
                return Clients.Caller.SendAsync("ReturnedStartUpValidation", false);
            var name = Context.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            if (user.Username != name)
                return Clients.Caller.SendAsync("ReturnedStartUpValidation", false);
            return Clients.Caller.SendAsync("ReturnedStartUpValidation", true);
        }

        public void ReturnLoginAttempt(User user)
        {
            _loginService.HandleReturnLoginAttempt(user, out UserModel returnUser, out string token);
            //SendConnectedUsers();
            Clients.Caller.SendAsync("ReturnedUser", returnUser);
            Clients.Caller.SendAsync("ReturnedToken", token);
            //Clients.Caller.SendAsync("ReturnedChannel", response.Channel);
        }

        //AUTHORIZE???
        public async Task ChangeChannel(User user, Channel enterChannel, Channel exitChannel)
        {
            if (user == null) return;
            if (user.Username == null || user.Username.Length == 0) return;
            if (enterChannel == null) return;
            if (enterChannel.Name.Length == 0) return;

            List<Message> messages = _queryService.ReturnMessagesByChannel_List(enterChannel);
            
            await Clients.Caller.SendAsync("ReturnedMessage", "Reset");
            messages.ForEach(async m =>
            {
                await Clients.Caller.SendAsync("ReturnedMessage", m);
            });

            if (exitChannel != null) { 
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, exitChannel.Name);
                await _connectionService.RemoveUserFromChannel(user, exitChannel);
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, enterChannel.Name);
            await _connectionService.AddUserToChannel(user, enterChannel);

            List<Message> messages1 = _channelService.HandleCreateKnockMessages(user, enterChannel, exitChannel);
            messages1.ForEach(async m =>
            {
                await SendMessageToChannel(m.Text, m.User, m.Channel, true);
            });

            await Clients.Caller.SendAsync("ReturnedChannel", enterChannel);
            //SendConnectedUsers();

            return;
        }

        private Task SendConnectedUsers(Channel channel)
        {
            //List<User> connectedUsers = _connectionService.GetUsersOnChannel_List(channel);
            //Clients.All.SendAsync("ReturnedConnectedUsers", connectedUsers);
            return Task.CompletedTask;
        }


        public Task SendMessageToChannel(string text, User user, Channel channel, bool isBot = false)
        {
            Message response = _channelService.HandleNewMessage(text, user, channel, isBot);
            return Clients.Group(channel.Name).SendAsync("ReturnedMessage", response);
        }

        public void Logout()
        {
            //_setupService.StoreInitialConnectionData(Context.ConnectionId);
            //UserConnection userConnection = _connectionService.GetUser_Username(Context.ConnectionId);
            //List<Message> messages = _channelService.HandleCreateKnockMessages(userConnection, new Channel { Id = 0, Name = "" });
            //messages.ForEach(m =>
            //{
            //    SendMessageToChannel(m);
            //});
            //userConnection = _setupService.HandleLogout(userConnection, Context.ConnectionId);
            //SendConnectedUsers();
            //Clients.Caller.SendAsync("ReturnedUser", userConnection.User);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            //Logout();
            //_connectionService.RemoveUser(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
