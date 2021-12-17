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

        public async Task ValidateUser(Credential credential)
        {
            string screenName = RetrieveScreenName(credential);

            Tuple<bool, string> validationResult = screenName == "" ?
                new Tuple<bool, string>(false, "") :
                new Tuple<bool, string>(true, screenName);

            // front end call
        }


        public async Task JoinRoom(UserConnection userConnection)
        {
            _connections[Context.ConnectionId] = userConnection;
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            List<Message> messages = RetrieveMessages(userConnection.Room);
            messages.ForEach(async m =>
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", m.User, m.Text)
            );
            
            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has entered {userConnection.Room}");
            await SendUsersInRoom(userConnection.Room);
        }

        public async Task SendMessage(string message)
        {
            if (!_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                userConnection = new UserConnection
                {
                    User = _botUser,
                    Room = "Default"
                };
            }

            await Clients.All.SendAsync("ReceiveMessage", userConnection.User, message); // Chat
            CreateMessage(userConnection, message); // Database
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
                _connections.Remove(Context.ConnectionId);

            Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has left the room");

            SendUsersInRoom(userConnection.Room);

            return base.OnDisconnectedAsync(exception);
        }

        public Task SendUsersInRoom(string room)
        {
            var users = _connections.Values
                .Where(connection => connection.Room == room)
                .Select(connection => connection.User);

            return Clients.Group(room).SendAsync("ReceiveUsers", users);
        }

        public void CreateMessage(UserConnection userConnection, string message)
        {
            using (mySession = mySessionFactory.OpenSession())
            {
                Message localMessage = new Message
                {
                    Text = message,
                    User = userConnection.User,
                    Room = userConnection.Room
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
                        .Where(m => m.User == user)
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

        public string RetrieveScreenName(Credential credential) // Add "isRegistered" functionality to avoid protected screennames
        {
            //var isRegistered = false;
            var screenName = "";

            using (mySession = mySessionFactory.OpenSession())
            {
                try
                {
                    screenName = mySession.Query<Credential>()
                         .Where(c => c.Username == credential.Username && c.Password == credential.Password)
                         .Select(c => c.ScreenName)
                         .ToString();

                    mySession.Flush(); // New
                }
                catch (Exception e)
                {
                    var ErrorOnRetrieveCredentials = e;
                }

                return screenName;
            }
        }
    }
}
