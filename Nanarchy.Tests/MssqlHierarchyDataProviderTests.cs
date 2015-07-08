using System.Configuration;
using Nanarchy.Data.MssqlHierarchyDataProvider;
using NUnit.Framework;

namespace Nanarchy.Tests
{
    [TestFixture]
    public class When_using_MssqlHierarchyDataProvider
    {
        [Test]
        public void Should_properly_initialize_sqlConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["NanarchyDb"].ConnectionString;

            var provider = new MssqlHierarchyDataProvider(connectionString);

            Assert.That(provider, Is.Not.Null);
        }
    }
}