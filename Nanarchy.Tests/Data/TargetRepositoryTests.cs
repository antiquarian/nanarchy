using Nanarchy.Data.Mssql;
using Nanarchy.Tests.TestData;
using NUnit.Framework;

namespace Nanarchy.Tests.Data
{
    [TestFixture]
    public class When_using_TargetRepository
    {
        [Test]
        public void Should_handle_basic_CRUD_operations()
        {
            // arrange
            var target = new TestTarget
            {
                Id = 0,
                Data = new TestTargetData
                {
                    TestString = "Test Target Data"
                }
            };
            var dataProvider = new MssqlDataProvider();
            var repository = new TargetRepository(dataProvider);

            // act
            var result = repository.Update(target);

            // assert
            Assert.That(result, Is.GreaterThan(0));

            // now Get
            var persistedTarget = repository.Get<TestTarget>(target.TableName, result);
            Assert.That(persistedTarget.Id, Is.GreaterThan(0));
            Assert.That(persistedTarget.Data, Is.Not.Null);

            var targetData = (TestTargetData) persistedTarget.Data;
            Assert.That(targetData.TestString, Is.EqualTo("Test Target Data"));

            // remove the record
            repository.Delete(persistedTarget);

        }
    }
}