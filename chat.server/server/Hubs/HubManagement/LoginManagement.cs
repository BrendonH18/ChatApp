using server.Models;
using System;

namespace server.Hubs.HubManagement
{
    public class LoginManagement
    {
        private readonly IQueryManagement _queryManagement;
        public LoginManagement(IQueryManagement queryManagement)
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
                    if (user.Username == "") return user;
                    user.Username = CreateRandomUsername(user.Username);
                    user.Password = user.Username;
                    goto case "Create";
                case "Create":
                    user = _queryManagement.CreateNewUser(user);
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
            var localUser = _queryManagement.ReturnUserFromUsername(user.Username);
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
