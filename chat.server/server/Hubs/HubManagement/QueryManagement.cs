using Microsoft.AspNetCore.SignalR;
using NHibernate;
using NHibernate.Linq;
using server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace server.Hubs.HubManagement
{
    public interface IQueryManagement
    {
        public User ReturnUserFromUsername(string username);
        public bool DoesUserExist(User user);
        public User CreateNewUser(User user);
        public List<Channel> ReturnAvailableChannels_List();
        public List<Message> ReturnMessagesByChannel_List(Channel channel);
        public void InsertMessage(Message message);
        public User UpdatePasswordForUser(User user);

    }
    
    public class QueryManagement : IQueryManagement
    {
        private readonly ISessionFactory myFactory;
        public QueryManagement(ISessionFactory factory)
        {
            myFactory = factory;
        }

        public User ReturnUserFromUsername(string username)
        {
            using (var session = myFactory.OpenSession())
            {
                var loCredential = session.Query<User>()
                    .SingleOrDefault(x => x.Username == username);
                session.Flush();
                return loCredential;
            }
        }

        public bool DoesUserExist(User user)
        {
            using (var session = myFactory.OpenSession())
            {
                var loCredential = session.Query<User>()
                    .Where(x => x.Username == user.Username)
                    .ToList();
                if (loCredential.Count == 0) return false;
                return true;
            }
        }

        public User CreateNewUser(User user)
        {
            if(DoesUserExist(user) == true) return user;
            string hashPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            using (var session = myFactory.OpenSession())
            {
                session.Save(new User { Password = hashPassword, Username = user.Username});
                session.Flush();
            }
            return user;
        }

        public List<Channel> ReturnAvailableChannels_List()
        {
            List<Channel> channels;
            using (var session = myFactory.OpenSession())
            {
                channels = session.Query<Channel>()
                        .ToList();
            }
            return channels;
        }

        public List<Message> ReturnMessagesByChannel_List(Channel channel)
        {
            List<Message> roomMessages;
            using (var session = myFactory.OpenSession())
            {
                roomMessages = session.Query<Message>()
                    .Where(m => m.Channel.Name == channel.Name)
                    .ToList();
            }
            return roomMessages;
        }

        public void InsertMessage(Message message)
        {
            using (var session = myFactory.OpenSession())
            {
                session.Save(message);
                session.Flush(); // New
            }
        }

        public User UpdatePasswordForUser(User user)
        {
            if (user == null) return null;
            if (!user.IsPasswordValid) return user;
            string newPassword = BCrypt.Net.BCrypt.HashPassword(user.NewPassword);
            user.NewPassword = null;
            using (var session = myFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Query<User>()
                        .Where(x => x.Username == user.Username)
                        .Update(x => new User { Username = user.Username, Password = newPassword });
                    transaction.Commit();
                }
            }
            return user;
        }

        
    }
}
