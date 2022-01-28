using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


// The UserDTO represents the user data transfer object and contains three string properties: UserName, Password, and Role. You'll use this class at several places in your application.

namespace server.Models
{
    public class UserDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ActiveRoom { get; set; }
        public string Role { get; set; }
    }
}
