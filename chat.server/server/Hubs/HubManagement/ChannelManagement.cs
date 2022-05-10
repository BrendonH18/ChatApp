using server.Models;
using System;

namespace server.Hubs.HubSupport
{
    public class ChannelManagement
    {
        public Message FormatNewMessage(string messageText, UserConnection userConnection, Channel specificChannel)
        {
            Message message = new Message();
            message.Created_on = DateTime.UtcNow;
            message.User = userConnection.User;
            message.Channel = userConnection.Channel;
            if (specificChannel != null)
                message.Channel = specificChannel;
            message.Text = messageText;
            return message;
        }
    }
}
