using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Models
{
    public class Message
    {
        public virtual int Id { get; set; }
        public virtual User User { get; set; }
        public virtual Channel Channel { get; set; }
        //public virtual string Username { get; set; }
        public virtual string Text { get; set; }
        //public virtual string Channel_name { get; set; }
        public virtual DateTime Created_on { get; set; }
    }
}
