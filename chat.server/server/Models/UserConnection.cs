using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Models
{
    public class UserConnection
    {
        public virtual User User { get; set; }
        public virtual Channel Channel { get; set; }
    }
}
