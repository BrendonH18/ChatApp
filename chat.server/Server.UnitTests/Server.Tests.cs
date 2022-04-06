using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;
using server.Hubs;
using server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Tests
{
    [TestFixture]
    public class HubsTests
    {
        // QUESTION: How do I handle a test when my database creates an ID, not my server?
        // Should I set up the database configuration once per test session or for each test?
        ISessionFactory mySessionFactory;
        ISession mySession;
        ChatHub chatHub;

        [OneTimeSetUp]
        public void Init()
        {
            Configuration myConfiguration = new Configuration();
            myConfiguration.Configure();
            mySessionFactory = myConfiguration.BuildSessionFactory();

            chatHub = new ChatHub(new Dictionary<string, UserConnection>());
        }


        [Test]
        public void CreateMessageInDB_MessageShould_BeInDB()
        {
            ////Arrange
            var testMessage = new Message
            {
                Username = "Test_Username",
                Text = "Test_Text",
                Created_on = DateTime.UtcNow,
                Room = "Test_Room"
            };
            List<Message> remoteMessages;

            //Act
            chatHub.CreateMessageInDB(testMessage);

            using (mySession = mySessionFactory.OpenSession())
            {
                remoteMessages = mySession.Query<Message>()
                    .Where(x =>
                        x.Username == testMessage.Username &&
                        x.Text == testMessage.Text &&
                        x.Room == testMessage.Room)
                    .ToList();
                mySession.Flush();

                mySession.Query<Message>()
                    .Where(x =>
                        x.Username == testMessage.Username &&
                        x.Text == testMessage.Text &&
                        x.Room == testMessage.Room)
                    .Delete();
                mySession.Flush();
            }

            //Assert
            Assert.That(remoteMessages, Has.Exactly(1).Items);
            Assert.That(testMessage.Created_on, Is.EqualTo(remoteMessages[0].Created_on).Within(5).Seconds);
            Assert.That(remoteMessages[0].Id, Is.TypeOf<int>());
            Assert.That(testMessage.Room, Is.EqualTo(remoteMessages[0].Room));
            Assert.That(testMessage.Username, Is.EqualTo(remoteMessages[0].Username));
            Assert.That(testMessage.Text, Is.EqualTo(remoteMessages[0].Text));
        }

        [Test]
        public void CreateCredentialInDB_CredentialShould_BeInDB()
        {
            ////Arrange
            var testCredential = new Credential { Username = "Test_Username", Password = "Test_Password" };
            List<Credential> remoteCredentials;

            //Act
            chatHub.CreateCredentialInDB(testCredential);

            using (mySession = mySessionFactory.OpenSession())
            {
                remoteCredentials = mySession.Query<Credential>()
                    .Where(x =>
                        x.Username == "Test_Username")
                    .ToList();
                mySession.Flush();

                mySession.Query<Credential>()
                    .Where(x =>
                        x.Username == "Test_Username")
                    .Delete();
                mySession.Flush();
            }

            var IsSamePassword = BCrypt.Net.BCrypt.Verify("Test_Password", remoteCredentials[0].Password);

            //Assert
            Assert.That(remoteCredentials, Has.Exactly(1).Items);
            Assert.That(testCredential.Username, Is.EqualTo(remoteCredentials[0].Username));
            Assert.That(IsSamePassword, Is.True);
        }

        [Test]
        public void UpdatePassword_ShouldReturnTrue()
        {
            ////Arrange
            string oldPassword = "Test1";
            Credential oldCredential = new Credential { Username = "Test_Username", Password = BCrypt.Net.BCrypt.HashPassword(oldPassword) };
            Credential newCredential = new Credential { Username = oldCredential.Username, Password = "Test2" };
            List<Credential> oldCredentials;
            List<Credential> newCredentials;

            //Act
            using (mySession = mySessionFactory.OpenSession())
            {
                mySession.Save(oldCredential);
                
                oldCredentials = mySession.Query<Credential>()
                    .Where(x =>
                        x.Username == oldCredential.Username)
                    .ToList();
                mySession.Flush();
            }

            chatHub.UpdatePasswordInDB(newCredential);

            using (mySession = mySessionFactory.OpenSession())
            {
                newCredentials = mySession.Query<Credential>()
                    .Where(x =>
                        x.Username == newCredential.Username)
                    .ToList();
                mySession.Flush();

                mySession.Query<Credential>()
                    .Where(x =>
                        x.Username == newCredential.Username)
                    .Delete();
                mySession.Flush();
            }

            var IsOldCredentialPasswordValid = BCrypt.Net.BCrypt.Verify(oldPassword, oldCredentials[0].Password);
            var IsNewCredentialPasswordValid = BCrypt.Net.BCrypt.Verify(newCredential.Password, newCredentials[0].Password);

            //Assert
            Assert.That(oldCredentials, Has.Exactly(1).Items);
            Assert.That(newCredentials, Has.Exactly(1).Items);
            Assert.That(IsOldCredentialPasswordValid, Is.True);
            Assert.That(IsNewCredentialPasswordValid, Is.True);
            Assert.That(oldCredentials[0].Username, Is.EqualTo(newCredentials[0].Username));
        }
    }
}
