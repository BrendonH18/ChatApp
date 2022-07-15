using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using server.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        private readonly ISetupService _setupService;
        private readonly IConfiguration _config;
        public LoginService(IQueryService queryServices, IConnectionService connectionService, ISetupService setupService, IConfiguration config)
        {
            _queryService = queryServices;
            _connectionService = connectionService;
            _setupService = setupService;
            _config = config;
        }
        public UserConnection HandleReturnLoginAttempt(User user, string connectionId)
        {
            _setupService.StoreInitialConnectionData(connectionId);
            UserConnection userConnection = _connectionService.GetUserConnection_UserConnection(connectionId);
            userConnection.User.LoginType = user.LoginType;
            //x2
            if (!_connectionService.IsUserLoggedIn(user))
                userConnection.User = CreateLoginResponse(user);
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

        //public IActionResult Login(UserLogin userLogin)
        //{
        //    var user = Authenticate(userLogin);
        //    if (user == null)
        //        return NotFound("User not found.");
        //    if (user.IsPasswordValid == false)
        //        return NotFound("Invalid Password/Username");
        //    var token = Generate(user);
        //    return Ok(token);
        //}

        //private string Generate(User user)
        //{
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, user.Username),
        //    };

        //    var token = new JwtSecurityToken(
        //        _config["Jwt:Issuer"],
        //        _config["Jwt:Audience"],
        //        claims,
        //        expires: DateTime.Now.AddHours(3),
        //        signingCredentials: credentials
        //        );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        //private User Authenticate(UserLogin userLogin)
        //{
        //    var user = _queryService.ReturnUserFromUsername(userLogin.UserName);
        //    //TEST
        //    if (user == null)
        //        return user;
        //    //TEST x2
        //    user.IsPasswordValid = BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password);
        //    //TEST
        //    user.Id = user.IsPasswordValid ? user.Id : 0;
        //    //TEST
        //    return user;

        //    //var currentUser = UserConstants.Users.FirstOrDefault(o => o.UserName.ToLower() == userLogin.UserName.ToLower() && o.Password == userLogin.Password);
        //    //if (currentUser == null)
        //    //    return null;
        //    //return currentUser;
        //}
    }
}
