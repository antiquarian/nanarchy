using System.Collections.Generic;
using Nanarchy.Core;
using Nanarchy.Core.Interfaces;

namespace Nanarchy.Service
{
    public interface IHierarchyService
    {
        HierarchyNode InitializeHierarchy(ITarget rootTarget);
        HierarchyNode GetRootNode();
        HierarchyNode InsertNode(HierarchyNode node, ITarget childTarget);
        HierarchyNode PrepareForInsertNode(HierarchyNode parentNode, HierarchyNode childNode);
        void DeleteNode(HierarchyNode node);
        HierarchyNode GetNodeByTarget(ITarget node);
        IList<HierarchyNode> GetChildren(HierarchyNode parentNode);
        IList<HierarchyNode> GetDescendants(HierarchyNode parentNode, bool orderTopDown, bool includeParent);
        IList<HierarchyNode> GetAncestors(HierarchyNode node, bool orderTopDown, bool includeChild);
        HierarchyNode GetParent(HierarchyNode node);
    }
}