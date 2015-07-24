using System;
using System.Collections.Generic;
using System.Data;
using Moq;
using Nanarchy.Core;
using Nanarchy.Core.Interfaces;
using Nanarchy.Data.Mssql;
using NUnit.Framework;

namespace Nanarchy.Tests.Data
{
    [TestFixture]
    public class _When_using_TargetEntryRepository 
    {
        [Test]
        public void Should_call_Update_on_DataProvider()
        {
            // arrange
            var targetEntry = new TargetEntry
            {
                Name = "NewTarget",
                TableName = "new_Target"
            };
            var mockDataProvider = new Mock<IDataProvider>();

            mockDataProvider
                .Setup(p => p.Update(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<KeyValuePair<string, object>>>()))
                .Returns(34);
            var repository = new TargetEntryRepository(mockDataProvider.Object);

            // act
            var result = repository.Update(targetEntry);

            // assert
            Assert.That(result, Is.EqualTo(34));
            mockDataProvider.Verify(p => p.Update(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<KeyValuePair<string, object>>>()));
        }

        [Test]
        public void Should_call_Get_on_DataProvider()
        {
            // arrange
            var targetEntry = new TargetEntry
            {
                Id = 34,
                Name = "NewTarget",
                TableName = "new_Target"
            };
            var mockDataProvider = new Mock<IDataProvider>();

            mockDataProvider
                .Setup(p => p.Get(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Func<IDataRecord, TargetEntry>>()))
                .Returns(targetEntry);
            var repository = new TargetEntryRepository(mockDataProvider.Object);

            // act
            var result = repository.Get(34);

            // assert
            Assert.That(result.Id, Is.EqualTo(34));
            mockDataProvider.Verify(p => p.Get(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Func<IDataRecord, TargetEntry>>()));
        }

        [Test]
        public void Should_call_Delete_on_DataProvider()
        {
            // arrange
            var mockDataProvider = new Mock<IDataProvider>();

            mockDataProvider
                .Setup(p => p.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);
            var repository = new TargetEntryRepository(mockDataProvider.Object);

            // act
            var result = repository.Delete(34);

            // assert
            Assert.That(result, Is.True);
            mockDataProvider.Verify(p => p.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
        }

        [Test]
        public void Should_call_ExecuteSql_on_DataProvider()
        {
            // arrange
            var mockDataProvider = new Mock<IDataProvider>();
            mockDataProvider.Setup(p => p.ExecuteSql(It.IsAny<string>()));
            var repository = new TargetEntryRepository(mockDataProvider.Object);

            // act
            repository.Initialize();

            // assert
            mockDataProvider.Verify(p => p.ExecuteSql(It.IsAny<string>()));
        }
    }
}