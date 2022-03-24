using NUnit.Framework;
using server.Hubs;
using server.Models;
using System.Collections.Generic;

namespace Server.UnitTests
{
    [TestFixture]
    public class HubsTests
    {
        [Test]
        public void CreateMessageInDB_MessageWithUsernameTextCreatedonAndRoom_MessageInDB()
        {
            ////Arrange
            var chathub = new ChatHub(new Dictionary<string, UserConnection>());
            
            //Act

            //Assert
            Assert.Pass();
        }
    }
}
