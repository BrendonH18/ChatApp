using NHibernate;
using NSubstitute;
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
    internal class ChatHubShould
    {
        private IConnectionManagement _connectionManagement;
        private User _user;
        private Channel _channel;
        private LoginManagement _loginManagement;
        private IQueryManagement _queryManagement;
        private ChannelManagement _channelManagement;

        [SetUp]
        public void Setup()
        {
            _connectionManagement = Substitute.For<IConnectionManagement>();
            //_user = new User { Id = 0, IsPasswordValid = false, LoginType = "", Password = "", Username = "" };
            //_channel = new Channel { Id = 0, Name = "" };
            var _factory = Substitute.For<ISessionFactory>();
            _queryManagement = Substitute.For<IQueryManagement>();
            _loginManagement = new LoginManagement(_queryManagement);
            _channelManagement = new ChannelManagement();
        }
        [Test]
        public void OnConnectedAsync_should_call_GetUserConnection_UserConnection_if_new_connection()
        {
            Assert.Fail();
        }
        [Test]
        public void OnConnectedAsync_should_call_nothing_if_established_connection()
        {
            Assert.Fail();
        }
        [Test]
        public void ConnectionSetup_should_unsure()
        {
            Assert.Fail();
        }
        [Test]
        public void ReturnAvailableChannels_should_call_ReturnAvailableChannels_List()
        {
            Assert.Fail();
        }
        [Test]
        public void UpdatePassword_should_unsure()
        {
            Assert.Fail();
        }
        [Test]
        public void JoinChannel_should_unsure()
        {
            Assert.Fail();
        }
        [Test]
        public void ReturnLoginAttempt_should_unsure()
        {
            Assert.Fail();
        }
        [Test]
        public void SendConnectedUsers_should_unsure()
        {
            Assert.Fail();
        }
        [Test]
        public void SendMessageToChannel_should_unsure()
        {
            Assert.Fail();
        }
        [Test]
        public void Logout_should_unsure()
        {
            Assert.Fail();
        }
        [Test]
        public void OnDisconnectedAsync_should_unsure()
        {
            Assert.Fail();
        }
    }
}
