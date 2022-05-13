using server.Models;
using System;

namespace server.Hubs.HubSupport
{
    public class ChannelManagement
    {
        public Message FormatNewMessage(Message paramMessage, UserConnection userConnection)
        {
            Message loMessage = new Message();
            loMessage.Created_on = DateTime.UtcNow;
            loMessage.User = userConnection.User;
            loMessage.Channel = userConnection.Channel;
            if (paramMessage.Channel != null)
                loMessage.Channel = paramMessage.Channel;
            loMessage.Text = paramMessage.Text;
            loMessage.isBot = paramMessage.isBot;
            return loMessage;
        }
    }
}
