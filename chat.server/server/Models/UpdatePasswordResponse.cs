using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace server.Models
{
    public class UpdatePasswordResponse
    {
        public virtual bool IsPasswordApproved { get; set; }
    }
}
