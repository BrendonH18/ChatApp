using NUnit.Framework;
using server.Hubs.HubManagement;
using server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Tests
{
    
    [TestFixture]
    public class ChannelManagementShould
    {
        private ChannelManagement _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new();
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
    }
}
