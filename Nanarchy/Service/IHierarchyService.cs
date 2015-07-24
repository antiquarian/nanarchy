using System.Collections.Generic;
using Nanarchy.Core;

namespace Nanarchy.Service
{
    public interface IHierarchyService
    {
        HierarchyNode InitializeHierarchy(HierarchyEntry hierarchyEntry, ITarget rootTarget = null);
        HierarchyNode GetRootNode(HierarchyEntry hierarchyEntry);
        HierarchyNode InsertNode(HierarchyEntry hierarchyEntry, HierarchyNode node, ITarget childTarget);
        HierarchyNode PrepareForInsertNode(HierarchyEntry hierarchyEntry,HierarchyNode parentNode, HierarchyNode childNode);
        void DeleteNode(HierarchyEntry hierarchyEntry,HierarchyNode node);
        IList<HierarchyNode> GetChildren(HierarchyEntry hierarchyEntry,HierarchyNode parentNode);
        IList<HierarchyNode> GetDescendants(HierarchyEntry hierarchyEntry,HierarchyNode parentNode, bool orderTopDown, bool includeParent);
        IList<HierarchyNode> GetAncestors(HierarchyEntry hierarchyEntry,HierarchyNode node, bool orderTopDown, bool includeChild);
        HierarchyNode GetParent(HierarchyEntry hierarchyEntry,HierarchyNode node);
    }
}