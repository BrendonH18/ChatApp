using NSubstitute;
using NSubstitute.ReturnsExtensions;
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
    internal class LoginManagementShould
    {
        private LoginManagement _sut;
        private IQueryManagement _queryManagement;

        [SetUp]
        public void Setup()
        {
            _queryManagement = Substitute.For<IQueryManagement>();
            _sut = new LoginManagement(_queryManagement);
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
            _queryManagement.ReturnUserFromUsername(testUser.Username).ReturnsNull();
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
            _queryManagement.ReturnUserFromUsername(testUser.Username).Returns(new User
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
            _queryManagement.ReturnUserFromUsername(testUser.Username).Returns(new User
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
            _queryManagement.ReturnUserFromUsername(testUser.Username).Returns(new User
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
            _queryManagement.ReturnUserFromUsername(testUser.Username).Returns(new User
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
            _queryManagement.ReturnUserFromUsername(testUser.Username).Returns(new User
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
            _queryManagement.ReturnUserFromUsername(testUser.Username).Returns(new User
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
            _queryManagement.ReturnUserFromUsername(testUser.Username).Returns(new User
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
