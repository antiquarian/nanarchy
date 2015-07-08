using System.Collections.Generic;
using Dell.Hierarchy.Service;

namespace Dell.Hierarchy.Data
{
    public interface IHierarchyDataProvider
    {
        /// <summary>
        /// Makes a space in the hierarchy for a new node.
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        void PrepareForInsertNode(IHierarchy hierarchy, HierarchyNode parent);

        HierarchyNode GetRootNode(IHierarchy hierarchy);

        /// <summary>
        /// Retrieve a node in the hierarchy based on what is being pointed to (item appears only once in the hierarchy).  
        /// NOTE hierarchies: call to GetNodesByTargetId, returning the first instance?  Or, do we want to continue using UniqueResult to enforce?
        /// NOTE: should this throw an exception if not found?  Or, leave that up to the caller?
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        HierarchyNode GetNodeByTarget(IHierarchy hierarchy, INodeTarget target);

        /// <summary>
        /// Retrieve a base node in the hierarchy based on what is being pointed to.  For an instance hierarchy, there may be multiple
        /// nodes that have the same target.  The RulesHierarchy will always have a node with a parent of root.  All rules need to be 
        /// defined somewhere.  The RulesHierarchy defines rules always with a parent of root, but these can be reused elsewhere.
        /// 
        /// TODO BKR - rename?  See notes in RuleService.  Does this get the Base node if multiple nodes point to the target?  If there
        /// is only one node, does it get that...even if it isn't a base node?  The sql below looks just wrong.  The NOT EXISTS
        /// will always pass.  So, this function effectively does the same thing as GetNodeByTarget().  The concept seems valid, the impl is 
        /// wrong.  But, we haven't had a case of reusing rules yet, so it just works.
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        HierarchyNode GetBaseNodeByTarget(IHierarchy hierarchy, INodeTarget target);

        List<HierarchyNode> GetNodesByTarget(IHierarchy hierarchy, INodeTarget target);

        /// <summary>
        /// Gets all HierachyNodes for this hierarchy.
        /// Currently used for testing.  In the future, something like this would be used for caching.
        /// During application startup, use this to select all nodes for a hierarchy.  Every other request 
        /// queries what is in memory, using the nested set patterns.
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <returns></returns>
        IList<HierarchyNode> GetList(IHierarchy hierarchy);

        /// <summary>
        /// Get a list of children (only the first level down) from the given node in the hierarchy ordered from left to right by position in the hierarchy
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        IList<HierarchyNode> GetChildren(IHierarchy hierarchy, HierarchyNode parent);

        HierarchyNode GetParent(IHierarchy hierarchy, HierarchyNode child);
        IList<HierarchyNode> GetDescendants(IHierarchy hierarchy, HierarchyNode parent, bool orderTopDown, bool includeParent);
        IList<HierarchyNode> GetAncestors(IHierarchy hierarchy, HierarchyNode child, bool orderTopDown, bool includeChild);

        /// <summary>
        /// Add a new hierarchy node.  Note that Prepare for insert should be called to set the Left and Right IDs before the new node itself is added
        /// </summary>
        /// <param name="type"></param>
        /// <param name="node"></param>
        void Add(IHierarchy hierarchy, HierarchyNode node);

        /// <summary>
        /// Delete a leaf-node from the hierarchy
        /// Supports deleting a leaf (a node with no children).
        /// </summary>
        /// <param name="type"></param>
        /// <param name="node"></param>
        void Delete(IHierarchy hierarchy, HierarchyNode node);

        bool IsAncestorOrSame(IHierarchy hierarchy, HierarchyNode candidateNode, HierarchyNode node);

        /// <summary>
        /// USE ONLY FOR TESTING
        /// Executes the sql statement.
        /// TODO BKR - move to generic
        /// </summary>
        /// <param name="sql"></param>
        void ExecuteSql(string sql);
    }
}