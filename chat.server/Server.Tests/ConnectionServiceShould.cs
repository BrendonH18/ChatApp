using NSubstitute;
using NUnit.Framework;
using server.Hubs.Services;
using server.Models;
using System;

namespace Server.Tests
{
    [TestFixture]
    internal class ConnectionServiceShould
    {
        private IConnectionService _sut;
        
        [SetUp]
        public void Setup()
        {
            _sut = Substitute.For<IConnectionService>();
        }

        [Test]
        public void UpdateUserConnection_Void_should_be_called()
        {
            var mockConnectionID = Guid.NewGuid().ToString();
            var mockUserConnection = new UserConnection();

            _sut.UpdateUserConnection_Void(mockConnectionID, mockUserConnection);

            _sut.Received(1).UpdateUserConnection_Void(mockConnectionID, mockUserConnection);
        }

        [Test]
        public void RemoveUserConnection_Void_should_be_called()
        {
            var mockConnectionID = Guid.NewGuid().ToString();

            _sut.RemoveUser(mockConnectionID);

            _sut.Received(1).RemoveUser(mockConnectionID);
        }

        [Test]
        public void GetUserConnection_UserConnection_should_be_called()
        {
            var mockConnectionID = Guid.NewGuid().ToString();

            _sut.GetUser_Username(mockConnectionID);

            _sut.Received(1).GetUser_Username(mockConnectionID);
        }

        [Test]
        public void GetUserConnectionsOnChannel_List_should_be_called()
        {
            var mockChannel = new Channel();

            _sut.GetUsersOnChannel_List(mockChannel);

            _sut.Received(1).GetUsersOnChannel_List(mockChannel);
        }

        [Test]
        public void GetAllUserConnections_List_should_be_called()
        {
            var mockChannel = new Channel();

            _sut.GetAllUserConnections_List();

            _sut.Received(1).GetAllUserConnections_List();
        }
    }
}
