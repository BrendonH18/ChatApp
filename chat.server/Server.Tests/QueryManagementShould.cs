using NSubstitute;
using NUnit.Framework;
using server.Hubs.HubManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Tests
{
    [TestFixture]
    internal class QueryManagementShould
    {
        private IQueryManagement _sut;

        [SetUp]
        public void Setup()
        {
            _sut = Substitute.For<IQueryManagement>();
        }

        [Test]
        public void NotSureIfThisIsNecessary()
        {
            Assert.Fail();
        }
    }
}
