using Microsoft.AspNetCore.SignalR;
using server.Models;

namespace server.Hubs.HubSupport
{
    public class ChannelManagement : Hub
    {
        private readonly string _connectionId;
        public ChannelManagement(string connectionId)
        {
            string _connectionId = connectionId;
        }
        public void ReturnedUser(User loginResponse)
        {
            Clients.Client(Context.ConnectionId).SendAsync("ReturnedUser", loginResponse);
        }
        
    }
}
