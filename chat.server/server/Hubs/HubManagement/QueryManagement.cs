using Microsoft.AspNetCore.SignalR;
using NHibernate;
using NHibernate.Linq;
using server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace server.Hubs.HubSupport
{
    public class QueryManagement : Hub
    {
        private readonly ISessionFactory myFactory;
        public QueryManagement(ISessionFactory factory)
        {
            myFactory = factory;
        }

        public User RetrieveCredential(string username)
        {
            using (var session = myFactory.OpenSession())
            {
                var loCredential = session.Query<User>()
                    .SingleOrDefault(x => x.Username == username);
                session.Flush();
                return loCredential;
            }
        }
        public User CreateUserInDB(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            using (var session = myFactory.OpenSession())
            {
                session.Save(user);
                session.Flush();
            }
            return user;
        }

        public List<Channel> QueryDBforChannels()
        {
            List<Channel> channels;
            using (var session = myFactory.OpenSession())
            {
                channels = session.Query<Channel>()
                        .ToList();
            }
            return channels;
        }

        public List<Message> RetrieveMessagesFromDB(Channel channel)
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

        public void CreateMessageInDB(Message message)
        {
            using (var session = myFactory.OpenSession())
            {
                session.Save(message);
                session.Flush(); // New
            }
        }

        public bool UpdatePasswordInDB(string newPassword, UserConnection userConnection)
        {
            try
            {
                if (userConnection == null) return false;

                newPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                using (var session = myFactory.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.Query<User>()
                            .Where(x => x.Username == userConnection.User.Username)
                            .Update(x => new User { Username = userConnection.User.Username, Password = newPassword });
                        transaction.Commit();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        
    }
}
