using NSubstitute;
using NUnit.Framework;
using server.Hubs.Services;
using server.Models;
using System;

namespace Server.Tests
{
    
    [TestFixture]
    public class ChannelServiceShould
    {
        private IQueryService _queryService;
        private IChannelService _sut;

        [SetUp]
        public void Setup()
        {
            _queryService = Substitute.For<IQueryService>();
            _sut = new ChannelService(_queryService);
        }

        [Test]
        public void return_zero_messages_if_userId_is_zero()
        {
            Channel testChannel = new();
            var testUserConnection = new UserConnection
            {
                User = new User
                {
                    Id = 0
                }
            };

            var messages = _sut.HandleCreateKnockMessages(testUserConnection,testChannel);

            Assert.That(messages, Is.Not.Null);
            Assert.That(messages.Count, Is.EqualTo(0));
        }
        [Test]
        public void return_two_messages_if_userId_is_not_zero()
        {
            Channel testChannel = new();
            var testUserConnection = new UserConnection
            {
                User = new User
                {
                    Id = 1
                }
            };

            var messages = _sut.HandleCreateKnockMessages(testUserConnection, testChannel);

            Assert.That(messages.Count, Is.EqualTo(2));
        }

        [Test]
        public void insert_message_if_not_isbot()
        {
            var testMessage = new Message
            {
                Text = "TestMessageText",
                Channel = new(),
                IsBot = false
            };
            var testUserConnection = new UserConnection
            {
                User = new()
            };

            _sut.HandleNewMessage(testMessage,testUserConnection);

            _queryService.ReceivedWithAnyArgs(1).InsertMessage(default);
        }

        [Test]
        public void not_insert_message_if_isbot()
        {
            var testMessage = new Message
            {
                Text = "TestMessageText",
                Channel = new(),
                IsBot = true
            };
            var testUserConnection = new UserConnection
            {
                User = new()
            };

            _sut.HandleNewMessage(testMessage, testUserConnection);

            _queryService.DidNotReceiveWithAnyArgs().InsertMessage(default);
        }

        [Test]
        public void return_valid_message()
        {
            var testMessage = new Message
            {
                Text = "TestMessageText",
                Channel = new()
            };
            var testUserConnection = new UserConnection {
                User = new()
            };

            var newMessage = _sut.FormatNewMessage(testMessage, testUserConnection);

            Assert.That(newMessage.Created_on, Is.EqualTo(DateTime.UtcNow).Within(5).Seconds);
            Assert.That(newMessage.Text.Length, Is.AtLeast(1));
            Assert.That(newMessage.User, Is.EqualTo(testUserConnection.User));
            Assert.That(newMessage.Channel, Is.EqualTo(testMessage.Channel));
        }

        [Test]
        public void return_message_channel_if_message_channel_exists()
        {
            var testMessage = new Message
            {
                Channel = new(),
                Text = "TestMessage"
            };
            var testUserConnection = new UserConnection
            {
                User = new()
            };

            var newMessage = _sut.FormatNewMessage(testMessage, testUserConnection);

            Assert.That(newMessage.Channel, Is.EqualTo(testMessage.Channel));
        }

        public void return_userconnection_channel_if_message_channel_not_exists()
        {
            var testMessage = new Message
            {
                Text = "TestMessage"
            };
            var testUserConnection = new UserConnection
            {
                Channel = new(),
                User = new()
            };

            var newMessage = _sut.FormatNewMessage(testMessage, testUserConnection);

            Assert.That(newMessage.Channel, Is.EqualTo(testUserConnection.Channel));
        }

        [Test]
        public void return_isbot_true_if_isbot_is_true()
        {
            var testMessage = new Message
            {
                Channel = new(),
                Text = "TestMessage",
                IsBot = true
            };
            var testUserConnection = new UserConnection
            {
                User = new()
            };

            var newMessage = _sut.FormatNewMessage(testMessage, testUserConnection);

            Assert.That(newMessage.IsBot, Is.EqualTo(true));
        }

        [Test]
        public void return_isbot_false_if_isbot_is_false()
        {
            var testMessage = new Message
            {
                Channel = new(),
                Text = "TestMessage",
                IsBot = false
            };

            var testUserConnection = new UserConnection
            {
                User = new()
            };

            var newMessage = _sut.FormatNewMessage(testMessage, testUserConnection);

            Assert.That(newMessage.IsBot, Is.EqualTo(false));
        }
        [Test]
        public void return_isbot_false_if_isbot_is_missing()
        {
            var testMessage = new Message
            {
                Channel = new(),
                Text = "TestMessage",
            };

            var testUserConnection = new UserConnection
            {
                User = new()
            };

            var newMessage = _sut.FormatNewMessage(testMessage, testUserConnection);

            Assert.That(newMessage.IsBot, Is.EqualTo(false));
        }

        [Test]
        public void throw_exeption_when_text_is_null()
        {
            var testMessage = new Message
            {
                Text = null,
            };
            var testUserConnection = new UserConnection();

            Assert.Throws<ValidationException>(() => _sut.FormatNewMessage(testMessage,testUserConnection));
        }

        [Test]
        public void throw_exeption_when_text_is_zero()
        {
            var testMessage = new Message
            {
                Text = "",
            };
            var testUserConnection = new UserConnection();

            Assert.Throws<ValidationException>(() => _sut.FormatNewMessage(testMessage, testUserConnection));
        }

        [Test]
        public void throw_exeption_when_message_and_userconnection_not_have_channel()
        {
            var testMessage = new Message
            {
                Text = "",
            };
            var testUserConnection = new UserConnection();

            Assert.Throws<ValidationException>(() => _sut.FormatNewMessage(testMessage, testUserConnection));
        }
    }
}
