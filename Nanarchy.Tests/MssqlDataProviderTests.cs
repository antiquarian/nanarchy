using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Nanarchy.Core;
using Nanarchy.Data.MssqlHierarchyDataProvider;
using NUnit.Framework;

namespace Nanarchy.Tests
{
    [TestFixture]
    public class When_using_MssqlDataProvider
    {

        //[Test]
        //public void Should_create_tables_when_they_do_not_exist()
        //{
        //    var connectionString = ConfigurationManager.ConnectionStrings["NanarchyDb"].ConnectionString;
        //    var provider = new MssqlDataProvider(connectionString);
        //    var hierarchy = new HierarchyEntry
        //    {
        //        Name = "Test Hierarchy",
        //        TableName = "test_hierarchy"
        //    };
        //    var targets = new List<TargetEntry>
        //    {
        //        new TargetEntry
        //        {
        //            Name = "Target One",
        //            TableName = "target_one"
        //        },
        //        new TargetEntry
        //        {
        //            Name = "Target Two",
        //            TableName = "target_two"
        //        }
        //    };

        //    // act
        //    provider.InitializeDatabase(hierarchy, targets);

        //    // Assert
        //    Assert.That(provider.TableExists("dbo", "Hierarchy"));
        //    Assert.That(provider.TableExists("dbo", "Target"));
        //    Assert.That(provider.TableExists("dbo", "target_one"));
        //    Assert.That(provider.TableExists("dbo", "target_two"));

        //    provider.DropTable("dbo", "Hierarchy");
        //    provider.DropTable("dbo", "Target");
        //    provider.DropTable("dbo", "target_one");
        //    provider.DropTable("dbo", "target_two");

        //}
        [Test]
        public void Should_handle_basic_CRUD_for_any_table()
        {
            const string schemaName = "dbo";
            const string tableName = "TestTable";
            var connectionString = ConfigurationManager.ConnectionStrings["NanarchyDb"].ConnectionString;

            var provider = new MssqlDataProvider(connectionString);

            // check for the test table
            if (provider.TableExists(schemaName, tableName))
            {
                provider.DropTable(schemaName, tableName);
            }

            // create table
            var createSql = string.Format(@"CREATE TABLE [{0}].[{1}](
	                [id] [int] IDENTITY(1,1) NOT NULL,
	                [name] [nvarchar](50) NOT NULL
                    CONSTRAINT [PK_{1}] PRIMARY KEY CLUSTERED 
                        ([id] ASC)
                    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]", schemaName, tableName);
            provider.ExecuteSql(createSql);

            // add to storage
            var insertSql = string.Format("INSERT INTO [{0}].[{1}] (name) OUTPUT inserted.id VALUES (@Name)", schemaName, tableName);
            var hierarchyId = provider.Update(insertSql, 0, new List<KeyValuePair<string, object>>{ new KeyValuePair<string, object>("@Name", "Test Name")});

            // retrieve from storage
            var getSql = string.Format("SELECT id, name FROM [{0}].[{1}] WHERE id = @Id", schemaName, tableName);
            var persistedRecord = provider.Get(getSql, hierarchyId, PopulateMethod);

            Assert.That(persistedRecord, Is.Not.Null);
            Assert.That(persistedRecord.Id, Is.EqualTo(hierarchyId));
            Assert.That(persistedRecord.Name, Is.EqualTo("Test Name"));

            // update, and save again
            persistedRecord.Name = "New Name";
            var updateSql = string.Format("UPDATE [{0}].[{1}] SET name=@Name WHERE id=@Id", schemaName, tableName);
            var revisedId = provider.Update(updateSql, hierarchyId, new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>("@Name", "New Name") });
            Assert.That(revisedId, Is.EqualTo(hierarchyId));

            // retrieve from storage
            var revisedRecord = provider.Get(getSql, hierarchyId, PopulateMethod);

            Assert.That(revisedRecord, Is.Not.Null);
            Assert.That(revisedRecord.Id, Is.EqualTo(hierarchyId));
            Assert.That(revisedRecord.Name, Is.EqualTo("New Name"));

            // delete from storage
            var deleteSuccessful = provider.Delete(schemaName, tableName, hierarchyId);
            Assert.That(deleteSuccessful, Is.True);

            // retrieve from storage
            var deletedRecord = provider.Get(getSql, hierarchyId, PopulateMethod);
            Assert.That(deletedRecord, Is.Null);

            // drop the table
            provider.DropTable(schemaName, tableName);

            Assert.That(provider.TableExists(schemaName, tableName), Is.False);
        }

        private class TestObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private TestObject PopulateMethod(IDataRecord reader)
        {
            var target = new TestObject
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
            };

            return target;
        }

        //[Test]
        //public void Should_handle_basic_CRUD_for_Target_record()
        //{
        //    var connectionString = ConfigurationManager.ConnectionStrings["NanarchyDb"].ConnectionString;

        //    var provider = new MssqlDataProvider(connectionString);

        //    // create a new hierarchy
        //    var target = new TargetEntry
        //    {
        //        Name = "Test Target",
        //        TableName = "test_target"
        //    };

        //    // add to storage
        //    var targetId = provider.UpdateTargetEntry(target);

        //    // retrieve from storage
        //    var persistedTarget = provider.GetTargetEntry(targetId);

        //    Assert.That(persistedTarget, Is.Not.Null);
        //    Assert.That(persistedTarget.Id, Is.EqualTo(targetId));
        //    Assert.That(persistedTarget.Name, Is.EqualTo(target.Name));
        //    Assert.That(persistedTarget.TableName, Is.EqualTo(target.TableName));

        //    // update, and save again
        //    persistedTarget.Name = "New Target";
        //    var revisedId = provider.UpdateTargetEntry(persistedTarget);
        //    Assert.That(revisedId, Is.EqualTo(targetId));

        //    // retrieve from storage
        //    var revisedTarget = provider.GetTargetEntry(targetId);

        //    Assert.That(revisedTarget, Is.Not.Null);
        //    Assert.That(revisedTarget.Id, Is.EqualTo(targetId));
        //    Assert.That(revisedTarget.Name, Is.EqualTo("New Target"));
        //    Assert.That(revisedTarget.TableName, Is.EqualTo(target.TableName));

        //    // delete from storage
        //    var deleteSuccessful = provider.DeleteTargetEntry(targetId);
        //    Assert.That(deleteSuccessful, Is.True);

        //    // retrieve from storage
        //    var deletedTarget = provider.GetTargetEntry(targetId);
        //    Assert.That(deletedTarget, Is.Null);
        //}
    }
}