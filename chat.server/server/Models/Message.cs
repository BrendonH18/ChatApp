using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public virtual bool IsBot { get; set; }

        //string text, User user, Channel channel, bool isBot = false
        public virtual Task FormatMessage()
        {
            if (Text == null)
                throw new ValidationException("Message text cannot be NULL");
            if (Text.Length == 0)
                throw new ValidationException("Message text length cannot be ZERO");
            if (Channel == null)
                throw new ValidationException("Message channel cannot be NULL");
            if (Channel.Name.Length == 0)
                throw new ValidationException("Message channel cannot be NULL");
            if (User == null)
                throw new ValidationException("Message user cannot be NULL");
            if (User.Username.Length == 0)
                throw new ValidationException("Message user cannot be NULL");
            Created_on = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }
}
