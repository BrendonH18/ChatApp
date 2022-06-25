using server.Models;

namespace server.Hubs.Services
{
    public interface ISetupService
    {
        public void StoreInitialConnectionData(string ConnectionId);
        public UserConnection HandleLogout(UserConnection userConnection, string ConnectionId);
    }

    public class SetupService : ISetupService
    {
        private IConnectionService _connectionService;
        private User _user;
        private Channel _channel;
        public SetupService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
            _user = new User { Id = 0, IsPasswordValid = false, LoginType = "", Password = "", Username = "" };
            _channel = new Channel { Id = 0, Name = "" };
        }
        public void StoreInitialConnectionData(string ConnectionId)
        {
            if (_connectionService.GetUserConnection_UserConnection(ConnectionId) == null)
                _connectionService.UpdateUserConnection_Void(ConnectionId, new UserConnection { Channel = _channel, User = _user });
        }
        public UserConnection HandleLogout(UserConnection userConnection, string ConnectionId)
        {
            userConnection.User = _user;
            _connectionService.UpdateUserConnection_Void(ConnectionId, userConnection);
            return userConnection;
        }

    }
}
