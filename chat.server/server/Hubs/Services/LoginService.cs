using server.Models;
using System;

namespace server.Hubs.Services
{
    public interface ILoginService
    {
        public UserConnection HandleReturnLoginAttempt(User user, string connectionId);
        public UpdatePasswordResponse HandleUpdatePassword(User user, string connectionId);
        public User CreateLoginResponse(User user);
        public bool IsKnownLoginType(string loginType);
        public string CreateRandomUsername(string username);
        public User IsValidUser(User user);
        public bool IsValidPassword(string stringPassword, string hashPassword);
    }

    public class LoginService : ILoginService
    {
        private readonly IQueryService _queryService;
        private readonly IConnectionService _connectionService;
        public LoginService(IQueryService queryServices, IConnectionService connectionService)
        {
            _queryService = queryServices;
            _connectionService = connectionService;
        }
        public UserConnection HandleReturnLoginAttempt(User user, string connectionId)
        {
            UserConnection userConnection = _connectionService.GetUserConnection_UserConnection(connectionId);
            //TEST
            if (userConnection == null) 
                throw new ValidationException("UserConnection Missing"); ;
            userConnection.User.LoginType = user.LoginType;
            //x2
            if (userConnection.User.LoginType == "Guest" || _connectionService.IsUserLoggedIn(user) == false)
                userConnection.User = CreateLoginResponse(user);
            //1
            _connectionService.UpdateUserConnection_Void(connectionId, userConnection);
            return userConnection;
        }

        public UpdatePasswordResponse HandleUpdatePassword(User user, string connectionId)
        {
            UserConnection userConnection = _connectionService.GetUserConnection_UserConnection(connectionId);

            //Do I need this?
            user.Username = userConnection.User.Username;
            user = IsValidUser(user);
            UpdatePasswordResponse response = new UpdatePasswordResponse { IsPasswordApproved = user.IsPasswordValid};
            if (response.IsPasswordApproved == false)
                return response;
            _queryService.UpdatePasswordForUser(user);
            return response;
        }



        public User CreateLoginResponse(User user)
        {
            if (!IsKnownLoginType(user.LoginType))
            {
                user.Password = null;
                user.IsPasswordValid = false;
                return user;
            }
            switch (user.LoginType)
            {
                case "Guest":
                    if (user.Username == "") return user;
                    user.Username = CreateRandomUsername(user.Username);
                    user.Password = user.Username;
                    goto case "Create";
                case "Create":
                    user = _queryService.CreateNewUser(user);
                    goto case "Returning";
                case "Returning":
                    user = IsValidUser(user);
                    return user;
                default:
                    return user;
            }
        }
        public bool IsKnownLoginType(string loginType)
        {
            if (loginType == "Guest") return true;
            if (loginType == "Create") return true;
            if (loginType == "Returning") return true;
            return false;
        }

        public string CreateRandomUsername(string username)
        {
            var number = new Random().Next(1000, 10000).ToString();
            var newUsername = username += number;
            return newUsername;
        }
        
        public User IsValidUser(User user)
        {
            var localUser = _queryService.ReturnUserFromUsername(user.Username);
            //TEST
            if (localUser == null)
            {
                user.IsPasswordValid = false;
                return user;
            }
            //TEST x2
            user.IsPasswordValid = IsValidPassword(user.Password, localUser.Password);
            //TEST
            user.Id = user.IsPasswordValid ? localUser.Id : 0;
            //TEST
            user.Password = null;
            return user;
        }

        public bool IsValidPassword(string stringPassword, string hashPassword)
        {
            bool isValid = false;
            try
            {
                isValid = BCrypt.Net.BCrypt.Verify(stringPassword, hashPassword);
            }
            catch (Exception x)
            {
                if (x.Message == "Invalid salt version")
                    return false;

                throw new ValidationException("Unable to verify password");
            }
            return isValid;
        }
    }
}
