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
    internal class ConnectionManagementShould
    {
        private IConnectionManagement _sut;
        
        [SetUp]
        public void Setup()
        {
            _sut = Substitute.For<IConnectionManagement>();
        }

        [Test]
        public void UpdateUserConnection_Void_should_be_called()
        {
            var mockConnectionID = Guid.NewGuid().ToString();
            var mockUserConnection = new UserConnection();

            _sut.UpdateUserConnection_Void(mockConnectionID, mockUserConnection);

            _sut.Received().UpdateUserConnection_Void(mockConnectionID, mockUserConnection);
        }

        [Test]
        public void RemoveUserConnection_Void_should_be_called()
        {
            var mockConnectionID = Guid.NewGuid().ToString();

            _sut.RemoveUserConnection_Void(mockConnectionID);

            _sut.Received().RemoveUserConnection_Void(mockConnectionID);
        }

        [Test]
        public void GetUserConnection_UserConnection_should_be_called()
        {
            var mockConnectionID = Guid.NewGuid().ToString();

            _sut.GetUserConnection_UserConnection(mockConnectionID);

            _sut.Received().GetUserConnection_UserConnection(mockConnectionID);
        }

        [Test]
        public void GetUserConnectionsOnChannel_List_should_be_called()
        {
            var mockChannel = new Channel();

            _sut.GetUserConnectionsOnChannel_List(mockChannel);

            _sut.Received().GetUserConnectionsOnChannel_List(mockChannel);
        }

        [Test]
        public void GetAllUserConnections_List_should_be_called()
        {
            var mockChannel = new Channel();

            _sut.GetAllUserConnections_List();

            _sut.Received().GetAllUserConnections_List();
        }
    }
}
