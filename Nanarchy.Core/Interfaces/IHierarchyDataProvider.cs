using System.Collections.Generic;

namespace Nanarchy.Core.Interfaces
{
    public interface IHierarchyDataProvider
    {
        /// <summary>
        /// Makes a space in the hierarchy for a new node.
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        void PrepareForInsertNode(HierarchyEntry hierarchy, HierarchyNode parent);

        /// <summary>
        /// Get the root (originating) node of a hierarchy.
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <returns></returns>
        HierarchyNode GetRootNode(HierarchyEntry hierarchy);

        /// <summary>
        /// Get a list of children (only the first level down) from the given node in the hierarchy ordered from left to right by position in the hierarchy
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        List<HierarchyNode> GetChildren(HierarchyEntry hierarchy, HierarchyNode parent);

        /// <summary>
        /// Get the immediate parent of the given node.
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        HierarchyNode GetParent(HierarchyEntry hierarchy, HierarchyNode child);
        
        /// <summary>
        /// Get all Descendants (in a collection) of the given Node.
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="parent"></param>
        /// <param name="orderTopDown"></param>
        /// <param name="includeParent"></param>
        /// <returns></returns>
        List<HierarchyNode> GetDescendants(HierarchyEntry hierarchy, HierarchyNode parent, bool orderTopDown, bool includeParent);

        /// <summary>
        /// Get all Ancestors (in a collection) of the given Node.
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="child"></param>
        /// <param name="orderTopDown"></param>
        /// <param name="includeChild"></param>
        /// <returns></returns>
        List<HierarchyNode> GetAncestors(HierarchyEntry hierarchy, HierarchyNode child, bool orderTopDown, bool includeChild);

        /// <summary>
        /// Add a new hierarchy node.  
        /// NOTE: PrepareForInsertNode should be called to set the Left and Right IDs before the new node itself is added
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="node"></param>
        int Add(HierarchyEntry hierarchy, HierarchyNode node);

        /// <summary>
        /// Delete a leaf-node from the hierarchy
        /// Supports deleting a leaf (a node with no children).
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="node"></param>
        bool Delete(HierarchyEntry hierarchy, HierarchyNode node);
    }
}