using Microsoft.AspNet.SignalR.Hubs;
using NHibernate;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using server.Hubs;
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
        //private User _user;
        //private Channel _channel;
        //private LoginManagement _loginManagement;
        //private IQueryManagement _queryManagement;
        //private ChannelManagement _channelManagement;
        private ChatHub _sut;

        [SetUp]
        public void Setup()
        {
            _connectionManagement = Substitute.For<IConnectionManagement>();
            var _factory = Substitute.For<ISessionFactory>();
            //_queryManagement = Substitute.For<IQueryManagement>();
            //_loginManagement = new LoginManagement(_queryManagement);
            //_channelManagement = new ChannelManagement();
            _sut = new ChatHub(_connectionManagement, _factory);
        }
        
        [Test]
        public void OnConnectedAsync_should_call_GetUserConnection_UserConnection_if_new_connection()
        {
            //var mockClients = Substitute.For<IHubCallerConnectionContext<dynamic>>();
            //_connectionManagement.GetUserConnection_UserConnection("ABC").ReturnsNull();
            //Substitute.For<HubCallerContext>().ConnectionId.Returns("ABC");
            //mockClientContext.ConnectionId.Returns("ABC");
            //Guid guid = Guid.NewGuid();
            //_connectionManagement.GetUserConnection_UserConnection(guid.ToString()).Returns(new UserConnection());


            //_sut.OnConnectedAsync();

            //_connectionManagement.Received().GetUserConnection_UserConnection("ABC");
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
