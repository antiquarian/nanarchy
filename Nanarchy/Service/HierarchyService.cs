using System.Collections.Generic;
using Nanarchy.Data;

namespace Nanarchy.Service
{
    public class HierarchyService : IHierarchyService
    {
        private readonly IHierarchyDataProvider _hierarchyDataProvider;
        private readonly Hierarchy _hierarchy;

        public HierarchyService(IHierarchyDataProvider hierarchyDataProvider, Hierarchy hierarchy)
        {
            _hierarchyDataProvider = hierarchyDataProvider;
            _hierarchy = hierarchy;
        }

        public HierarchyNode InitializeHierarchy(INodeTarget rootTarget)
        {
            var rootNode = new HierarchyNode
            {
                LeftId = 1,
                RightId = 2,
                TargetId = rootTarget == null ? 0 : rootTarget.Id 
            };

            // initialize the target table for the node
            _hierarchyDataProvider.Add(_hierarchy, rootNode);

            return rootNode;
        }

        public HierarchyNode GetRootNode()
        {
            return _hierarchyDataProvider.GetRootNode(_hierarchy);
        }

        public HierarchyNode InsertNode(HierarchyNode parentNode, INodeTarget childTarget)
        {
            var childNode = new HierarchyNode { TargetId = childTarget.Id };
            childNode = PrepareForInsertNode(parentNode, childNode);
            _hierarchyDataProvider.Add(_hierarchy, childNode);
            return childNode;
        }

        public HierarchyNode PrepareForInsertNode(HierarchyNode parentNode, HierarchyNode childNode)
        {
            childNode.LeftId = parentNode.RightId;
            _hierarchyDataProvider.PrepareForInsertNode(_hierarchy, parentNode);
            childNode.RightId = childNode.LeftId + 1;
            return childNode;
        }

        public void DeleteNode(HierarchyNode node)
        {
            _hierarchyDataProvider.Delete(_hierarchy, node);
        }

        public HierarchyNode GetNodeByTarget(INodeTarget target)
        {
            return _hierarchyDataProvider.GetNodeByTarget(_hierarchy, target);
        }

        public IList<HierarchyNode> GetChildren(HierarchyNode parentNode)
        {
            return _hierarchyDataProvider.GetChildren(_hierarchy, parentNode);
        }

        public IList<HierarchyNode> GetDescendants(HierarchyNode parentNode, bool orderTopDown, bool includeParent)
        {
            return _hierarchyDataProvider.GetDescendants(_hierarchy, parentNode, orderTopDown, includeParent);
        }

        public IList<HierarchyNode> GetAncestors(HierarchyNode node, bool orderTopDown, bool includeChild)
        {
            return _hierarchyDataProvider.GetAncestors(_hierarchy, node, orderTopDown, includeChild);
        }

        public HierarchyNode GetParent(HierarchyNode node)
        {
            return _hierarchyDataProvider.GetParent(_hierarchy, node);
        }
    }
}