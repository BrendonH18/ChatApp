using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace server.Models
{
    public class User
    {
        public virtual int Id { get; set; }
        [Required]
        public virtual string Username { get; set; }
        [Required]
        public virtual string Password { get; set; }
        public virtual string LoginType { get; set; }
        public virtual bool IsPasswordValid { get; set; }
        public virtual string NewPassword { get; set; }
    }
}
