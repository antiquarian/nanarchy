using System;
using System.Collections.Generic;
using Moq;
using Nanarchy.Core;
using Nanarchy.Core.Interfaces;
using Nanarchy.Data.Mssql;
using Nanarchy.Service;
using Nanarchy.Tests.TestData;
using NUnit.Framework;

namespace Nanarchy.Tests
{
    [TestFixture]
    public class When_using_HierarchyService 
    {
        private HierarchyEntry hierarchyEntry = new HierarchyEntry { Name = "Profile", TableName = "profile" }; 
        [Test]
        public void Should_return_valid_Root_Node_calling_Initializehierarchy()
        {
            // arrange
            var rootNodeTarget = new TestTarget
            {
                Id = 0,
                Data = new TestTargetData
                {
                    TestString = "Profile Root"
                }
            };
            var mockHierarchyDataProvider = new Mock<IHierarchyDataProvider>();
            mockHierarchyDataProvider.Setup(p => p.Add(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>()));
            var mockHierarchyEntryRepository = new Mock<IHierarchyEntryRepository>();
            var mockTargetEntryRepository = new Mock<ITargetEntryRepository>();
            var mockTargetRepository = new Mock<ITargetRepository>();
            var mockTargetTypes = new List<Type>();

            var service = new HierarchyService(mockHierarchyDataProvider.Object, mockHierarchyEntryRepository.Object, mockTargetEntryRepository.Object, mockTargetRepository.Object, mockTargetTypes);

            // act
            var rootHierarchyNode = service.InitializeHierarchy(hierarchyEntry, rootNodeTarget);
            
            // assert
            Assert.That(rootHierarchyNode, Is.Not.Null);
            Assert.That(rootHierarchyNode.LeftId, Is.EqualTo(1));
            Assert.That(rootHierarchyNode.RightId, Is.EqualTo(2));
            Assert.That(rootHierarchyNode.Id, Is.EqualTo(0));

            mockHierarchyDataProvider
                .Verify(p => p.Add(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>()));
        }

        //[Test]
        //public void Should_return_valid_Root_Node_calling_InitializeDatabase_when_tables_exist()
        //{
        //    // arrange
        //    var mockHierarchyDataProvider = new Mock<IHierarchyDataProvider>();
        //    var mockHierarchyEntryRepository = new Mock<IHierarchyEntryRepository>();
        //    mockHierarchyEntryRepository.Setup(p => p.TableExists()).Returns(true);
        //    mockHierarchyEntryRepository.Setup(p => p.Update(It.IsAny<HierarchyEntry>()));
        //    var mockTargetEntryRepository = new Mock<ITargetEntryRepository>();
        //    mockTargetEntryRepository.Setup(p => p.TableExists()).Returns(true);
        //    mockTargetEntryRepository.Setup(p => p.Update(It.IsAny<TargetEntry>()));

        //    var service = new HierarchyService(mockHierarchyDataProvider.Object, mockHierarchyDataProvider.Object, mockHierarchyEntryRepository.Object, mockTargetEntryRepository.Object, mockTargetEntryRepository, mockTargets);

        //    // act
        //    var initalized = service.InitializeDatabase(hierarchyEntry, new List<TargetEntry>
        //    {
        //        new TargetEntry{ Name = "Target One", TableName = "target_one"},
        //        new TargetEntry{ Name = "Target Two", TableName = "target_two"},
        //    });

        //    // assert
        //    Assert.That(initalized, Is.True);

        //    mockHierarchyEntryRepository.Verify(p => p.TableExists());
        //    mockHierarchyEntryRepository.Verify(p => p.Update(It.IsAny<HierarchyEntry>()));
        //    mockTargetEntryRepository.Verify(p => p.TableExists());
        //    mockTargetEntryRepository.Verify(p => p.Update(It.IsAny<TargetEntry>()), Times.Exactly(2));
        //}

        //[Test]
        //public void Should_return_valid_Root_Node_calling_InitializeDatabase_when_tables_do_not_exist()
        //{
        //    // arrange
        //    var mockHierarchyDataProvider = new Mock<IHierarchyDataProvider>();
        //    var mockHierarchyEntryRepository = new Mock<IHierarchyEntryRepository>();
        //    mockHierarchyEntryRepository.Setup(p => p.TableExists()).Returns(false);
        //    mockHierarchyEntryRepository.Setup(p => p.TableExists()).Returns(true);
        //    mockHierarchyEntryRepository.Setup(p => p.Update(It.IsAny<HierarchyEntry>()));

        //    var mockTargetEntryRepository = new Mock<ITargetEntryRepository>();
        //    mockTargetEntryRepository.Setup(p => p.TableExists()).Returns(true);

        //    mockTargetEntryRepository.Setup(p => p.Update(It.IsAny<TargetEntry>()));
        //    var service = new HierarchyService(mockHierarchyDataProvider.Object, mockHierarchyEntryRepository.Object, mockTargetEntryRepository.Object);

        //    // act
        //    var initalized = service.InitializeDatabase(hierarchyEntry, new List<TargetEntry>
        //    {
        //        new TargetEntry{ Name = "Target One", TableName = "target_one"},
        //        new TargetEntry{ Name = "Target Two", TableName = "target_two"},
        //    });

        //    // assert
        //    Assert.That(initalized, Is.True);

        //    mockHierarchyEntryRepository.Verify(p => p.TableExists());
        //    mockHierarchyEntryRepository.Verify(p => p.Update(It.IsAny<HierarchyEntry>()));
        //    mockTargetEntryRepository.Verify(p => p.TableExists());
        //    mockTargetEntryRepository.Verify(p => p.Update(It.IsAny<TargetEntry>()), Times.Exactly(2));
        //}


        [Test]
        public void Should_return_valid_node_calling_GetRootNode()
        {
            // arrange
            var rootNodeTarget = new HierarchyNode
            {
                Id = 0,
                LeftId = 1,
                RightId = 2
            };
            var mockHierarchyDataProvider = new Mock<IHierarchyDataProvider>();
            mockHierarchyDataProvider
                .Setup(p => p.GetRootNode(It.IsAny<HierarchyEntry>()))
                .Returns(rootNodeTarget);
            var mockHierarchyEntryRepository = new Mock<IHierarchyEntryRepository>();
            var mockTargetEntryRepository = new Mock<ITargetEntryRepository>();
            var mockTargetRepository = new Mock<ITargetRepository>();
            var mockTargetTypes = new List<Type>();
            var service = new HierarchyService(mockHierarchyDataProvider.Object, mockHierarchyEntryRepository.Object, mockTargetEntryRepository.Object, mockTargetRepository.Object, mockTargetTypes);

            // act
            var rootHierarchyNode = service.GetRootNode(hierarchyEntry);

            // assert
            Assert.That(rootHierarchyNode, Is.Not.Null);
            Assert.That(rootHierarchyNode.LeftId, Is.EqualTo(1));
            Assert.That(rootHierarchyNode.RightId, Is.EqualTo(2));
            Assert.That(rootHierarchyNode.Id, Is.EqualTo(0));

            mockHierarchyDataProvider.Verify(p => p.GetRootNode(It.IsAny<HierarchyEntry>()));
        }

        [Test]
        public void Should_return_valid_node_calling_PrepareForInsertNode()
        {
            // arrange
            var parentNode = new HierarchyNode
            {
                Id = 0,
                LeftId = 1,
                RightId = 2
            };
            var childNode = new HierarchyNode
            {
                Id = 0,
                LeftId = 0,
                RightId = 0,
                TargetId = 345
            };

            var mockHierarchyDataProvider = new Mock<IHierarchyDataProvider>();
            mockHierarchyDataProvider.Setup(p => p.PrepareForInsertNode(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>()));
            var mockHierarchyEntryRepository = new Mock<IHierarchyEntryRepository>();
            var mockTargetEntryRepository = new Mock<ITargetEntryRepository>();
            var mockTargetRepository = new Mock<ITargetRepository>();
            var mockTargetTypes = new List<Type>();
            var service = new HierarchyService(mockHierarchyDataProvider.Object, mockHierarchyEntryRepository.Object, mockTargetEntryRepository.Object, mockTargetRepository.Object, mockTargetTypes);

            // act
            var newHierarchyNode = service.PrepareForInsertNode(hierarchyEntry, parentNode, childNode);

            // assert
            Assert.That(newHierarchyNode, Is.Not.Null);
            Assert.That(newHierarchyNode.LeftId, Is.EqualTo(parentNode.RightId));
            Assert.That(newHierarchyNode.RightId, Is.EqualTo(parentNode.RightId + 1));
            Assert.That(newHierarchyNode.Id, Is.EqualTo(0));

            mockHierarchyDataProvider.Verify(p => p.PrepareForInsertNode(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>()));
        }

        [Test]
        public void Should_return_valid_node_calling_InsertNode()
        {
            // arrange
            var parentNode = new HierarchyNode
            {
                Id = 0,
                LeftId = 1,
                RightId = 2
            };
            var childTarget = new TestTarget
            {
                Id = 345,
                Data = new TestTargetData
                {
                    TestString =  "Its a boy!"
                }
            };

            var mockHierarchyDataProvider = new Mock<IHierarchyDataProvider>();
            mockHierarchyDataProvider.Setup(p => p.PrepareForInsertNode(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>()));
            mockHierarchyDataProvider.Setup(p => p.Add(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>()));
            var mockHierarchyEntryRepository = new Mock<IHierarchyEntryRepository>();
            var mockTargetEntryRepository = new Mock<ITargetEntryRepository>();
            var mockTargetRepository = new Mock<ITargetRepository>();
            var mockTargetTypes = new List<Type>();
            var service = new HierarchyService(mockHierarchyDataProvider.Object, mockHierarchyEntryRepository.Object, mockTargetEntryRepository.Object, mockTargetRepository.Object, mockTargetTypes);

            // act
            var newHierarchyNode = service.InsertNode(hierarchyEntry, parentNode, childTarget);

            // assert
            Assert.That(newHierarchyNode, Is.Not.Null);
            Assert.That(newHierarchyNode.LeftId, Is.EqualTo(parentNode.RightId));
            Assert.That(newHierarchyNode.RightId, Is.EqualTo(parentNode.RightId + 1));
            Assert.That(newHierarchyNode.Id, Is.EqualTo(0));

            mockHierarchyDataProvider.Verify(p => p.PrepareForInsertNode(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>()));
            mockHierarchyDataProvider.Verify(p => p.Add(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>()));
        }


        [Test]
        public void Should_verify_calling_DeleteNode()
        {
            // arrange
            var deleteNode = new HierarchyNode
            {
                Id = 1,
                LeftId = 2,
                RightId = 3
            };
            var mockHierarchyDataProvider = new Mock<IHierarchyDataProvider>();
            mockHierarchyDataProvider.Setup(p => p.Delete(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>()));
            var mockHierarchyEntryRepository = new Mock<IHierarchyEntryRepository>();
            var mockTargetEntryRepository = new Mock<ITargetEntryRepository>();
            var mockTargetRepository = new Mock<ITargetRepository>();
            var mockTargetTypes = new List<Type>();
            var service = new HierarchyService(mockHierarchyDataProvider.Object, mockHierarchyEntryRepository.Object, mockTargetEntryRepository.Object, mockTargetRepository.Object, mockTargetTypes);

            // act
            service.DeleteNode(hierarchyEntry, deleteNode);

            // assert
            mockHierarchyDataProvider.Verify(p => p.Delete(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>()));
        }

        [Test]
        public void Should_return_children_calling_GetChildren()
        {
            // arrange
            var testTarget = new TestTarget
            {
                Id = 345,
                Data = new TestTargetData { TestString = "Its a target!" }
            };
            var parentNode = new HierarchyNode
            {
                Id = 23,
                LeftId = 1,
                RightId = 6,
                TargetId = testTarget.Id
            };
            var child1 = new HierarchyNode
            {
                Id = 45,
                LeftId = 2,
                RightId = 3,
                TargetId = testTarget.Id
            };
            var child2 = new HierarchyNode
            {
                Id = 46,
                LeftId = 4,
                RightId = 5,
                TargetId = testTarget.Id
            };

            var mockHierarchyDataProvider = new Mock<IHierarchyDataProvider>();
            mockHierarchyDataProvider.Setup(p => p.GetChildren(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>()))
                .Returns(new List<HierarchyNode> { child1, child2 });
            var mockHierarchyEntryRepository = new Mock<IHierarchyEntryRepository>();
            var mockTargetEntryRepository = new Mock<ITargetEntryRepository>();
            var mockTargetRepository = new Mock<ITargetRepository>();
            var mockTargetTypes = new List<Type>();
            var service = new HierarchyService(mockHierarchyDataProvider.Object, mockHierarchyEntryRepository.Object, mockTargetEntryRepository.Object, mockTargetRepository.Object, mockTargetTypes);

            // act
            var resultNode = service.GetChildren(hierarchyEntry, parentNode);

            // assert
            Assert.That(resultNode, Is.Not.Null);
            Assert.That(resultNode.Count, Is.EqualTo(2));
            Assert.That(resultNode[0].Id, Is.EqualTo(child1.Id));
            Assert.That(resultNode[0].LeftId, Is.EqualTo(child1.LeftId));
            Assert.That(resultNode[0].RightId, Is.EqualTo(child1.RightId));
            Assert.That(resultNode[0].TargetId, Is.EqualTo(child1.TargetId));

            mockHierarchyDataProvider.Verify(p => p.GetChildren(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>()));
        }

        [Test]
        public void Should_return_descendants_calling_GetDescendants()
        {
            // arrange
            var testTarget = new TestTarget
            {
                Id = 345,
                Data = new TestTargetData{ TestString = "Its a target!"}
            };
            var parentNode = new HierarchyNode
            {
                Id = 23,
                LeftId = 1,
                RightId = 6,
                TargetId = testTarget.Id
            };
            var child1 = new HierarchyNode
            {
                Id = 45,
                LeftId = 2,
                RightId = 3,
                TargetId = testTarget.Id
            };
            var child2 = new HierarchyNode
            {
                Id = 46,
                LeftId = 4,
                RightId = 5,
                TargetId = testTarget.Id
            };

            var mockHierarchyDataProvider = new Mock<IHierarchyDataProvider>();
            mockHierarchyDataProvider.Setup(p => p.GetDescendants(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>(), true, true))
                .Returns(new List<HierarchyNode> { parentNode, child1, child2 });
            var mockHierarchyEntryRepository = new Mock<IHierarchyEntryRepository>();
            var mockTargetEntryRepository = new Mock<ITargetEntryRepository>();
            var mockTargetRepository = new Mock<ITargetRepository>();
            var mockTargetTypes = new List<Type>();
            var service = new HierarchyService(mockHierarchyDataProvider.Object, mockHierarchyEntryRepository.Object, mockTargetEntryRepository.Object, mockTargetRepository.Object, mockTargetTypes);

            // act
            var resultNode = service.GetDescendants(hierarchyEntry, parentNode, true, true);

            // assert
            Assert.That(resultNode, Is.Not.Null);
            Assert.That(resultNode.Count, Is.EqualTo(3));
            Assert.That(resultNode[0].Id, Is.EqualTo(parentNode.Id));
            Assert.That(resultNode[0].LeftId, Is.EqualTo(parentNode.LeftId));
            Assert.That(resultNode[0].RightId, Is.EqualTo(parentNode.RightId));
            Assert.That(resultNode[0].TargetId, Is.EqualTo(parentNode.TargetId));
            Assert.That(resultNode[1].Id, Is.EqualTo(child1.Id));
            Assert.That(resultNode[1].LeftId, Is.EqualTo(child1.LeftId));
            Assert.That(resultNode[1].RightId, Is.EqualTo(child1.RightId));
            Assert.That(resultNode[1].TargetId, Is.EqualTo(child1.TargetId));
            Assert.That(resultNode[2].Id, Is.EqualTo(child2.Id));
            Assert.That(resultNode[2].LeftId, Is.EqualTo(child2.LeftId));
            Assert.That(resultNode[2].RightId, Is.EqualTo(child2.RightId));
            Assert.That(resultNode[2].TargetId, Is.EqualTo(child2.TargetId));

            mockHierarchyDataProvider.Verify(p => p.GetDescendants(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>(), true, true));
        }

        [Test]
        public void Should_return_ancestors_calling_GetAncestors()
        {
            // arrange
            var testTarget = new TestTarget
            {
                Id = 345,
                Data = new TestTargetData { TestString = "Its a target!" }
            };
            var node = new HierarchyNode
            {
                Id = 34,
                LeftId = 2,
                RightId = 3,
                TargetId = testTarget.Id
            };
            var ancestor1 = new HierarchyNode
            {
                Id = 22,
                LeftId = 1,
                RightId = 6,
                TargetId = testTarget.Id
            };
            var ancestor2 = new HierarchyNode
            {
                Id = 11,
                LeftId = 1,
                RightId = 13,
                TargetId = testTarget.Id
            };

            var mockHierarchyDataProvider = new Mock<IHierarchyDataProvider>();
            mockHierarchyDataProvider.Setup(p => p.GetAncestors(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>(), true, true))
                .Returns(new List<HierarchyNode> { ancestor2, ancestor1, node });
            var mockHierarchyEntryRepository = new Mock<IHierarchyEntryRepository>();
            var mockTargetEntryRepository = new Mock<ITargetEntryRepository>();
            var mockTargetRepository = new Mock<ITargetRepository>();
            var mockTargetTypes = new List<Type>();
            var service = new HierarchyService(mockHierarchyDataProvider.Object, mockHierarchyEntryRepository.Object, mockTargetEntryRepository.Object, mockTargetRepository.Object, mockTargetTypes);

            // act
            var resultNode = service.GetAncestors(hierarchyEntry, node, true, true);

            // assert
            Assert.That(resultNode, Is.Not.Null);
            Assert.That(resultNode.Count, Is.EqualTo(3));
            Assert.That(resultNode[0].Id, Is.EqualTo(ancestor2.Id));
            Assert.That(resultNode[0].LeftId, Is.EqualTo(ancestor2.LeftId));
            Assert.That(resultNode[0].RightId, Is.EqualTo(ancestor2.RightId));
            Assert.That(resultNode[0].TargetId, Is.EqualTo(ancestor2.TargetId));
            Assert.That(resultNode[1].Id, Is.EqualTo(ancestor1.Id));
            Assert.That(resultNode[1].LeftId, Is.EqualTo(ancestor1.LeftId));
            Assert.That(resultNode[1].RightId, Is.EqualTo(ancestor1.RightId));
            Assert.That(resultNode[1].TargetId, Is.EqualTo(ancestor1.TargetId));
            Assert.That(resultNode[2].Id, Is.EqualTo(node.Id));
            Assert.That(resultNode[2].LeftId, Is.EqualTo(node.LeftId));
            Assert.That(resultNode[2].RightId, Is.EqualTo(node.RightId));
            Assert.That(resultNode[2].TargetId, Is.EqualTo(node.TargetId));

            mockHierarchyDataProvider.Verify(p => p.GetAncestors(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>(), true, true));
        }

        [Test]
        public void Should_return_parent_calling_GetParent()
        {
            // arrange
            var testTarget = new TestTarget
            {
                Id = 345,
                Data = new TestTargetData { TestString = "Its a target!" }
            };
            var childNode = new HierarchyNode
            {
                Id = 34,
                LeftId = 2,
                RightId = 3,
                TargetId = testTarget.Id
            };
            var parentNode = new HierarchyNode
            {
                Id = 22,
                LeftId = 1,
                RightId = 6,
                TargetId = testTarget.Id
            };


            var mockHierarchyDataProvider = new Mock<IHierarchyDataProvider>();
            mockHierarchyDataProvider.Setup(p => p.GetParent(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>()))
                .Returns(parentNode);
            var mockHierarchyEntryRepository = new Mock<IHierarchyEntryRepository>();
            var mockTargetEntryRepository = new Mock<ITargetEntryRepository>();
            var mockTargetRepository = new Mock<ITargetRepository>();
            var mockTargetTypes = new List<Type>();
            var service = new HierarchyService(mockHierarchyDataProvider.Object, mockHierarchyEntryRepository.Object, mockTargetEntryRepository.Object, mockTargetRepository.Object, mockTargetTypes);

            // act
            var resultNode = service.GetParent(hierarchyEntry, childNode);

            // assert
            Assert.That(resultNode, Is.Not.Null);
            Assert.That(resultNode.Id, Is.EqualTo(parentNode.Id));
            Assert.That(resultNode.LeftId, Is.EqualTo(parentNode.LeftId));
            Assert.That(resultNode.RightId, Is.EqualTo(parentNode.RightId));
            Assert.That(resultNode.TargetId, Is.EqualTo(parentNode.TargetId));


            mockHierarchyDataProvider.Verify(p => p.GetParent(It.IsAny<HierarchyEntry>(), It.IsAny<HierarchyNode>()));
        }
    }
}