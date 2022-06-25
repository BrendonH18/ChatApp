using server.Models;
using System;
using System.Collections.Generic;

namespace server.Hubs.Services
{
    public interface IChannelService
    {
        public List<Message> HandleCreateKnockMessages(UserConnection userConnection, Channel newChannel);
        public Message HandleNewMessage(Message paramMessage, UserConnection userConnection);
        public Message FormatNewMessage(Message paramMessage, UserConnection userConnection);
    }

    public class ChannelService : IChannelService
    {
        private IQueryService _queryService;
        public ChannelService(IQueryService queryService)
        {
            _queryService = queryService;
        }
        public List<Message> HandleCreateKnockMessages(UserConnection userConnection, Channel newChannel)
        {
            List<Message> messages = new();
            if (userConnection.User.Id == 0)
                return messages;
            messages.Add(new Message { 
                IsBot = true,
                Channel = newChannel,
                Text = $"{userConnection.User.Username} has entered {newChannel.Name}" 
            });
            messages.Add(new Message
            {
                IsBot = true,
                Channel = userConnection.Channel,
                Text = $"{userConnection.User.Username} has left {newChannel.Name}"
            });
            return messages;
        }

        public Message HandleNewMessage(Message paramMessage, UserConnection userConnection)
        {
            Message response = FormatNewMessage(paramMessage, userConnection);
            if (!response.IsBot)
                _queryService.InsertMessage(response);
            return response;
        }

        public Message FormatNewMessage(Message paramMessage, UserConnection userConnection)
        {
            if (paramMessage.Text == null)
                throw new ValidationException("Message text cannot be NULL");
            if (paramMessage.Text.Length == 0)
                throw new ValidationException("Message text length cannot be ZERO");
            if(paramMessage.Channel == null && userConnection.Channel == null)
                throw new ValidationException("No parameter contains a Channel");
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
