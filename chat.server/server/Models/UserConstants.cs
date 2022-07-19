using System.Collections.Generic;

namespace server.Models
{
    public class UserConstants
    {
        public static List<User> Users = new List<User>()
        {
            new User()
            {
                Username = "jason_admin",
                Password = "MyPass_w0rd",
            },
            new User()
            {
                Username = "brendon_seller",
                Password = "MyPass_w0rd",
            }
        };
    }
}
