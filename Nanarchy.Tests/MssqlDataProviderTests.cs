using System.Configuration;
using Nanarchy.Data.MssqlHierarchyDataProvider;
using Nanarchy.Service;
using NUnit.Framework;

namespace Nanarchy.Tests
{
    [TestFixture]
    public class When_using_MssqlDataProvider
    {
        [Test]
        public void Should_handle_basic_CRUD_for_Hierarchy_record()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["NanarchyDb"].ConnectionString;

            var provider = new MssqlDataProvider(connectionString);

            // create a new hierarchy
            var hierarchy = new Hierarchy
            {
                Name = "Test Hierarchy",
                TableName = "test_hierarchy"
            };

            // add to storage
            var hierarchyId = provider.UpdateHierarchy(hierarchy);

            // retrieve from storage
            var persistedHierarchy = provider.GetHierarchy(hierarchyId);

            Assert.That(persistedHierarchy, Is.Not.Null);
            Assert.That(persistedHierarchy.Id, Is.EqualTo(hierarchyId));
            Assert.That(persistedHierarchy.Name, Is.EqualTo(hierarchy.Name));
            Assert.That(persistedHierarchy.TableName, Is.EqualTo(hierarchy.TableName));

            // update, and save again
            persistedHierarchy.Name = "New Hierarchy";
            var revisedId = provider.UpdateHierarchy(persistedHierarchy);
            Assert.That(revisedId, Is.EqualTo(hierarchyId));

            // retrieve from storage
            var revisedHierarchy = provider.GetHierarchy(hierarchyId);

            Assert.That(revisedHierarchy, Is.Not.Null);
            Assert.That(revisedHierarchy.Id, Is.EqualTo(hierarchyId));
            Assert.That(revisedHierarchy.Name, Is.EqualTo("New Hierarchy"));
            Assert.That(revisedHierarchy.TableName, Is.EqualTo(hierarchy.TableName));

            // delete from storage
            var deleteSuccessful = provider.DeleteHierarchy(hierarchyId);
            Assert.That(deleteSuccessful, Is.True);

            // retrieve from storage
            var deletedHierarchy = provider.GetHierarchy(hierarchyId);
            Assert.That(deletedHierarchy, Is.Null);
        }

        [Test]
        public void Should_handle_basic_CRUD_for_Target_record()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["NanarchyDb"].ConnectionString;

            var provider = new MssqlDataProvider(connectionString);

            // create a new hierarchy
            var target = new Target
            {
                Name = "Test Target",
                TableName = "test_target"
            };

            // add to storage
            var targetId = provider.UpdateTarget(target);

            // retrieve from storage
            var persistedTarget = provider.GetTarget(targetId);

            Assert.That(persistedTarget, Is.Not.Null);
            Assert.That(persistedTarget.Id, Is.EqualTo(targetId));
            Assert.That(persistedTarget.Name, Is.EqualTo(target.Name));
            Assert.That(persistedTarget.TableName, Is.EqualTo(target.TableName));

            // update, and save again
            persistedTarget.Name = "New Target";
            var revisedId = provider.UpdateTarget(persistedTarget);
            Assert.That(revisedId, Is.EqualTo(targetId));

            // retrieve from storage
            var revisedTarget = provider.GetTarget(targetId);

            Assert.That(revisedTarget, Is.Not.Null);
            Assert.That(revisedTarget.Id, Is.EqualTo(targetId));
            Assert.That(revisedTarget.Name, Is.EqualTo("New Target"));
            Assert.That(revisedTarget.TableName, Is.EqualTo(target.TableName));

            // delete from storage
            var deleteSuccessful = provider.DeleteTarget(targetId);
            Assert.That(deleteSuccessful, Is.True);

            // retrieve from storage
            var deletedTarget = provider.GetTarget(targetId);
            Assert.That(deletedTarget, Is.Null);
        }
    }
}