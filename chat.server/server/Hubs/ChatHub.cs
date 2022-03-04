using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NHibernate;
using NHibernate.Cfg;
using server.Models;

namespace server.Hubs
{
    public class ChatHub : Hub
    {
        //NHibernate
        private Configuration myConfiguration;
        private ISessionFactory mySessionFactory;
        private ISession mySession;

        //Server
        private readonly IDictionary<string, UserConnection> _connections;
        private readonly string _botUser;
        private readonly List<string> _rooms;

        public ChatHub(IDictionary<string, UserConnection> connections)
        {
            //NHibernate
            myConfiguration = new Configuration();
            myConfiguration.Configure();
            mySessionFactory = myConfiguration.BuildSessionFactory();
            //mySession = mySessionFactory.OpenSession(); // unsafe

            //Server
            _botUser = "ChatBot";
            _connections = connections;
            _rooms = new List<string> { "Sports" , "Culture" , "Art" , "Fashion" , "Custom/New"};
        }

        //Online users stored in "_connections"
        //User Credentials stored in Database
        //Past Messages stored in Database


        //SIGNALR REQUESTS
        public void ReturnIsValid(Credential credential)
        {
            if (credential.LoginType == "Guest")
                GuestLogin(credential);
            if (credential.LoginType == "Create")
            {
                if (RetrieveCredential(credential.Username) == null)
                    AddCredential(credential);
                CheckCredential(credential);
            }
            if (credential.LoginType == "Returning")
                CheckCredential(credential);
        }

        public void ReturnAvailableRooms()
        {
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedAvailableRooms", _rooms);
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            _connections[Context.ConnectionId] = userConnection;
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.ActiveRoom);

            List<Message> messages = RetrieveMessagesFromDB(userConnection.ActiveRoom);
            messages.ForEach(async m =>
                await Clients.Client(Context.ConnectionId).SendAsync("ReturnedMessage", new Message { Username = m.Username, Text = m.Text, Created_on = m.Created_on })
            );

            var param = new Message
            {
                Username = _botUser,
                Text = $"{userConnection.Username} has entered {userConnection.ActiveRoom}",
                Created_on = DateTime.Now,
                Room = userConnection.ActiveRoom
            };

            await Clients.Group(userConnection.ActiveRoom).SendAsync("ReturnedMessage", param);

            await SendUsersInRoom(userConnection.ActiveRoom);

        }

        public async Task LeaveRoom()
        {
            _connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);

            var param = new Message
            {
                Username = _botUser,
                Text = $"{userConnection.Username} has left {userConnection.ActiveRoom}",
                Created_on = DateTime.Now,
                Room = userConnection.ActiveRoom
            };

            await Clients.Group(userConnection.ActiveRoom).SendAsync("ReturnedMessage", param);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userConnection.ActiveRoom);
            userConnection.ActiveRoom = null;
            SendUsersInRoom(param.Room);
        }

        public async Task SendMessage(string message)
        {
            _connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);

            var param = new Message
            {
                Username = userConnection.Username,
                Text = message,
                Created_on = DateTime.UtcNow,
                Room = userConnection.ActiveRoom
            };

            await Clients.All.SendAsync("ReturnedMessage", param);
            CreateMessageInDB(param);
        }

        //Server Code
        public void GuestLogin(Credential credential)
        {
            var param = new
            {
                IsValid = true,
                Username = AppendNumberToUsername(credential.Username),
                LoginMessage = "Guest Login Successful"
            };
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedIsValid", param);
        }

        private string AppendNumberToUsername(string username)
        {
            var number = new Random().Next(1000, 10000).ToString();
            return username += number;
        }

        private Credential RetrieveCredential(string username)
        {
            using (mySession = mySessionFactory.OpenSession())
            {
                var loCredential = mySession.Query<Credential>()
                    .SingleOrDefault(x => x.Username == username);
                mySession.Flush();
                return loCredential;
            }
        }

        private async void AddCredential(Credential credential)
        {
            var loCredential = new Credential
            {
                Username = credential.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(credential.Password)
            };
            using (mySession = mySessionFactory.OpenSession())
            {
                await mySession.SaveAsync(loCredential);
                mySession.Flush();
            }
        }

        private void CheckCredential(Credential credential)
        {
            var param = new
            {
                IsValid = IsValid(credential),
                Username = credential.Username,
                LoginMessage = IsValid(credential) ? "Login Successful" : "Login Unsuccessful"
            };
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedIsValid", param);
        }

        private bool IsValid(Credential credential)
        {
            var loCredential = RetrieveCredential(credential.Username);
            if (loCredential == null)
                return false;
            var hashedPassword = loCredential.Password;
            return BCrypt.Net.BCrypt.Verify(credential.Password, hashedPassword);

        }

        private void SendUsersInRoom(string room)
        {
            var param = _connections.Values
                .Where(connection => connection.ActiveRoom == room)
                .Select(connection => connection.Username)
                .Distinct();
            
            Clients.Group(room).SendAsync("ReturnedUsers", param);
        }

        public void CreateMessageInDB(Message param)
        {
            using (mySession = mySessionFactory.OpenSession())
            {
                try
                {
                    mySession.Save(param);
                    mySession.Flush(); // New
                }
                catch (Exception e)
                {
                    var ErrorOnCreateMessage = e;
                }
            }
        }

        public List<Message> RetrieveMessagesFromDB(string room)
        {
            var roomMessages = new List<Message>();

            using (mySession = mySessionFactory.OpenSession())
            {
                try
                {
                    roomMessages = mySession.Query<Message>()
                        .Where(m => m.Room == room)
                        .ToList();

                    mySession.Flush(); // New
                }
                catch (Exception e)
                {
                    var ErrorOnRetrieveMessage = e;
                }

                return roomMessages;
            }
        }

        public List<string> RetrieveRooms(string user)
        {
            var userRooms = new List<string>();

            using (mySession = mySessionFactory.OpenSession())
            {
                try
                {
                    userRooms = mySession.Query<Message>()
                        .Where(m => m.Username == user)
                        .Select(m => m.Room)
                        .Distinct()
                        .ToList();

                    mySession.Flush(); // New
                }
                catch (Exception e)
                {
                    var ErrorOnRetrieveRooms = e;
                }

                return userRooms;
            }
        }
    }
}
