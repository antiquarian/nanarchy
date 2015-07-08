using System.Collections.Generic;
using Moq;
using Nanarchy.Data;
using Nanarchy.Service;
using Nanarchy.Tests.TestData;
using NUnit.Framework;

namespace Nanarchy.Tests
{
    [TestFixture]
    public class When_using_HierarchyService // : UnitTestsFor<HierarchyService>
    {
        [Test]
        public void Should_return_valid_Root_Node_calling_Initializehierarchy()
        {
            // arrange
            var hierarchy = new TestHierarchy("Profile"); 
            var rootNodeTarget = new TestTarget
            {
                Id = 0,
                Name = "Profile Root"
            };
            var mock = new Mock<IHierarchyDataProvider>();
            mock.Setup(p => p.Add(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>()));
            var service = new HierarchyService(mock.Object, hierarchy);

            // act
            var rootHierarchyNode = service.InitializeHierarchy(rootNodeTarget);
            
            // assert
            Assert.That(rootHierarchyNode, Is.Not.Null);
            Assert.That(rootHierarchyNode.LeftId, Is.EqualTo(1));
            Assert.That(rootHierarchyNode.RightId, Is.EqualTo(2));
            Assert.That(rootHierarchyNode.Id, Is.EqualTo(0));

            mock.Verify(p => p.Add(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>()));
        }

        [Test]
        public void Should_return_valid_node_calling_GetRootNode()
        {
            // arrange
            var hierarchy = new TestHierarchy("Profile");
            var rootNodeTarget = new HierarchyNode
            {
                Id = 0,
                LeftId = 1,
                RightId = 2
            };
            var mock = new Mock<IHierarchyDataProvider>();
            mock
                .Setup(p => p.GetRootNode(It.IsAny<Hierarchy>()))
                .Returns(rootNodeTarget);
            var service = new HierarchyService(mock.Object, hierarchy);

            // act
            var rootHierarchyNode = service.GetRootNode();

            // assert
            Assert.That(rootHierarchyNode, Is.Not.Null);
            Assert.That(rootHierarchyNode.LeftId, Is.EqualTo(1));
            Assert.That(rootHierarchyNode.RightId, Is.EqualTo(2));
            Assert.That(rootHierarchyNode.Id, Is.EqualTo(0));

            mock.Verify(p => p.GetRootNode(It.IsAny<Hierarchy>()));
        }

        [Test]
        public void Should_return_valid_node_calling_PrepareForInsertNode()
        {
            // arrange
            var hierarchy = new TestHierarchy("Profile");
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

            var mock = new Mock<IHierarchyDataProvider>();
            mock.Setup(p => p.PrepareForInsertNode(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>()));
            var service = new HierarchyService(mock.Object, hierarchy);

            // act
            var newHierarchyNode = service.PrepareForInsertNode(parentNode, childNode);

            // assert
            Assert.That(newHierarchyNode, Is.Not.Null);
            Assert.That(newHierarchyNode.LeftId, Is.EqualTo(parentNode.RightId));
            Assert.That(newHierarchyNode.RightId, Is.EqualTo(parentNode.RightId + 1));
            Assert.That(newHierarchyNode.Id, Is.EqualTo(0));

            mock.Verify(p => p.PrepareForInsertNode(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>()));
        }

        [Test]
        public void Should_return_valid_node_calling_InsertNode()
        {
            // arrange
            var hierarchy = new TestHierarchy("Profile");
            var parentNode = new HierarchyNode
            {
                Id = 0,
                LeftId = 1,
                RightId = 2
            };
            var childTarget = new TestTarget
            {
                Id = 345,
                Name = "Its a boy!"
            };

            var mock = new Mock<IHierarchyDataProvider>();
            mock.Setup(p => p.PrepareForInsertNode(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>()));
            mock.Setup(p => p.Add(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>()));
            var service = new HierarchyService(mock.Object, hierarchy);

            // act
            var newHierarchyNode = service.InsertNode(parentNode, childTarget);

            // assert
            Assert.That(newHierarchyNode, Is.Not.Null);
            Assert.That(newHierarchyNode.LeftId, Is.EqualTo(parentNode.RightId));
            Assert.That(newHierarchyNode.RightId, Is.EqualTo(parentNode.RightId + 1));
            Assert.That(newHierarchyNode.Id, Is.EqualTo(0));

            mock.Verify(p => p.PrepareForInsertNode(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>()));
            mock.Verify(p => p.Add(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>()));
        }


        [Test]
        public void Should_verify_calling_DeleteNode()
        {
            // arrange
            var hierarchy = new TestHierarchy("Profile");
            var deleteNode = new HierarchyNode
            {
                Id = 1,
                LeftId = 2,
                RightId = 3
            };
            var mock = new Mock<IHierarchyDataProvider>();
            mock.Setup(p => p.Delete(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>()));
            var service = new HierarchyService(mock.Object, hierarchy);

            // act
            service.DeleteNode(deleteNode);

            // assert
            mock.Verify(p =>p.Delete(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>()));
        }


        [Test]
        public void Should_verify_calling_GetNodeByTarget()
        {
            // arrange
            var hierarchy = new TestHierarchy("Profile");
            var testTarget = new TestTarget
            {
                Id = 345,
                Name = "Its a target!"
            };
            var testNode = new HierarchyNode
            {
                Id = 1,
                LeftId = 2,
                RightId = 3,
                TargetId = testTarget.Id
            };
            var mock = new Mock<IHierarchyDataProvider>();
            mock.Setup(p => p.GetNodeByTarget(It.IsAny<Hierarchy>(), It.IsAny<INodeTarget>()))
                .Returns(testNode);
            var service = new HierarchyService(mock.Object, hierarchy);

            // act
            var resultNode = service.GetNodeByTarget(testTarget);

            // assert
            Assert.That(resultNode, Is.Not.Null);
            Assert.That(resultNode.Id, Is.EqualTo(1));
            Assert.That(resultNode.LeftId, Is.EqualTo(testNode.LeftId));
            Assert.That(resultNode.RightId, Is.EqualTo(testNode.RightId));
            Assert.That(resultNode.TargetId, Is.EqualTo(testNode.TargetId));

            mock.Verify(p => p.GetNodeByTarget(It.IsAny<Hierarchy>(), It.IsAny<INodeTarget>()));
        }

        [Test]
        public void Should_return_children_calling_GetChildren()
        {
            // arrange
            var hierarchy = new TestHierarchy("Profile");
            var testTarget = new TestTarget
            {
                Id = 345,
                Name = "Its a target!"
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

            var mock = new Mock<IHierarchyDataProvider>();
            mock.Setup(p => p.GetChildren(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>()))
                .Returns(new List<HierarchyNode>{ child1, child2});
            var service = new HierarchyService(mock.Object, hierarchy);

            // act
            var resultNode = service.GetChildren(parentNode);

            // assert
            Assert.That(resultNode, Is.Not.Null);
            Assert.That(resultNode.Count, Is.EqualTo(2));
            Assert.That(resultNode[0].Id, Is.EqualTo(child1.Id));
            Assert.That(resultNode[0].LeftId, Is.EqualTo(child1.LeftId));
            Assert.That(resultNode[0].RightId, Is.EqualTo(child1.RightId));
            Assert.That(resultNode[0].TargetId, Is.EqualTo(child1.TargetId));

            mock.Verify(p => p.GetChildren(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>()));
        }

        [Test]
        public void Should_return_descendants_calling_GetDescendants()
        {
            // arrange
            var hierarchy = new TestHierarchy("Profile");
            var testTarget = new TestTarget
            {
                Id = 345,
                Name = "Its a target!"
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

            var mock = new Mock<IHierarchyDataProvider>();
            mock.Setup(p => p.GetDescendants(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>(), true, true))
                .Returns(new List<HierarchyNode> { parentNode, child1, child2 });
            var service = new HierarchyService(mock.Object, hierarchy);

            // act
            var resultNode = service.GetDescendants(parentNode, true, true);

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

            mock.Verify(p => p.GetDescendants(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>(), true, true));
        }

        [Test]
        public void Should_return_ancestors_calling_GetAncestors()
        {
            // arrange
            var hierarchy = new TestHierarchy("Profile");
            var testTarget = new TestTarget
            {
                Id = 345,
                Name = "Its a target!"
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

            var mock = new Mock<IHierarchyDataProvider>();
            mock.Setup(p => p.GetAncestors(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>(), true, true))
                .Returns(new List<HierarchyNode> { ancestor2, ancestor1, node });
            var service = new HierarchyService(mock.Object, hierarchy);

            // act
            var resultNode = service.GetAncestors(node, true, true);

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

            mock.Verify(p => p.GetAncestors(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>(), true, true));
        }

        [Test]
        public void Should_return_parent_calling_GetParent()
        {
            // arrange
            var hierarchy = new TestHierarchy("Profile");
            var testTarget = new TestTarget
            {
                Id = 345,
                Name = "Its a target!"
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


            var mock = new Mock<IHierarchyDataProvider>();
            mock.Setup(p => p.GetParent(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>()))
                .Returns(parentNode);
            var service = new HierarchyService(mock.Object, hierarchy);

            // act
            var resultNode = service.GetParent(childNode);

            // assert
            Assert.That(resultNode, Is.Not.Null);
            Assert.That(resultNode.Id, Is.EqualTo(parentNode.Id));
            Assert.That(resultNode.LeftId, Is.EqualTo(parentNode.LeftId));
            Assert.That(resultNode.RightId, Is.EqualTo(parentNode.RightId));
            Assert.That(resultNode.TargetId, Is.EqualTo(parentNode.TargetId));


            mock.Verify(p => p.GetParent(It.IsAny<Hierarchy>(), It.IsAny<HierarchyNode>()));
        }
    }
}