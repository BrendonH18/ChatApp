using NHibernate;
using server.Models;
using System;
using System.Linq;

namespace server.Hubs.HubSupport
{
    public class LoginManagement
    {
        private readonly DatabaseQueries db;
        public LoginManagement(ISessionFactory factory)
        {
            DatabaseQueries db = new DatabaseQueries(factory);
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
                    user = db.CreateUserInDB(user);
                    user.Password = user.Username;
                    goto case "Returning";
                case "Returning":
                    user = IsValid(user);
                    user.Password = null;
                    return user;
                case "Update":
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
            if (loginType == "Update") return true;
            return false;
        }

        private User AppendNumberToUsername(User user)
        {
            var number = new Random().Next(1000, 10000).ToString();
            user.Username = user.Username += number;
            return user;
        }
        
        private User IsValid(User user)
        {
            var localUser = db.RetrieveCredential(user.Username);
            if (localUser == null)
            {
                user.IsPasswordValid = false;
                return user;
            }
            user.IsPasswordValid = BCrypt.Net.BCrypt.Verify(user.Password, localUser.Password);
            user.Id = user.IsPasswordValid ? localUser.Id : 0;
            return user;
        }
    }
}
