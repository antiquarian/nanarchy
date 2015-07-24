using System;
using Nanarchy.Core;
using Nanarchy.Core.Interfaces;

namespace Nanarchy.Tests.TestData
{

    public static class TestHierarchy
    {
        public static void BuildTestHierarchy(HierarchyEntry hierarchyEntry, IHierarchyDataProvider provider)
        {
            // initialize the hierarchy
            var originalRootNode = new HierarchyNode
            {
                LeftId = 1,
                RightId = 2
            };
            var rootId = provider.Add(hierarchyEntry, originalRootNode);

            // test get root node
            var persistedRootNode = provider.GetRootNode(hierarchyEntry);

            // then insert first primary child
            var firstChild = new HierarchyNode();
            firstChild.LeftId = persistedRootNode.RightId;
            provider.PrepareForInsertNode(hierarchyEntry, persistedRootNode);
            firstChild.RightId = firstChild.LeftId + 1;
            provider.Add(hierarchyEntry, firstChild);

            persistedRootNode = provider.GetRootNode(hierarchyEntry);

            // insert second primary child
            var secondChild = new HierarchyNode();
            secondChild.LeftId = persistedRootNode.RightId;
            provider.PrepareForInsertNode(hierarchyEntry, persistedRootNode);
            secondChild.RightId = secondChild.LeftId + 1;
            provider.Add(hierarchyEntry, secondChild);

            // reset root node and get children
            persistedRootNode = provider.GetRootNode(hierarchyEntry);
            var secondChildren = provider.GetChildren(hierarchyEntry, persistedRootNode);

            // insert first secondary child
            var primaryChildAsParent = secondChildren[0];
            var secondaryChild1 = new HierarchyNode();
            secondaryChild1.LeftId = primaryChildAsParent.RightId;
            provider.PrepareForInsertNode(hierarchyEntry, primaryChildAsParent);
            secondaryChild1.RightId = secondaryChild1.LeftId + 1;
            provider.Add(hierarchyEntry, secondaryChild1);

            // reset root node and get children
            persistedRootNode = provider.GetRootNode(hierarchyEntry);
            secondChildren = provider.GetChildren(hierarchyEntry, persistedRootNode);

            // insert second secondary child
            primaryChildAsParent = secondChildren[0];
            var secondaryChild2 = new HierarchyNode();
            secondaryChild2.LeftId = primaryChildAsParent.RightId;
            provider.PrepareForInsertNode(hierarchyEntry, primaryChildAsParent);
            secondaryChild2.RightId = secondaryChild2.LeftId + 1;
            provider.Add(hierarchyEntry, secondaryChild2);


            // reset root node and get children of primary2
            persistedRootNode = provider.GetRootNode(hierarchyEntry);
            secondChildren = provider.GetChildren(hierarchyEntry, persistedRootNode);

            // insert first secondary child
            var primaryChild2AsParent = secondChildren[1];
            var secondaryChild3 = new HierarchyNode();
            secondaryChild3.LeftId = primaryChild2AsParent.RightId;
            provider.PrepareForInsertNode(hierarchyEntry, primaryChild2AsParent);
            secondaryChild3.RightId = secondaryChild3.LeftId + 1;
            provider.Add(hierarchyEntry, secondaryChild3);

            // reset root node and get children
            persistedRootNode = provider.GetRootNode(hierarchyEntry);
            secondChildren = provider.GetChildren(hierarchyEntry, persistedRootNode);

            // insert second secondary child
            primaryChild2AsParent = secondChildren[1];
            var secondaryChild4 = new HierarchyNode();
            secondaryChild4.LeftId = primaryChild2AsParent.RightId;
            provider.PrepareForInsertNode(hierarchyEntry, primaryChild2AsParent);
            secondaryChild4.RightId = secondaryChild4.LeftId + 1;
            provider.Add(hierarchyEntry, secondaryChild4);
        }
    }
}