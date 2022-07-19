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
using System.Threading.Tasks;

namespace server.Hubs.Services
{
    public interface ILoginService
    {
        public Task HandleReturnLoginAttempt(User user, out UserModel returnUser, out string token);
        public UpdatePasswordResponse HandleUpdatePassword(User user, string userName);
        public string CreateRandomUsername(string username);
        public bool IsValidPassword(string stringPassword, string hashPassword);
        public string GenerateJWT(User user);
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
        public Task HandleReturnLoginAttempt(User user, out UserModel returnUser, out string token)
        {

            returnUser = new UserModel();
            token = String.Empty;

            if (_connectionService.IsUserLoggedIn(user))
                return Task.CompletedTask;
            CreateLoginResponse(user, ref returnUser, ref token);
            if ( token.Length != 0 )_connectionService.AddUserToServer(user);
            //_setupService.StoreInitialConnectionData(user); // Why not just one fuction "StoreConnectionData" ?
            return Task.CompletedTask;
        }

        public UpdatePasswordResponse HandleUpdatePassword(User user, string connectionId)
        {
            return new UpdatePasswordResponse();
            //UserConnection userConnection = _connectionService.GetUserConnection_UserConnection(connectionId);

            ////Do I need this?
            //user.Username = userConnection.User.Username;
            //user = IsValidUser(user);
            //UpdatePasswordResponse response = new UpdatePasswordResponse { IsPasswordApproved = user.IsPasswordValid};
            //if (response.IsPasswordApproved == false)
            //    return response;
            //_queryService.UpdatePasswordForUser(user);
            //return response;
        }



        public void CreateLoginResponse(User user, ref UserModel returnUser, ref string token)
        {
            switch (user.LoginType)
            {
                case "Guest":
                    if (user.Username == "") return;
                    user.Username = CreateRandomUsername(user.Username);
                    user.Password = user.Username;
                    goto case "Create";
                case "Create":
                    _queryService.HandleAddNewUser(user);
                    goto case "Returning";
                case "Returning":
                    var localUser = _queryService.ReturnUserFromUsername(user.Username);
                    if (localUser == null) return;
                    if (!IsValidPassword(user.Password, localUser.Password)) return;
                    token = GenerateJWT(localUser);
                    returnUser.Id = localUser.Id;
                    returnUser.Username = localUser.Username;
                    return;
                default:
                    return;
            }
        }

        public string CreateRandomUsername(string username)
        {
            var number = new Random().Next(1000, 10000).ToString();
            var newUsername = username += number;
            return newUsername;
        }
        
        
        public bool IsValidUser(User user)
        {
            var localUser = _queryService.ReturnUserFromUsername(user.Username);
            if (localUser == null) return false;
            return IsValidPassword(user.Password, localUser.Password);
        }

        public bool IsValidPassword(string stringPassword, string hashPassword)
        {
            return BCrypt.Net.BCrypt.Verify(stringPassword, hashPassword);
        }

        public string GenerateJWT(User user)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
            };

                var token = new JwtSecurityToken(
                    _config["Jwt:Issuer"],
                    _config["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddSeconds(5),
                    signingCredentials: credentials
                    );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
