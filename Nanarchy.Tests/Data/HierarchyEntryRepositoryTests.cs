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
    public class _When_using_HierarchyEntryRepository // : UnitTestsFor<HierarchyEntryRepository>
    {
        [Test]
        public void Should_call_Update_on_DataProvider()
        {
            // arrange
            var hierarchyEntry = new HierarchyEntry
            {
                Name = "NewHierarchy",
                TableName = "new_hierarchy"
            };
            var mockDataProvider = new Mock<IDataProvider>();

            mockDataProvider
                .Setup(p => p.Update(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<KeyValuePair<string, object>>>()))
                .Returns(34);
            var repository = new HierarchyEntryRepository(mockDataProvider.Object);

            // act
            var result = repository.Update(hierarchyEntry);

            // assert
            Assert.That(result, Is.EqualTo(34));
        }

        [Test]
        public void Should_call_Get_on_DataProvider()
        {
            // arrange
            var hierarchyEntry = new HierarchyEntry
            {
                Id = 34,
                Name = "NewHierarchy",
                TableName = "new_hierarchy"
            };
            var mockDataProvider = new Mock<IDataProvider>();

            mockDataProvider
                .Setup(p => p.Get(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Func<IDataRecord, HierarchyEntry>>()))
                .Returns(hierarchyEntry);
            var repository = new HierarchyEntryRepository(mockDataProvider.Object);

            // act
            var result = repository.Get(34);

            // assert
            Assert.That(result.Id, Is.EqualTo(34));
        }
        
        [Test]
        public void Should_call_Delete_on_DataProvider()
        {
            // arrange
            var mockDataProvider = new Mock<IDataProvider>();

            mockDataProvider
                .Setup(p => p.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(true);
            var repository = new HierarchyEntryRepository(mockDataProvider.Object);

            // act
            var result = repository.Delete(34);

            // assert
            Assert.That(result, Is.True);
        }
    }
}