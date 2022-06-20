using server.Models;
using System;

namespace server.Hubs.HubManagement
{
    public class ChannelManagement
    {
        public Message FormatNewMessage(Message paramMessage, UserConnection userConnection)
        {
            if (paramMessage.Text == null)
                throw new ValidationException("Message text cannot be NULL");
            if (paramMessage.Text.Length == 0)
                throw new ValidationException("Message text length cannot be ZERO");
            Message loMessage = new();
            loMessage.Created_on = DateTime.UtcNow;
            loMessage.User = userConnection.User;
            loMessage.Channel = paramMessage.Channel ?? userConnection.Channel;
            loMessage.Text = paramMessage.Text;
            loMessage.IsBot = paramMessage.IsBot;
            return loMessage;
        }
    }
}
