using System.Configuration;
using System.Data.SqlClient;
using Dell.Hierarchy.Data.MssqlHierarchyDataProvider;
using NUnit.Framework;

namespace Dell.Hierarchy.Tests
{
    [TestFixture]
    public class When_using_MssqlHierarchyDataProvider
    {
        [Test]
        public void Should_properly_initialize_sqlConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["HierarchyDb"].ConnectionString;

            var provider = new MssqlHierarchyDataProvider(connectionString);

            Assert.That(provider, Is.Not.Null);
        }
    }
}