using NSubstitute;
using NUnit.Framework;
using server.Hubs.Services;
using server.Models;

namespace Server.Tests
{
    [TestFixture]
    internal class QueryServiceShould
    {
        private IQueryService _sut;

        [SetUp]
        public void Setup()
        {
            _sut = Substitute.For<IQueryService>();
        }

        [Test]
        public void ReturnUserFromUsername_should_be_called()
        {
            var mockUsername = "Test";

            _sut.ReturnUserFromUsername(mockUsername);

            _sut.Received().ReturnUserFromUsername(mockUsername);
        }

        [Test]
        public void DoesUserExist_should_be_called()
        {
            var mockUser = new User();

            _sut.DoesUserExist(mockUser);

            _sut.Received().DoesUserExist(mockUser);
        }

        [Test]
        public void CreateNewUser_should_be_called()
        {
            var mockUser = new User();

            _sut.HandleAddNewUser(mockUser);

            _sut.Received().HandleAddNewUser(mockUser);
        }

        [Test]
        public void ReturnAvailableChannels_List_should_be_called()
        {
            _sut.ReturnAvailableChannels_List();

            _sut.Received().ReturnAvailableChannels_List();
        }

        [Test]
        public void ReturnMessagesByChannel_List_should_be_called()
        {
            var mockChannel = new Channel();

            _sut.ReturnMessagesByChannel_List(mockChannel);

            _sut.Received().ReturnMessagesByChannel_List(mockChannel);
        }

        [Test]
        public void InsertMessage_should_be_called()
        {
            var mockMessage = new Message();

            _sut.InsertMessage(mockMessage);

            _sut.Received().InsertMessage(mockMessage);
        }

        [Test]
        public void UpdatePasswordForUser_should_be_called()
        {
            var mockUser = new User();

            _sut.UpdatePasswordForUser(mockUser);

            _sut.Received().UpdatePasswordForUser(mockUser);
        }
    }
}
