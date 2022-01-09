using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Models
{
    public class UserConnection
    {
        public virtual int Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string ScreenName { get; set; }
        public virtual bool is_guest { get; set; }
        public virtual string ActiveRoom { get; set; }
        public virtual List<string> AvailableRooms { get; set; }
    }
}
