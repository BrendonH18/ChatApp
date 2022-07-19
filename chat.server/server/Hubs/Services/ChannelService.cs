using server.Models;
using System;
using System.Collections.Generic;

namespace server.Hubs.Services
{
    public interface IChannelService
    {
        public List<Message> HandleCreateKnockMessages(User userConnection, Channel enterChannel, Channel exitChannel);
        public Message HandleNewMessage(Message message);
        public Message FormatNewMessage(string text, User user, Channel channel, bool isBot = false);
    }

    public class ChannelService : IChannelService
    {
        private IQueryService _queryService;
        public ChannelService(IQueryService queryService)
        {
            _queryService = queryService;
        }
        public List<Message> HandleCreateKnockMessages(User user, Channel enterChannel, Channel exitChannel)
        {
            List<Message> messages = new();
            if (user == null)
                return messages;
            messages.Add(new Message { 
                IsBot = true,
                Channel = enterChannel,
                Text = $"{user.Username} has entered {enterChannel.Name}" 
            });
            if (exitChannel != null) 
                messages.Add(new Message
                {
                    IsBot = true,
                    Channel = exitChannel,
                    Text = $"{user.Username} has left {exitChannel.Name}"
                });
            return messages;
        }

        public Message HandleNewMessage(Message message)
        {
            message.FormatMessage();
            if (!message.IsBot)
                _queryService.InsertMessage(message);
            return message;
        }

        public Message FormatNewMessage(string text, User user, Channel channel, bool isBot = false)
        {
            if (text == null)
                throw new ValidationException("Message text cannot be NULL");
            if (text.Length == 0)
                throw new ValidationException("Message text length cannot be ZERO");
            if(channel == null)
                throw new ValidationException("Message channel cannot be NULL");
            Message message = new();
            message.Created_on = DateTime.UtcNow;
            message.User = user;
            message.Channel = channel;
            message.Text = text;
            message.IsBot = isBot;
            return message;
        }
    }
}
