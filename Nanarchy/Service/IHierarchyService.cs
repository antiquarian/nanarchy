using System.Collections.Generic;

namespace Nanarchy.Service
{
    public interface IHierarchyService
    {
        HierarchyNode InitializeHierarchy(INodeTarget rootTarget);
        HierarchyNode GetRootNode();
        HierarchyNode InsertNode(HierarchyNode node, INodeTarget childTarget);
        HierarchyNode PrepareForInsertNode(HierarchyNode parentNode, HierarchyNode childNode);
        void DeleteNode(HierarchyNode node);
        HierarchyNode GetNodeByTarget(INodeTarget node);
        IList<HierarchyNode> GetChildren(HierarchyNode parentNode);
        IList<HierarchyNode> GetDescendants(HierarchyNode parentNode, bool orderTopDown, bool includeParent);
        IList<HierarchyNode> GetAncestors(HierarchyNode node, bool orderTopDown, bool includeChild);
        HierarchyNode GetParent(HierarchyNode node);
    }
}