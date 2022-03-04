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

        public void ValidateCredentials(Credential credential)
        {
            bool exists = false;

            using (mySession = mySessionFactory.OpenSession())
            {
                try
                {
                    exists = mySession.Query<Credential>()
                        .Any(x => x.Username == credential.Username && x.Password == credential.Password);
                    mySession.Flush();

                    if (exists)
                        _connections[Context.ConnectionId] = new UserConnection 
                        {
                            Username = credential.Username
                        };
                    
                }
                catch (Exception e)
                {
                    var Error = e;
                }
            }

            Clients.User(Context.UserIdentifier).SendAsync("IsValid", exists, credential.Username);
        }


        //public async Task JoinRoom(UserConnection userConnection)
        public async Task JoinRoom()
        {
            //_connections[Context.ConnectionId] = userConnection;
            _connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);
            userConnection.ActiveRoom = "Code";
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.ActiveRoom);

            List<Message> messages = RetrieveMessages(userConnection.ActiveRoom);
            messages.ForEach(async m =>
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", m.Username, m.Text, m.Created_on)
            );
            
            await Clients.Group(userConnection.ActiveRoom).SendAsync("ReceiveMessage", _botUser, $"{userConnection.Username} has entered {userConnection.ActiveRoom}");
            await SendUsersInRoom(userConnection.ActiveRoom);
        }

        public async Task SendMessage(string message)
        {
            _connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);

            await Clients.All.SendAsync("ReceiveMessage", userConnection.Username, message, DateTime.UtcNow); // To Chat
            CreateMessage(userConnection, message, DateTime.UtcNow); // To Database
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection);
            _connections.Remove(Context.ConnectionId);

            Clients.Group(userConnection.ActiveRoom).SendAsync("ReceiveMessage", _botUser, $"{userConnection.Username} has left the room");

            SendUsersInRoom(userConnection.ActiveRoom);

            return base.OnDisconnectedAsync(exception);
        }

        public Task SendUsersInRoom(string room)
        {
            var users = _connections.Values
                .Where(connection => connection.ActiveRoom == room)
                .Select(connection => connection.Username);

            return Clients.Group(room).SendAsync("ReceiveUsers", users);
        }

        public void CreateMessage(UserConnection userConnection, string message, DateTime dateTime)
        {
            using (mySession = mySessionFactory.OpenSession())
            {
                Message localMessage = new Message
                {
                    Text = message,
                    Username = userConnection.Username,
                    Room = userConnection.ActiveRoom,
                    Created_on = dateTime
                };

                try
                {
                    mySession.Save(localMessage);
                    mySession.Flush(); // New
                    //mySession.GetCurrentTransaction().Commit(); // Is this necessary?
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

        //public string RetrieveScreenName(UserConnection userConnection) // Add "isRegistered" functionality to avoid protected screennames
        //{
        //    //var isRegistered = false;
        //    var screenName = "";

        //    using (mySession = mySessionFactory.OpenSession())
        //    {
        //        try
        //        {
        //            screenName = mySession.Query<Credential>()
        //                 .Where(c => c.Username == UserConnection.Username && c.Password == UserConnection.Password)
        //                 .Select(c => c.ScreenName)
        //                 .ToString();

        //            mySession.Flush(); // New
        //        }
        //        catch (Exception e)
        //        {
        //            var ErrorOnRetrieveCredentials = e;
        //        }

        //        return screenName;
        //    }
        //}
    }
}
