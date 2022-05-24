using server.Models;
using System;

namespace server.Hubs.HubSupport
{
    public class LoginManagement
    {
        private readonly QueryManagement _queryManagement;
        public LoginManagement(QueryManagement queryManagement)
        {
            _queryManagement = queryManagement;
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
                    user = AppendNumberToUsername(user);
                    user.Password = user.Username;
                    goto case "Create";
                case "Create":
                    user = _queryManagement.CreateNewUser(user);
                    goto case "Returning";
                case "Returning":
                    user = IsValidUser(user);
                    user.Password = null;
                    return user;
                case "Update":
                    goto case "Returning";          
                default:
                    return user;
            }
        }
        public bool IsKnownLoginType(string loginType)
        {
            if (loginType == "Guest") return true;
            if (loginType == "Create") return true;
            if (loginType == "Returning") return true;
            if (loginType == "Update") return true;
            return false;
        }

        private User AppendNumberToUsername(User user)
        {
            var number = new Random().Next(1000, 10000).ToString();
            user.Username = user.Username += number;
            return user;
        }
        
        private User IsValidUser(User user)
        {
            var localUser = _queryManagement.ReturnUserFromUsername(user.Username);
            if (localUser == null)
            {
                user.IsPasswordValid = false;
                return user;
            }
            user.IsPasswordValid = IsValidPassword(user.Password, localUser.Password);
            user.Id = user.IsPasswordValid ? localUser.Id : 0;
            return user;
        }

        public bool IsValidPassword(string stringPassword, string hashPassword)
        {
            bool isValid = BCrypt.Net.BCrypt.Verify(stringPassword,hashPassword);
            return isValid;
        }
    }
}
