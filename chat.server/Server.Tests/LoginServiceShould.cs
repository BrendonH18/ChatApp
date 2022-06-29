using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using server.Hubs.Services;
using server.Models;
using System;

namespace Server.Tests
{
    [TestFixture]
    internal class LoginServiceShould
    {
        private IQueryService _queryService;
        private IConnectionService _connectionService;
        private ILoginService _sut;
        private ISetupService _setupService;

        [SetUp]
        public void Setup()
        {
            _queryService = Substitute.For<IQueryService>();
            _connectionService = Substitute.For<IConnectionService>();
            _setupService = Substitute.For<ISetupService>();
            _sut = new LoginService(_queryService, _connectionService, _setupService);
        }

        [Test]
        public void HandleReturnLoginAttempt_calls_CreateNewUser_if_IsUserLoggedIn_false()
        {
            string connectionId = Guid.NewGuid().ToString();
            User user = new User
            {
                LoginType = "Create",
                Password = "Test"
            };
            UserConnection userConnection = new UserConnection
            {
                User = user
            };
            _connectionService.GetUserConnection_UserConnection(connectionId).Returns(userConnection);
            _connectionService.IsUserLoggedIn(user).Returns(false);
            _queryService.CreateNewUser(user).Returns(user);
            _queryService.ReturnUserFromUsername(default).Returns(new User { Password = BCrypt.Net.BCrypt.HashPassword(userConnection.User.Password)});

            _sut.HandleReturnLoginAttempt(user, connectionId);

            _queryService.Received(1).CreateNewUser(user);
        }

        [Test]
        public void HandleReturnLoginAttempt_not_calls_CreateNewUser_if_IsUserLoggedIn_true()
        {
            string connectionId = Guid.NewGuid().ToString();
            User user = new User
            {
                LoginType = "Create",
                Password = "Test"
            };
            UserConnection userConnection = new UserConnection
            {
                User = user
            };
            _connectionService.GetUserConnection_UserConnection(connectionId).Returns(userConnection);
            _connectionService.IsUserLoggedIn(user).Returns(true);
            _queryService.CreateNewUser(user).Returns(user);
            _queryService.ReturnUserFromUsername(default).Returns(new User { Password = BCrypt.Net.BCrypt.HashPassword(userConnection.User.Password) });

            _sut.HandleReturnLoginAttempt(user, connectionId);

            _queryService.DidNotReceive().CreateNewUser(user);
        }

        [Test]
        public void HandleUpdatePassword_calls_UpdatePasswordForUser_if_true()
        {
            string connectionId = Guid.NewGuid().ToString();
            UserConnection userConnection = new UserConnection { 
                User = new User
                {
                    Username = "Test"
                }
            };
            User user1 = new User
            {
                Password = "TEST"
            };
            User user2 = new User 
            { 
                Password = BCrypt.Net.BCrypt.HashPassword(user1.Password)
            };
            _connectionService.GetUserConnection_UserConnection(connectionId).Returns(userConnection);
            _queryService.ReturnUserFromUsername(userConnection.User.Username).Returns(user2);

            _sut.HandleUpdatePassword(user1, connectionId);

            _queryService.ReceivedWithAnyArgs(1).UpdatePasswordForUser(default);
        }

        [Test]
        public void HandleUpdatePassword_not_calls_UpdatePasswordForUser_if_false()
        {
            string connectionId = Guid.NewGuid().ToString();
            UserConnection userConnection = new UserConnection
            {
                User = new User
                {
                    Username = "Test"
                }
            };
            User user1 = new User
            {
                Password = "TEST"
            };
            User user2 = new User
            {
                Password = BCrypt.Net.BCrypt.HashPassword("FALSE")
            };

            _connectionService.GetUserConnection_UserConnection(connectionId).Returns(userConnection);
            _queryService.ReturnUserFromUsername(userConnection.User.Username).Returns(user2);

            _sut.HandleUpdatePassword(user1, connectionId);

            _queryService.DidNotReceiveWithAnyArgs().UpdatePasswordForUser(default);
        }

        [Test]
        public void CreateRandomUsername_should_return_valid_random_username()
        {
            
            string testUsername = "TEST";
            var newUsername = _sut.CreateRandomUsername(testUsername);
            Assert.That(newUsername, Contains.Substring(testUsername));
            Assert.That(newUsername, Is.Not.EqualTo(testUsername));
        }

        [Test]
        public void IsValidUser_should_return_ispasswordvalid_false_if_no_user_exists()
        {
            var testUser = new User
            {
                Username = "TESTusername"
            };
            _queryService.ReturnUserFromUsername(testUser.Username).ReturnsNull();
            var newUser = _sut.IsValidUser(testUser);
            Assert.That(newUser.IsPasswordValid, Is.False);
        }

        [Test]
        public void IsValidUser_should_return_ispasswordvalid_true_if_user_exists_with_given_password()
        {
            var testUser = new User
            {
                Username = "TESTusername",
                Password = "TESTpassword"
            };
            _queryService.ReturnUserFromUsername(testUser.Username).Returns(new User
            {
                Username = testUser.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(testUser.Password)
            });
            var newUser = _sut.IsValidUser(testUser);
            Assert.That(newUser.IsPasswordValid, Is.True);
        }
        [Test]
        public void IsValidUser_should_return_password_is_null()
        {
            var testUser = new User
            {
                Username = "TESTusername",
                Password = "TESTpassword"
            };
            _queryService.ReturnUserFromUsername(testUser.Username).Returns(new User
            {
                Username = testUser.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(testUser.Password)
            });
            var newUser = _sut.IsValidUser(testUser);
            Assert.That(newUser.Password, Is.Null);
        }
        [Test]
        public void IsValidUser_should_return_id_not_zero_if_ispasswordvalid_is_true()
        {
            var testUser = new User
            {
                Username = "TESTusername",
                Password = "TESTpassword"
            };
            _queryService.ReturnUserFromUsername(testUser.Username).Returns(new User
            {
                Id = 5,
                Username = testUser.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(testUser.Password)
            });
            var newUser = _sut.IsValidUser(testUser);
            Assert.That(newUser.IsPasswordValid, Is.True);
            Assert.That(newUser.Id, Is.EqualTo(5));
        }
        [Test]
        public void IsValidUser_should_return_id_zero_if_ispasswordvalid_is_false()
        {
            var testUser = new User
            {
                Username = "TESTusername",
                Password = "TESTpassword"
            };
            _queryService.ReturnUserFromUsername(testUser.Username).Returns(new User
            {
                Id = 5,
                Username = testUser.Username,
                Password = "FAILPASSWORD"
            });
            var newUser = _sut.IsValidUser(testUser);
            Assert.That(newUser.IsPasswordValid, Is.False);
            Assert.That(newUser.Id, Is.EqualTo(0));
        }
        [Test]
        public void IsValidPassword_should_return_true_if_password_matches()
        {
            var testUser = new User
            {
                Username = "TESTusername",
                Password = "TESTpassword"
            };
            _queryService.ReturnUserFromUsername(testUser.Username).Returns(new User
            {
                Id = 5,
                Username = testUser.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(testUser.Password)
            });
            var newUser = _sut.IsValidUser(testUser);
            Assert.That(newUser.IsPasswordValid, Is.True);
        }
        [Test]
        public void IsValidPassword_should_return_false_if_password_not_matches()
        {
            var testUser = new User
            {
                Username = "TESTusername",
                Password = "TESTpassword"
            };
            _queryService.ReturnUserFromUsername(testUser.Username).Returns(new User
            {
                Id = 5,
                Username = testUser.Username,
                Password = BCrypt.Net.BCrypt.HashPassword("TESTFAILpassword")
            });
            var newUser = _sut.IsValidUser(testUser);
            Assert.That(newUser.IsPasswordValid, Is.False);
        }
        [Test]
        public void IsValidPassword_should_return_false_if_password_not_hash()
        {
            var testUser = new User
            {
                Username = "TESTusername",
                Password = "TESTpassword"
            };
            _queryService.ReturnUserFromUsername(testUser.Username).Returns(new User
            {
                Id = 5,
                Username = testUser.Username,
                Password = "TESTFAILpassword"
            });
            var newUser = _sut.IsValidUser(testUser);
            Assert.That(newUser.IsPasswordValid, Is.False);
        }
    }
}
