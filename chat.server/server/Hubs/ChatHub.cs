using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using server.Models;
using server.DatabaseManagement;

namespace server.Hubs
{
    public class ChatHub : Hub
    {

        private readonly IDictionary<string, UserConnection> _connections;
        private readonly string _botUser;

        public ChatHub(IDictionary<string, UserConnection> connections)
        {
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

            try
            {
                using (var db = new Database())
                {
                    db.InsertNewMessage(userConnection, message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            

            await Clients.All.SendAsync("ReceiveMessage", userConnection.User, message);
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
