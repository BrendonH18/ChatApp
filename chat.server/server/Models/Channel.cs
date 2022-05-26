using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace server.Models
{
    public class Channel
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Image { get; set; }
    }
}
