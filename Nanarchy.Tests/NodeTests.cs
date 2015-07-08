using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Dell.Hierarchy.Tests
{
    [TestFixture]
    public class When_using_Node_and_related_classes
    {
        [Test]
        public void Should_create_new_simple_node()
        {
            // arrange
            // act
            var node = new Node
            {
                Name = "A Node",
            };

            // assert
            Assert.That(node.NodeId, Is.EqualTo(0));
            Assert.That(node.Name, Is.EqualTo("A Node"));
            Assert.That(node.Children, Is.Not.Null);
            Assert.That(node.Children.Any(), Is.False);
        }

        [Test]
        public void Should_create_new_node_with_children()
        {
            // arrange
            // act
            var node = new Node
            {
                Name = "A Node",
                Children = new List<Node>
                {
                    new Node
                    {
                        Name = "A Child Node 1"
                    },
                    new Node
                    {
                        Name = "A Child Node 2"
                    }
                }
            };

            // assert
            Assert.That(node.NodeId, Is.EqualTo(0));
            Assert.That(node.Name, Is.EqualTo("A Node"));
            Assert.That(node.Children, Is.Not.Null);
            Assert.That(node.Children.Any(), Is.True);
            Assert.That(node.Children[0].NodeId, Is.EqualTo(0));
            Assert.That(node.Children[0].Name, Is.EqualTo("A Child Node 1"));
            Assert.That(node.Children[1].NodeId, Is.EqualTo(0));
            Assert.That(node.Children[1].Name, Is.EqualTo("A Child Node 2"));
        }
    }
}
