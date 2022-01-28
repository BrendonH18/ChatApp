using Microsoft.AspNetCore.SignalR;
using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Models;

namespace server.Hubs
{
    public class UserValidation
    {
        //NHibernate
        private Configuration myConfiguration;
        private ISessionFactory mySessionFactory;
        private ISession mySession;

        public UserValidation()
        {
            //NHibernate
            myConfiguration = new Configuration();
            myConfiguration.Configure();
            mySessionFactory = myConfiguration.BuildSessionFactory();
            //mySession = mySessionFactory.OpenSession(); // unsafe
        }

        //public void ValidateCredentials(Credential credential)
        //{
        //    bool exists = false;

        //    using (mySession = mySessionFactory.OpenSession())
        //    {
        //        try
        //        {
        //            exists = mySession.Query<Credential>()
        //                .Any(x => x.Username == credential.Username && x.Password == credential.Password);
                        
        //        }
        //        catch (Exception e)
        //        {
        //            var Error = e;
        //        }
        //    }

        //    Clients.User(Context.UserIdentifier).SendAsync("IsValid", exists);
        //}
    }
}
