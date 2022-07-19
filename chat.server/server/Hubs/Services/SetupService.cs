using server.Models;

namespace server.Hubs.Services
{
    public interface ISetupService
    {
        public void StoreInitialConnectionData(User user);
        public UserConnection HandleLogout(UserConnection userConnection, string userName);
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
        public void StoreInitialConnectionData(User user)
        {
            //_connectionService.UpdateUserConnection_Void(user.Username, new UserConnection { User = new User() });
        }
        public UserConnection HandleLogout(UserConnection userConnection, string ConnectionId)
        {
            //userConnection.User = _user;
            //_connectionService.UpdateUserConnection_Void(ConnectionId, userConnection);
            //return userConnection;
            return new UserConnection();
        }

    }
}
