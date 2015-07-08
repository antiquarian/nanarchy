using System.Collections.Generic;
using Nanarchy.Service;

namespace Nanarchy.Data
{
    public interface IHierarchyDataProvider
    {
        /// <summary>
        /// Makes a space in the hierarchy for a new node.
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        void PrepareForInsertNode(Hierarchy hierarchy, HierarchyNode parent);

        HierarchyNode GetRootNode(Hierarchy hierarchy);

        /// <summary>
        /// Retrieve a node in the hierarchy based on what is being pointed to (item appears only once in the hierarchy).  
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        HierarchyNode GetNodeByTarget(Hierarchy hierarchy, INodeTarget target);

        /// <summary>
        /// Retrieve a base node in the hierarchy based on what is being pointed to.
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        HierarchyNode GetBaseNodeByTarget(Hierarchy hierarchy, INodeTarget target);

        List<HierarchyNode> GetNodesByTarget(Hierarchy hierarchy, INodeTarget target);

        /// <summary>
        /// Gets all HierachyNodes for this hierarchy.
        /// Currently used for testing.  In the future, something like this would be used for caching.
        /// During application startup, use this to select all nodes for a hierarchy.  Every other request 
        /// queries what is in memory, using the nested set patterns.
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <returns></returns>
        IList<HierarchyNode> GetList(Hierarchy hierarchy);

        /// <summary>
        /// Get a list of children (only the first level down) from the given node in the hierarchy ordered from left to right by position in the hierarchy
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        IList<HierarchyNode> GetChildren(Hierarchy hierarchy, HierarchyNode parent);

        HierarchyNode GetParent(Hierarchy hierarchy, HierarchyNode child);
        IList<HierarchyNode> GetDescendants(Hierarchy hierarchy, HierarchyNode parent, bool orderTopDown, bool includeParent);
        IList<HierarchyNode> GetAncestors(Hierarchy hierarchy, HierarchyNode child, bool orderTopDown, bool includeChild);

        /// <summary>
        /// Add a new hierarchy node.  Note that Prepare for insert should be called to set the Left and Right IDs before the new node itself is added
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="node"></param>
        void Add(Hierarchy hierarchy, HierarchyNode node);

        /// <summary>
        /// Delete a leaf-node from the hierarchy
        /// Supports deleting a leaf (a node with no children).
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="node"></param>
        void Delete(Hierarchy hierarchy, HierarchyNode node);

        bool IsAncestorOrSame(Hierarchy hierarchy, HierarchyNode candidateNode, HierarchyNode node);
    }
}