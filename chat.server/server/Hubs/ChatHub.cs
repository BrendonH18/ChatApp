using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NHibernate;
using NHibernate.Cfg;
using server.Models;
using Npgsql; //new
using server; //new
using BCrypt.Net;

//using server.DatabaseManagement;

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
        }

        //Online users stored in "_connections"
        //Database of past users stored in DB
        //Database of past messages stored in DB


        //CREDENTIALS
        //RETRIEVE
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

        //ADD
        public async void AddCredential(Credential credential)
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

        public void CheckCredential(Credential credential)
        {
            var param = new
            {
                IsValid = IsValid(credential),
                Username = credential.Username
            };
            Clients.User(Context.UserIdentifier).SendAsync("ReturnedIsValid", param);
        }

        //VALIDATE
        private bool IsValid(Credential credential)
        {
            var hashedPassword = RetrieveCredential(credential.Username).Password;
            return BCrypt.Net.BCrypt.Verify(credential.Password, hashedPassword);
            
        }

        //Validate - SEND
        public void ReturnIsValid(Credential credential)
        {
            if (credential.LoginType == "Guest")
                GuestLogin(credential);
            if (credential.LoginType == "Create")
            {
                AddCredential(credential);
                CheckCredential(credential);
            }
            if (credential.LoginType == "Returning")
                CheckCredential(credential);
        }

        public void ReturnPastRooms(string Username)
        {
            var rooms = RetrieveRooms(Username);
            var param = new
            {

            };
            Clients.User(Context.UserIdentifier).SendAsync("ReturnedPastRooms", param);
        }


        //CHANGE


        //GUEST
        public void GuestLogin(Credential credential)
        {
            var param = new
            {
                IsValid = true,
                Username = AppendNumberToUsername(credential.Username)
            };
            Clients.User(Context.UserIdentifier).SendAsync("ReturnedIsValid", param );
        }

        private string AppendNumberToUsername(string username)
        {
            var number = new Random().Next(1000, 10000).ToString();
            return username += number;
        }




        //public async Task JoinRoom(UserConnection userConnection)
        public async Task JoinRoom(UserConnection userConnection)
        {
            _connections[Context.ConnectionId] = userConnection;
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.ActiveRoom);

            List<Message> messages = RetrieveMessages(userConnection.ActiveRoom);
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

        public async Task SendMessage(string message)
        {
            _connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);

            var param = new Message
            {
                Username = userConnection.Username,
                Text = message,
                Created_on = DateTime.Now,
                Room = userConnection.ActiveRoom
            };

            await Clients.All.SendAsync("ReturnedMessage", param); // To Chat
            CreateMessage(param); // To Database
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);
            _connections.Remove(Context.ConnectionId);
            
            if(userConnection != null)
            {
                Clients.Group(userConnection.ActiveRoom).SendAsync("ReturnedMessage", new Message { Username = _botUser, Text = $"{userConnection.Username} has left the room" });
                SendUsersInRoom(userConnection.ActiveRoom);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task SendUsersInRoom(string room)
        {
            var param = _connections.Values
                .Where(connection => connection.ActiveRoom == room)
                .Select(connection => connection.Username)
                .Distinct();
            
            return Clients.Group(room).SendAsync("ReturnedUsers", param);
        }

        public void CreateMessage(Message param)
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

        public List<Message> RetrieveMessages(string room)
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
