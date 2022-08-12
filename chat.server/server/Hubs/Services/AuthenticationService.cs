using Microsoft.Extensions.Configuration;
using server.Models;
using System;
using System.Collections.Generic;

namespace server.Hubs.Services
{
    public interface IAuthenticationService
    {
        
    }

    public class AuthenticationService : IAuthenticationService
    {
        private IConfiguration _config;
        public AuthenticationService(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void Login(User user)
        {
            var user1 = Authenticate(user);
            //Generate();
        }

        public bool Authenticate(User user)
        {
            return true;
        }

        public bool IsAuthenticated(User user)
        {
            return true;
        }
    }
}
