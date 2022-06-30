using System.Collections.Generic;

namespace server.Models
{
    public class UserConstants
    {
        public static List<UserModel> Users = new List<UserModel>()
        {
            new UserModel()
            {
                UserName = "jason_admin",
                EmailAddress = "jason.admin@gmail.com",
                Password = "MyPass_w0rd",
                GivenName = "Jason",
                Surname =  "Bryant",
                Role = "Administrator"
            },
            new UserModel()
            {
                UserName = "brendon_seller",
                EmailAddress = "brendon.seller@gmail.com",
                Password = "MyPass_w0rd",
                GivenName = "Brendon",
                Surname =  "Hall",
                Role = "Seller"
            }
        };
    }
}
