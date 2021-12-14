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
            //myConfiguration.AddAssembly(typeof(Message).Assembly); //NEW
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

        public async Task JoinRoom(UserConnection userConnection)
        {
            _connections[Context.ConnectionId] = userConnection;

            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);
            
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

            await Clients.All.SendAsync("ReceiveMessage", userConnection.User, message);
            
            Exception error1;
            
            using (mySession = mySessionFactory.OpenSession())
            {
                Message loMessage = new Message
                {
                    Text = message,
                    User = userConnection.User,
                    Room = userConnection.Room
                };

                try
                {
                    mySession.Save(loMessage); //exception
                    mySession.Flush(); // New
                    mySession.GetCurrentTransaction().Commit(); // Is this necessary?
                }
                catch (Exception e)
                {
                    error1 = e;
                }
            }

            Exception error2;

            using (var con = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Username=postgres;Password=password;Database=nhibernatedemo"))
            {
                
                try
                {
                    using (var cmd = new NpgsqlCommand())
                    {
                        con.Open();
                        cmd.Connection = con;
                        
                        cmd.CommandText = $"INSERT INTO messsage (room, user, text) VALUES ('{userConnection.Room}', '{userConnection.User}' , '{message}')";

                        cmd.ExecuteNonQuery();

                        con.Close();
                        Console.WriteLine($"PostgreSQL Command: {cmd.CommandText}");
                    }
                }
                catch (Exception e)
                {
                    error2 = e;
                }
                
            }
            
            Console.WriteLine("Pause");
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
            var users= _connections.Values
                .Where(connection => connection.Room == room)
                .Select(connection => connection.User)


            ;
            //foreach (var item in RoomUsers)
            //{
            //    users.Add(item.Value.User);
            //}

                //.Select(connection => connection.User);

            //var connectionIds = _connections.Values
            //    .Where(connection => connection.

            return Clients.Group(room).SendAsync("ReceiveUsers", users);
        }
    }
}
