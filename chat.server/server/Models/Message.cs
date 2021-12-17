using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Models
{
    public class Message
    {
        public virtual int Id { get; set; }
        public virtual string User { get; set; }
        public virtual string Text { get; set; }
        public virtual string Room { get; set; }
    }
}
