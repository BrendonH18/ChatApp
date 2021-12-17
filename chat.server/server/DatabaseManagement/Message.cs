using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server
{
    public class Message
    {
        public virtual int Id { get; set; }
        public virtual string User { get; set; }
        public virtual string Text { get; set; }
        public virtual string Room { get; set; }

        //public override string ToString()
        //{
        //    return string.Format("Id ({0}):\n\t{1}\n\t(Text: {2})\n\t(Room: {3})",Id, User, Text, Room);
        //}
    }
}
