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
        public virtual Channel ActiveRoom { get; set; }
    }
}
