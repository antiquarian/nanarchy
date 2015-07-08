using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Nanarchy.Service;

namespace Nanarchy.Data.MssqlHierarchyDataProvider
{
    public class MssqlHierarchyDataProvider : IHierarchyDataProvider
    {
        #region Private Members

        private readonly string _connectionString;
        private SqlConnection _connection;

        private SqlConnection Connection
        {
            get { return _connection ?? (_connection = new SqlConnection(_connectionString)); }
        }

        #endregion

        #region Constructor

        public MssqlHierarchyDataProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion

        #region Private Methods

        private void ReloadLeftRight(Hierarchy hierarchy, HierarchyNode node)
        {
            var reloadedNode = GetNode(hierarchy, node.Id);
            node.LeftId = reloadedNode.LeftId;
            node.RightId = reloadedNode.RightId;
        }

        private HierarchyNode GetNode(Hierarchy hierarchy, int id)
        {
            var sql = string.Format(@"
				SELECT Id, left_id as LeftId, right_id as RightId, target_id as TargetId 
				from {0} 
				where id = {1}", hierarchy.TableName, id);

            HierarchyNode returnNode = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    returnNode = PopulateHierarchyNode(reader);
                }
            }
            return returnNode;
        }

        private HierarchyNode PopulateHierarchyNode(SqlDataReader reader)
        {
            var node = new HierarchyNode
            {
                Id = Convert.ToInt32(reader[0]),
                LeftId = Convert.ToInt32(reader[1]),
                RightId = Convert.ToInt32(reader[2]),
                TargetId = Convert.ToInt32(reader[3])
            };
            return node;
        }

        #endregion

        #region Hierarchy Methods

        public void PrepareForInsertNode(Hierarchy hierarchy, HierarchyNode parent)
        {
            throw new NotImplementedException();
        }

        public HierarchyNode GetRootNode(Hierarchy hierarchy)
        {
            var sql = string.Format(@"
            					select id as Id, left_id as LeftId, right_id as RightId, target_id as TargetId
            					from {0}
            					where left_id = 1", hierarchy.TableName);

            HierarchyNode returnNode = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    returnNode = PopulateHierarchyNode(reader);
                }
            }
            return returnNode;
        }


        public HierarchyNode GetNodeByTarget(Hierarchy hierarchy, INodeTarget target)
        {
            throw new NotImplementedException();
        }

        public HierarchyNode GetBaseNodeByTarget(Hierarchy hierarchy, INodeTarget target)
        {
            throw new NotImplementedException();
        }

        public List<HierarchyNode> GetNodesByTarget(Hierarchy hierarchy, INodeTarget target)
        {
            throw new NotImplementedException();
        }

        public IList<HierarchyNode> GetList(Hierarchy hierarchy)
        {
            throw new NotImplementedException();
        }

        public IList<HierarchyNode> GetChildren(Hierarchy hierarchy, HierarchyNode parent)
        {
            throw new NotImplementedException();
        }

        public HierarchyNode GetParent(Hierarchy hierarchy, HierarchyNode child)
        {
            throw new NotImplementedException();
        }

        public IList<HierarchyNode> GetDescendants(Hierarchy hierarchy, HierarchyNode parent, bool orderTopDown, bool includeParent)
        {
            throw new NotImplementedException();
        }

        public IList<HierarchyNode> GetAncestors(Hierarchy hierarchy, HierarchyNode child, bool orderTopDown, bool includeChild)
        {
            throw new NotImplementedException();
        }

        public void Add(Hierarchy hierarchy, HierarchyNode node)
        {
            var insertQuery = string.Format(@"
					INSERT INTO {0} (target_id, left_id, right_id)
					VALUES ({1}, {2}, {3})", hierarchy.TableName, node.TargetId, node.LeftId, node.RightId);
            
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(insertQuery, connection);
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }

            if (rowsAffected <= 0) return;

            var getQuery = string.Format(@"
					SELECT id
					FROM {0}
					WHERE left_id = {1}
					AND right_id = {2}",
                hierarchy.TableName, node.LeftId, node.RightId);
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(getQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    node.Id = Convert.ToInt32(reader[0]);
                }
            }
        }

        public void Delete(Hierarchy hierarchy, HierarchyNode node)
        {
            throw new NotImplementedException();
        }

        public bool IsAncestorOrSame(Hierarchy hierarchy, HierarchyNode candidateNode, HierarchyNode node)
        {
            throw new NotImplementedException();
        }


        #endregion

        //        /// <summary>
//        /// Makes a space in the hierarchy for a new node.
//        /// </summary>
//        /// <param name="hierarchy"></param>
//        /// <param name="parent"></param>
//        /// <returns></returns>
//        public void PrepareForInsertNode(IHierarchy hierarchy, HierarchyNode parent)
//        {
//            HandleGap(hierarchy, parent, true);

//            // update parent with newly generated right_id.  Could just get a new object from the repo, but don't want to kill the object inself.
//            ReloadLeftRight(hierarchy, parent);
//        }

//        private void ReloadLeftRight(IHierarchy hierarchy, HierarchyNode node)
//        {
//            var reloadedNode = GetNode(hierarchy, node.Id);
//            node.LeftId = reloadedNode.LeftId;
//            node.RightId = reloadedNode.RightId;
//        }

//        private HierarchyNode GetNode(IHierarchy hierarchy, int id)
//        {
//            var queryString = string.Format(@"
//				SELECT Id, left_id as LeftId, right_id as RightId, target_id as TargetId 
//				from {0} 
//				where id = :Id", hierarchy.TableName);
//            var node = Session.CreateSQLQuery(queryString)
//                .SetInt32("Id", id)
//                .UniqueResult();

//            return TransformResult(node);
//        }

//        private HierarchyNode TransformResult(object obj)
//        {
//            if (obj == null)
//            {
//                return null;
//            }

//            var objArray = obj.DownCastTo<object[]>();
//            var hierarchyNode = new HierarchyNode
//            {
//                Id = objArray[0].DownCastTo<int>(),
//                LeftId = objArray[1].DownCastTo<int>(),
//                RightId = objArray[2].DownCastTo<int>(),
//                TargetId = objArray[3].DownCastTo<int>()
//            };
//            return hierarchyNode;
//        }

//        public HierarchyNode GetRootNode(IHierarchy hierarchy)
//        {
//            var sql = string.Format(@"
//					select id as Id, left_id as LeftId, right_id as RightId, target_id as TargetId
//					from {0}
//					where left_id = 1", hierarchy.TableName);
//            var node = Session.CreateSQLQuery(sql)
//                .UniqueResult();

//            return TransformResult(node);
//        }

//        /// <summary>
//        /// Retrieve a node in the hierarchy based on what is being pointed to (item appears only once in the hierarchy).  
//        /// NOTE hierarchies: call to GetNodesByTargetId, returning the first instance?  Or, do we want to continue using UniqueResult to enforce?
//        /// NOTE: should this throw an exception if not found?  Or, leave that up to the caller?
//        /// </summary>
//        /// <param name="hierarchy"></param>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public HierarchyNode GetNodeByTarget(IHierarchy hierarchy, INodeTargetable target)
//        {
//            // Check.Require(!hierarchy.IsInstance, "This can only be used for non-instance hierarchies"); // this is no longer accurate.  A RuleDataHierarchy can use this function to look up a non-virtual rule.
//            var queryString = string.Format(@"
//				SELECT Id, left_id as LeftId, right_id as RightId, target_id as TargetId 
//				from {0} 
//				where target_id = :targetId", hierarchy.TableName);
//            var node = Session.CreateSQLQuery(queryString)
//                .SetInt32("targetId", target.GetTargetId())
//                .UniqueResult();
//            return TransformResult(node);
//        }

//        /// <summary>
//        /// Retrieve a base node in the hierarchy based on what is being pointed to.  For an instance hierarchy, there may be multiple
//        /// nodes that have the same target.  The RulesHierarchy will always have a node with a parent of root.  All rules need to be 
//        /// defined somewhere.  The RulesHierarchy defines rules always with a parent of root, but these can be reused elsewhere.
//        /// 
//        /// TODO BKR - rename?  See notes in RuleService.  Does this get the Base node if multiple nodes point to the target?  If there
//        /// is only one node, does it get that...even if it isn't a base node?  The sql below looks just wrong.  The NOT EXISTS
//        /// will always pass.  So, this function effectively does the same thing as GetNodeByTarget().  The concept seems valid, the impl is 
//        /// wrong.  But, we haven't had a case of reusing rules yet, so it just works.
//        /// </summary>
//        /// <param name="hierarchy"></param>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public HierarchyNode GetBaseNodeByTarget(IHierarchy hierarchy, INodeTargetable target)
//        {
//            Check.Require(hierarchy.IsInstance, "This should only be used for instance hierarchies.");
//            var queryString =
//                string.Format(
//                    @"
//					SELECT Id, left_id as LeftId, right_id as RightId, target_id as TargetId
//					FROM {0} h
//					WHERE target_id = :targetId
//					AND NOT EXISTS (
//						SELECT *
//						FROM {0} b
//						WHERE b.target_id = :targetId
//						AND b.left_id < h.left_id
//						AND b.right_id > h.right_id
//						and b.left_id <> 1)",
//                    hierarchy.TableName);
//            var node = Session.CreateSQLQuery(queryString)
//                .SetInt32("targetId", target.GetTargetId())
//                .UniqueResult();
//            return TransformResult(node);
//        }

//        public List<HierarchyNode> GetNodesByTarget(IHierarchy hierarchy, INodeTargetable target)
//        {
//            // NOTE hierarchies: move the logic from GetNodeByTargetId() to here?  (see notes above)  That method will return result(0), if found.
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// Gets all HierachyNodes for this hierarchy.
//        /// Currently used for testing.  In the future, something like this would be used for caching.
//        /// During application startup, use this to select all nodes for a hierarchy.  Every other request 
//        /// queries what is in memory, using the nested set patterns.
//        /// </summary>
//        /// <param name="hierarchy"></param>
//        /// <returns></returns>
//        public IList<HierarchyNode> GetList(IHierarchy hierarchy)
//        {
//            var query = Session.CreateSQLQuery(
//                string.Format(
//                    @"
//					SELECT Id, left_id as LeftId, right_id as RightId, target_id as TargetId 
//					FROM {0} n 
//					order by n.left_id",
//                    hierarchy.TableName));

//            var results = query.List();
//            return TransformResultList(results);
//        }

//        private IList<HierarchyNode> TransformResultList(object obj)
//        {
//            var results = obj.DownCastTo<ArrayList>();
//            var newList = new List<HierarchyNode>();
//            foreach (var node in results)
//            {
//                newList.Add(TransformResult(node));
//            }

//            return newList;
//        }

//        /// <summary>
//        /// Get a list of children (only the first level down) from the given node in the hierarchy ordered from left to right by position in the hierarchy
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="parent"></param>
//        /// <returns></returns>
//        public IList<HierarchyNode> GetChildren(IHierarchy hierarchy, HierarchyNode parent)
//        {
//            var query = Session.CreateSQLQuery(
//                string.Format(
//                    @"
//					SELECT Id, left_id as LeftId, right_id as RightId, target_id as TargetId 
//					FROM {0} n 
//					where n.left_id > :parentLeft 
//						and n.right_id < :parentRight 
//						and not exists (
//							select * 
//							from {0} mid 
//							where mid.left_id between :parentLeft and :parentRight 
//							AND n.left_id between mid.left_id and mid.right_id 
//							and mid.id != n.id and mid.id != :parentId) 
//					order by n.left_id",
//                    hierarchy.TableName))
//                .SetInt32("parentId", parent.Id)
//                .SetInt32("parentLeft", parent.LeftId)
//                .SetInt32("parentRight", parent.RightId);

//            var results = query.List();
//            return TransformResultList(results);

//            //var transformed = query.SetResultTransformer(Transformers.AliasToBean(typeof(HierarchyNode)));
//            //var theList = transformed.List();

//            //var newList = new List<HierarchyNode>();
//            //foreach (var rawNode in theList)
//            //{
//            //    newList.Add(rawNode.DownCastTo<HierarchyNode>());
//            //}

//            //return newList;
//        }

//        public HierarchyNode GetParent(IHierarchy hierarchy, HierarchyNode child)
//        {
//            var nodeList = GetAncestors(hierarchy, child, false, false, 1);
//            if (nodeList != null && nodeList.Count > 0)
//                return nodeList[0];
//            return null;
//        }

//        public IList<HierarchyNode> GetDescendants(IHierarchy hierarchy, HierarchyNode parent, bool orderTopDown, bool includeParent)
//        {
//            return GetDescendants(hierarchy, parent, orderTopDown, includeParent, 0);
//        }

//        public IList<HierarchyNode> GetAncestors(IHierarchy hierarchy, HierarchyNode child, bool orderTopDown, bool includeChild)
//        {
//            return GetAncestors(hierarchy, child, orderTopDown, includeChild, 0);
//        }


//        /// <summary>
//        /// Gets a list of all the nodes that are children of the given node -- walks all the way down the tree
//        /// Order is based on orderTopDown param
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="parent"></param>
//        /// <param name="orderTopDown"></param>
//        /// <param name="includeParent"></param>
//        /// <param name="maxResults"></param>
//        /// <returns></returns>
//        private IList<HierarchyNode> GetDescendants(IHierarchy hierarchy, HierarchyNode parent, bool orderTopDown, bool includeParent, int maxResults)
//        {
//            var orderString = CoreConstants.OrderAscendingString;
//            if (!orderTopDown)
//                orderString = CoreConstants.OrderDescendingString;

//            var includeParentString = "=";
//            if (!includeParent)
//                includeParentString = "";

//            var query = Session.CreateSQLQuery(
//                string.Format(
//                    @"
//					SELECT Id, left_id as LeftId, right_id as RightId, target_id as TargetId 
//					FROM {0} n 
//					where n.left_id >{2} :parentLeft 
//						and n.right_id <{2} :parentRight
//					order by n.left_id {1}",
//                    hierarchy.TableName, orderString, includeParentString))
//                .SetInt32("parentLeft", parent.LeftId)
//                .SetInt32("parentRight", parent.RightId);
//            //.SetResultTransformer(Transformers.AliasToBean(typeof(HierarchyNode)));

//            var results = query.List();
//            return TransformResultList(results);


//            // TODO - if we aren't using MaxResults, let's get rid if the parameter
//            //if (maxResults > 0)
//            //    query.SetMaxResults(maxResults);
//            //var queryResults = query.List();
//            //var newList = new List<HierarchyNode>();
//            //foreach (var result in queryResults)
//            //{
//            //    newList.Add(result.DownCastTo<HierarchyNode>());
//            //}
//            //return newList;
//        }

//        /// <summary>
//        /// Get a list of all of the nodes that are parents of the given node -- walks all the way up the tree.  
//        /// Order is based on orderTopDown param
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="child"></param>
//        /// <param name="orderTopDown"></param>
//        /// <param name="includeChild"></param>
//        /// <param name="maxResults"></param>
//        /// <returns></returns>
//        private IList<HierarchyNode> GetAncestors(IHierarchy hierarchy, HierarchyNode child, bool orderTopDown, bool includeChild, int maxResults)
//        {
//            var orderString = CoreConstants.OrderAscendingString;
//            if (!orderTopDown)
//                orderString = CoreConstants.OrderDescendingString;

//            var includeChildString = "=";
//            if (!includeChild)
//                includeChildString = "";

//            var query = Session.CreateSQLQuery(
//                string.Format(
//                    @"
//					SELECT Id, left_id as LeftId, right_id as RightId, target_id as TargetId 
//					FROM {0} n 
//					where n.left_id <{2} :childLeft 
//						and n.right_id >{2} :childRight
//					order by n.left_id {1}", hierarchy.TableName, orderString, includeChildString))
//                .SetInt32("childLeft", child.LeftId)
//                .SetInt32("childRight", child.RightId);
//            //.SetResultTransformer(Transformers.AliasToBean(typeof(HierarchyNode)));
//            if (maxResults > 0)
//                query.SetMaxResults(maxResults);
//            var queryResults = query.List();

//            return TransformResultList(queryResults);

//            //var newList = new List<HierarchyNode>();
//            //foreach (var result in queryResults)
//            //{
//            //    newList.Add(result.DownCastTo<HierarchyNode>());
//            //}
//            //return newList;
//        }

//        /// <summary>
//        /// Add a new hierarchy node.  Note that Prepare for insert should be called to set the Left and Right IDs before the new node itself is added
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="node"></param>
//        public void Add(IHierarchy hierarchy, HierarchyNode node)
//        {
//            IQuery query = Session.CreateSQLQuery(
//                string.Format(@"
//					INSERT INTO {0} (target_id, left_id, right_id)
//					VALUES (:targetId, :leftId, :rightId)", hierarchy.TableName))
//                .SetInt32("targetId", node.TargetId)
//                .SetInt32("leftId", node.LeftId)
//                .SetInt32("rightId", node.RightId);

//            query.ExecuteUpdate();

//            // update node with newly generated ID
//            node.Id = Session.CreateSQLQuery(
//                string.Format(@"
//					SELECT id
//					FROM {0}
//					WHERE left_id = :leftId
//					AND right_id = :rightId",
//                    hierarchy.TableName))
//                .SetInt32("leftId", node.LeftId)
//                .SetInt32("rightId", node.RightId)
//                .UniqueResult<int>();
//        }

//        /// <summary>
//        /// Delete a leaf-node from the hierarchy
//        /// Supports deleting a leaf (a node with no children).
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="node"></param>
//        public void Delete(IHierarchy hierarchy, HierarchyNode node)
//        {

//            Check.Require(node.RightId == node.LeftId + 1, "Deletion of non-Leaf nodes is not supported", new NotSupportedException());

//            IQuery query = Session.CreateSQLQuery(
//                string.Format(@"
//					DELETE FROM {0} 
//					WHERE id = :nodeId ", hierarchy.TableName))
//                .SetInt32("nodeId", node.Id);
//            query.ExecuteUpdate();

//            // TODO BKR - code review: to be consistent, HandleGap should be called by the HierarchyService.  Prepare is called by the service.  Closing up a gap should be as well.
//            // The HierarchyService should be the singleton that handles locking code.  The HierarchyNodeRepository handles the individual db calls that combine to expose a tree api.
//            HandleGap(hierarchy, node, false);
//        }

//        // TODO BKR - works with just nodes.  But, calling code may need to be smarter for instance hierarchies.
//        public bool IsAncestorOrSame(IHierarchy hierarchy, HierarchyNode candidateNode, HierarchyNode node)
//        {
//            // TODO BKR: Test needed
//            if (candidateNode.Id == node.Id) // TODO BKR: implement equals, etc... or make HierarchyNode a sharpEntity?
//                return true;

//            // TODO BKR - if this was <=, we wouldn't need the check above...

//            var count = Session.CreateSQLQuery(
//                string.Format(
//                    @"
//					SELECT count(*)
//					FROM {0}
//					WHERE id = :candidateNodeId
//					AND left_id < :nodeLeftId
//					AND right_id > :nodeRightId",
//                    hierarchy.TableName))
//                .SetInt32("nodeLeftId", node.LeftId)
//                .SetInt32("nodeRightId", node.RightId)
//                .SetInt32("candidateNodeId", candidateNode.Id)
//                .UniqueResult<int>();

//            return count > 0 ? true : false;
//        }

//        /// <summary>
//        /// USE ONLY FOR TESTING
//        /// Executes the sql statement.
//        /// TODO BKR - move to generic
//        /// </summary>
//        /// <param name="sql"></param>
//        public void ExecuteSql(string sql)
//        {
//            // TODO BKR - ExecutionContext: throw an exception based on execution context here.  StructureMap can hold the context as a dependency.
//            // This should only be used in testing.  Perhaps in db build.
//            Session.CreateSQLQuery(sql).ExecuteUpdate();
//        }

//        /// <summary>
//        /// Handle the opening (insert) or closing (delete) of a gap
//        /// TODO BKR - this only handles gaps for single-nodes.  If an insert, the gap is 2.  If deleting, the gap is really node.right_id - node.left_id.  No need to guess.
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="node"></param>
//        /// <param name="isInsert"></param>
//        private void HandleGap(IHierarchy hierarchy, HierarchyNode node, bool isInsert)
//        {
//            int offset = 2;
//            if (!isInsert)
//                offset = -2;

//            string queryString = string.Format(@"
//				UPDATE {0} 
//				SET left_id = CASE 
//						WHEN left_id > :node_right 
//						THEN left_id + :offset 
//						ELSE left_id END, 
//					right_id = CASE 
//						WHEN right_id >= :node_right 
//						THEN right_id + :offset 
//						ELSE right_id END 
//				WHERE right_id >= :node_right", hierarchy.TableName);

//            IQuery query = Session.CreateSQLQuery(queryString)
//                .SetInt32("node_right", node.RightId)
//                .SetInt32("offset", offset);

//            int result = query.ExecuteUpdate();
//        }

    }
}
