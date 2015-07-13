using System.Configuration;
using System.Linq;
using Nanarchy.Core;
using Nanarchy.Data.MssqlHierarchyDataProvider;
using Nanarchy.Tests.TestData;
using NUnit.Framework;

namespace Nanarchy.Tests
{
    [TestFixture]
    public class When_using_MssqlHierarchyDataProvider
    {
        [Test]
        public void Should_properly_create_and_handle_simple_hierarchy_with_basic_operations()
        {
            // build a sample hierarchy
            var hierarchyEntry = new HierarchyEntry
            {
                Name = "Test Hierarchy",
                SchemaName = "dbo",
                TableName = "test"
            };
 
            // initialize the provider
            var connectionString = ConfigurationManager.ConnectionStrings["NanarchyDb"].ConnectionString;
            var dataProvider = new MssqlDataProvider(connectionString);
            var provider = new MssqlHierarchyDataProvider(dataProvider);

            // initialize the hierarchy
            var originalRootNode = new HierarchyNode
            {
                LeftId = 1,
                RightId = 2
            };
            var rootId = provider.Add(hierarchyEntry, originalRootNode);

            // test get root node
            var persistedRootNode = provider.GetRootNode(hierarchyEntry);

            Assert.That(persistedRootNode.Id, Is.EqualTo(rootId));
            Assert.That(persistedRootNode.LeftId, Is.EqualTo(1));
            Assert.That(persistedRootNode.RightId, Is.EqualTo(2));

            // then insert first primary child
            var firstChild = new HierarchyNode();
            firstChild.LeftId = persistedRootNode.RightId;
            provider.PrepareForInsertNode(hierarchyEntry, persistedRootNode);
            firstChild.RightId = firstChild.LeftId + 1;
            provider.Add(hierarchyEntry, firstChild);

            // verify new hierarchy
            persistedRootNode = provider.GetRootNode(hierarchyEntry);
            var firstChildren = provider.GetChildren(hierarchyEntry, persistedRootNode);

            Assert.That(firstChildren.Count, Is.EqualTo(1));
            Assert.That(firstChildren[0].Id, Is.Not.EqualTo(0));
            Assert.That(firstChildren[0].LeftId, Is.EqualTo(2));
            Assert.That(firstChildren[0].RightId, Is.EqualTo(3));
            Assert.That(persistedRootNode.LeftId, Is.EqualTo(1));
            Assert.That(persistedRootNode.RightId, Is.EqualTo(4));

            // insert second primary child
            var secondChild = new HierarchyNode();
            secondChild.LeftId = persistedRootNode.RightId;
            provider.PrepareForInsertNode(hierarchyEntry, persistedRootNode);
            secondChild.RightId = secondChild.LeftId + 1;
            provider.Add(hierarchyEntry, secondChild);

            // verify new hierarchy
            persistedRootNode = provider.GetRootNode(hierarchyEntry);
            var secondChildren = provider.GetChildren(hierarchyEntry, persistedRootNode);

            Assert.That(secondChildren.Count, Is.EqualTo(2));
            Assert.That(secondChildren[1].Id, Is.Not.EqualTo(0));
            Assert.That(secondChildren[1].LeftId, Is.EqualTo(4));
            Assert.That(secondChildren[1].RightId, Is.EqualTo(5));
            Assert.That(secondChildren[0].LeftId, Is.EqualTo(2));
            Assert.That(secondChildren[0].RightId, Is.EqualTo(3));
            Assert.That(persistedRootNode.LeftId, Is.EqualTo(1));
            Assert.That(persistedRootNode.RightId, Is.EqualTo(6));

            // clean up
            dataProvider.DropTable(hierarchyEntry.SchemaName, hierarchyEntry.TableName);
        }

        [Test]
        public void Should_handle_GetDescendants_operation_over_larger_hierarchy()
        {
            // build a sample hierarchy
            var hierarchyEntry = new HierarchyEntry
            {
                Name = "Test Hierarchy",
                SchemaName = "dbo",
                TableName = "test"
            };

            // initialize the provider
            var connectionString = ConfigurationManager.ConnectionStrings["NanarchyDb"].ConnectionString;
            var dataProvider = new MssqlDataProvider(connectionString);
            var provider = new MssqlHierarchyDataProvider(dataProvider);

            // generate the complex hierarchy
            TestHierarchy.BuildTestHierarchy(hierarchyEntry, provider);

            var rootNode = provider.GetRootNode(hierarchyEntry);
            var descendants = provider.GetDescendants(hierarchyEntry, rootNode, true, true);

            Assert.That(descendants.Count, Is.EqualTo(7));
            Assert.That(descendants[0].LeftId, Is.EqualTo(1));
            Assert.That(descendants[0].RightId, Is.EqualTo(14));
            Assert.That(descendants[1].LeftId, Is.EqualTo(2));
            Assert.That(descendants[1].RightId, Is.EqualTo(7));
            Assert.That(descendants[2].LeftId, Is.EqualTo(3));
            Assert.That(descendants[2].RightId, Is.EqualTo(4));
            Assert.That(descendants[3].LeftId, Is.EqualTo(5));
            Assert.That(descendants[3].RightId, Is.EqualTo(6));
            Assert.That(descendants[4].LeftId, Is.EqualTo(8));
            Assert.That(descendants[4].RightId, Is.EqualTo(13));
            Assert.That(descendants[5].LeftId, Is.EqualTo(9));
            Assert.That(descendants[5].RightId, Is.EqualTo(10)); 
            Assert.That(descendants[6].LeftId, Is.EqualTo(11));
            Assert.That(descendants[6].RightId, Is.EqualTo(12));

            Assert.That(rootNode.LeftId, Is.EqualTo(1));
            Assert.That(rootNode.RightId, Is.EqualTo(14));

            // clean up
            dataProvider.DropTable(hierarchyEntry.SchemaName, hierarchyEntry.TableName);
        }

        [Test]
        public void Should_handle_GetAncestors_operation_over_larger_hierarchy()
        {
            // build a sample hierarchy
            var hierarchyEntry = new HierarchyEntry
            {
                Name = "Test Hierarchy",
                SchemaName = "dbo",
                TableName = "test"
            };

            // initialize the provider
            var connectionString = ConfigurationManager.ConnectionStrings["NanarchyDb"].ConnectionString;
            var dataProvider = new MssqlDataProvider(connectionString);
            var provider = new MssqlHierarchyDataProvider(dataProvider);

            // generate the complex hierarchy
            TestHierarchy.BuildTestHierarchy(hierarchyEntry, provider);

            var rootNode = provider.GetRootNode(hierarchyEntry);
            var primaryChildren = provider.GetChildren(hierarchyEntry, rootNode);
            var primaryChild1 = primaryChildren.First();
            var secondaryChildren = provider.GetChildren(hierarchyEntry, primaryChild1);
            var leafChild = secondaryChildren.First();

            var ancestors = provider.GetAncestors(hierarchyEntry, leafChild, true, true);

            Assert.That(ancestors.Count, Is.EqualTo(3));
            Assert.That(ancestors[0].LeftId, Is.EqualTo(1));
            Assert.That(ancestors[0].RightId, Is.EqualTo(14));
            Assert.That(ancestors[1].LeftId, Is.EqualTo(2));
            Assert.That(ancestors[1].RightId, Is.EqualTo(7));
            Assert.That(ancestors[2].LeftId, Is.EqualTo(leafChild.LeftId));
            Assert.That(ancestors[2].RightId, Is.EqualTo(leafChild.RightId));

            // clean up
            dataProvider.DropTable(hierarchyEntry.SchemaName, hierarchyEntry.TableName);
        }

        [Test]
        public void Should_handle_GetParent_operation_over_larger_hierarchy()
        {
            // build a sample hierarchy
            var hierarchyEntry = new HierarchyEntry
            {
                Name = "Test Hierarchy",
                SchemaName = "dbo",
                TableName = "test"
            };

            // initialize the provider
            var connectionString = ConfigurationManager.ConnectionStrings["NanarchyDb"].ConnectionString;
            var dataProvider = new MssqlDataProvider(connectionString);
            var provider = new MssqlHierarchyDataProvider(dataProvider);

            // generate the complex hierarchy
            TestHierarchy.BuildTestHierarchy(hierarchyEntry, provider);

            var rootNode = provider.GetRootNode(hierarchyEntry);
            var primaryChildren = provider.GetChildren(hierarchyEntry, rootNode);
            var primaryChild1 = primaryChildren.First();
            var secondaryChildren = provider.GetChildren(hierarchyEntry, primaryChild1);
            var leafChild = secondaryChildren.First();

            var parent = provider.GetParent(hierarchyEntry, leafChild);

            Assert.That(parent.LeftId, Is.LessThan(leafChild.LeftId));
            Assert.That(parent.RightId, Is.GreaterThan(leafChild.RightId));

            // clean up
            dataProvider.DropTable(hierarchyEntry.SchemaName, hierarchyEntry.TableName);
        }
    }
}