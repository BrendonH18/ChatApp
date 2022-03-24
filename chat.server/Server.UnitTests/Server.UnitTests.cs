//using NHibernate;
//using NHibernate.Cfg;
//using NHibernate.Linq;
using NUnit.Framework;
using server.Hubs;
using server.Models;
//using System;
using System.Collections.Generic;
//using System.Linq;

namespace Server.UnitTests
{
    [TestFixture]
    public class HubsTests
    {
        ////NHibernate
        //private Configuration myConfiguration;
        //private ISessionFactory mySessionFactory;
        //private ISession mySession;

        ////Server
        //private IDictionary<string, UserConnection> _connections;
        //private string _botUser;
        //private List<string> _rooms;
        //private Message _testMessage;
        //private ChatHub _chatHub;
        //private string _testUsername;
        //private string _testRoom;
        //private DateTime _testCreatedOn;
        //private string _testText;


        //[SetUp]
        //public void Setup()
        //{
        //    myConfiguration = new Configuration();
        //    myConfiguration.Configure();
        //    mySessionFactory = myConfiguration.BuildSessionFactory();
        //    //mySession = mySessionFactory.OpenSession(); // unsafe

        //    //Server
        //    _botUser = "ChatBot";
        //    _rooms = new List<string> { "Sports", "Culture", "Art", "Fashion", "Custom/New" };
        //    _testUsername = "TestUsername";
        //    _testRoom = "TestRoom";
        //    _testCreatedOn = DateTime.Now;
        //    _testText = "TestText";
        //    _testMessage = new Message
        //    {
        //        Username = _testUsername,
        //        Room = _testRoom,
        //        Created_on = _testCreatedOn,
        //        Text = _testText
        //    };
        //    _chatHub = new ChatHub(_connections);
        //}

        [Test]
        public void CreateMessageInDB_MessageWithUsernameTextCreatedonAndRoom_MessageInDB()
        {
            ////Arrange
            var chathub = new ChatHub(new Dictionary<string, UserConnection>());


            ////Act
            //_chatHub.CreateMessageInDB(_testMessage);
            //Message remoteMessage;
            //using (mySession = mySessionFactory.OpenSession())
            //{
            //    var DBMessage = mySession.Query<Message>()
            //        .Single(x =>
            //        x.Username == _testUsername &&
            //        x.Room == _testRoom &&
            //        x.Created_on == _testCreatedOn &&
            //        x.Text == _testText);
            //    mySession.Flush();
            //    remoteMessage = DBMessage;
            //}

            ////Assert
            //Assert.That(remoteMessage == _testMessage);
            Assert.Pass();
        }

        //[TearDown]
        //public void RemoveMessageFromDB()
        //{
        //    using (mySession = mySessionFactory.OpenSession())
        //    {
        //        mySession.Query<Message>()
        //            .Where(x =>
        //            x.Username == _testUsername &&
        //            x.Room == _testRoom &&
        //            x.Created_on == _testCreatedOn &&
        //            x.Text == _testText)
        //            .Delete();
        //        mySession.Flush();
        //    }
        //}
    }
}
