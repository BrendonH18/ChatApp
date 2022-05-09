using NHibernate;
using server.Models;
using System.Linq;

namespace server.Hubs.HubSupport
{
    public class DatabaseQueries
    {
        private readonly ISessionFactory myFactory;
        public DatabaseQueries(ISessionFactory factory)
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
    }
}
