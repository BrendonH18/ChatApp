using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace server.Models
{
    public class Credential
    {
        public virtual int Id { get; set; }
        [Required]
        public virtual string Username { get; set; }
        [Required]
        public virtual string Password { get; set; }
        public virtual string ScreenName { get; set; }
        public virtual string LoginType { get; set; }
    }
}
