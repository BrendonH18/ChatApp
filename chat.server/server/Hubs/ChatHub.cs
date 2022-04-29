using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq;
using server.Models;

namespace server.Hubs
{
    public class ChatHub : Hub
    {
        //NHibernate
        private ISessionFactory myFactory;

        //Server
        private readonly IDictionary<string, UserConnection> _connections;
        private readonly string _botUser;
        private readonly List<string> _rooms;

        public ChatHub(IDictionary<string, UserConnection> connections, ISessionFactory factory)
        {
            //NHibernate
            myFactory = factory;

            //Server
            _botUser = "ChatBot";
            _connections = connections;
        }
        //SIGNALR REQUESTS
        //NEW

        public void ReturnLoginAttempt(User user)
        {
            User loginResponse = CreateLoginResponse(user);
            _connections[Context.ConnectionId] = new UserConnection { User = user };
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedUser", loginResponse);
        }
        
        public User CreateLoginResponse(User user)
        {
            if(user == null || !IsKnownLoginType(user.LoginType) )
            {
                user.Password = null;
                user.IsPasswordValid = false;
                return user;
            }
            if(user.LoginType == "Guest")
            {
                user = AppendNumberToUsername(user);
                user.IsPasswordValid = true;
                return user;
            }
            if(user.LoginType == "Create")
            {
                CreateUserInDB(user);
            }

            user = IsValid(user);
            user.Password = null;
            return user;
        }

        public bool IsKnownLoginType(string loginType)
        {
            if (loginType == "Guest") return true;
            if (loginType == "Create") return true;
            if (loginType == "Returning") return true;
            return false;
        }
        
        public void ReturnAvailableChannels()
        {
            List<Channel> channels;
            using (var session = myFactory.OpenSession())
            {
                channels = session.Query<Channel>()
                        .ToList();
            }
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedAvailableChannels", channels);
        }


        public async Task JoinChannel(UserConnection userConnection)
        {
            _connections[Context.ConnectionId] = userConnection;

            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Channel.Name);

            ToggleClientToChat();

            int lastId = 0;
            List<Message> messages = RetrieveMessagesFromDB(userConnection.Channel);
            messages.ForEach(async m =>
            {
                lastId = m.Id;

                await SendMessageToThisClient(m);
            });

            await SendMessageToEntireGroup($"{userConnection.User.Username} has entered {userConnection.Channel.Name}", true);

            SendUsersInChannel(userConnection.Channel.Name);
        }

        public Task SendMessageToThisClient(Message message)
        {
            return Clients.Client(Context.ConnectionId).SendAsync("ReturnedMessage", message);
        }

        public Task SendMessageToEntireGroup(string text, bool isBot = false)
        {
            _connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);

            Message message = new Message();
            message.Created_on = DateTime.UtcNow;
            message.User = userConnection.User;
            //message.Username = userConnection.User.Username;
            message.Channel = userConnection.Channel;
            //message.Channel_name = userConnection.Channel.Name;
            message.Text = text;


            //Could cast the "Username" and "Channel_name" as strings even though their numbers. Will the database bounce it?
            if (!isBot) 
                CreateMessageInDB(message);

            return Clients.Group(userConnection.Channel.Name).SendAsync("ReturnedMessage", message);
        }

        public Task ToggleClientToChat()
        {
            return Clients.Client(Context.ConnectionId).SendAsync("ReturnedToggleDisplay", "Chat");
        }

        public void ToggleClientToLobby()
        {
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedToggleDisplay", "Lobby");
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedChannel", new { });
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedConnectedUsers", new List<string>());
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedMessage", "Reset");
            ReturnAvailableChannels();
        }

        public async Task LeaveChannel()
        {
            _connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userConnection.Channel.Name);

            await SendMessageToEntireGroup($"{userConnection.User.Username} has left {userConnection.Channel.Name}", true);

            _connections[Context.ConnectionId] = new UserConnection { User = userConnection.User };

            SendUsersInChannel(userConnection.Channel.Name);

            ToggleClientToLobby();
        }

        public void LogOut()
        {
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedUser", new { });
        }

        public void SendMessage(string message)
        {
            SendMessageToEntireGroup(message);
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
                _connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);
                if (userConnection == null) return false;

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

        private User AppendNumberToUsername(User user)
        {
            var number = new Random().Next(1000, 10000).ToString();
            user.Username = user.Username += number;
            return user;
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

        public void CreateUserInDB(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            
            using (var session = myFactory.OpenSession())
            {
                session.Save(user);
                session.Flush();
            }
        }


        private User IsValid(User user)
        {
            var localUser = RetrieveCredential(user.Username);
            if (localUser == null)
            {
                user.IsPasswordValid = false;
                return user;
            }
            user.IsPasswordValid = BCrypt.Net.BCrypt.Verify(user.Password, localUser.Password);
            user.Id = user.IsPasswordValid ? localUser.Id : 0;
            return user;
        }

        //AN ERROR
        private void SendUsersInChannel(string channel)
        {
            var names = new List<string>();
            var data = _connections.Values
                .Where(c => c.Channel.Name == channel)
                .ToList();

            foreach (var name in data)
            {
                names.Add(name.User.Username);
            }
            
            Clients.Group(channel).SendAsync("ReturnedConnectedUsers", names);
        }

        //AN ERROR
        public void CreateMessageInDB(Message message)
        {
            using (var session = myFactory.OpenSession())
            {
                try
                {
                    session.Save(message);
                    session.Flush(); // New
                }
                catch (Exception e)
                {
                    var ErrorOnCreateMessage = e;
                }
            }
        }

        public List<Message> RetrieveMessagesFromDB(Channel channel)
        {
            var roomMessages = new List<Message>();

            using ( var session = myFactory.OpenSession())
            {
                try
                {
                    roomMessages = session.Query<Message>()
                        //issue
                        .Where(m => m.Channel.Name == channel.Name)
                        .ToList();

                    //session.Flush(); // New
                }
                catch (Exception e)
                {
                    var ErrorOnRetrieveMessage = e;
                }
                
            }
            return roomMessages;
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);
            _connections.Remove(Context.ConnectionId);

            if (userConnection != null && userConnection.Channel != null)
            {
                Clients.Group(userConnection.Channel.Name).SendAsync("ReturnedMessage", new Message { User = new User { Username = _botUser}, Text = $"{userConnection.User} has left the room" });
                SendUsersInChannel(userConnection.Channel.Name);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
