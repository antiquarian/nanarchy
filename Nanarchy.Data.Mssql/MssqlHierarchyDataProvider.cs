using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nanarchy.Core;
using Nanarchy.Core.Interfaces;
using UsefulUtilities;

namespace Nanarchy.Data.Mssql
{
    public class MssqlHierarchyDataProvider : IHierarchyDataProvider
    {
        #region Private Members

        private readonly string _connectionString;
        private readonly IDataProvider _dataProvider;
        private Dictionary<int, string> _hierarchyInitialized;

        #endregion

        #region Constructor

        public MssqlHierarchyDataProvider(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            _connectionString = dataProvider.ConnectionString;
            _hierarchyInitialized = new Dictionary<int, string>();
        }

        #endregion

        #region Private Methods

        private void ReloadLeftRight(HierarchyEntry hierarchyEntry, HierarchyNode node)
        {
            var reloadedNode = GetNode(hierarchyEntry, node.Id);
            node.LeftId = reloadedNode.LeftId;
            node.RightId = reloadedNode.RightId;
        }

        /// <summary>
        /// Handle the opening (insert) or closing (delete) of a gap
        /// </summary>
        /// <param name="hierarchyEntry"></param>
        /// <param name="node"></param>
        /// <param name="isInsert"></param>
        private void HandleGap(HierarchyEntry hierarchyEntry, HierarchyNode node, bool isInsert)
        {
            var offset = (isInsert) ? 2 : -2;

            var queryString = string.Format(@"
        				UPDATE [{0}].[{1}] 
        				SET left_id = CASE 
        						WHEN left_id > @LeftId 
        						THEN left_id + @Offset 
        						ELSE left_id END, 
        					right_id = CASE 
        						WHEN right_id >= @RightId 
        						THEN right_id + @Offset 
        						ELSE right_id END 
        				WHERE right_id >= @RightId", hierarchyEntry.SchemaName, hierarchyEntry.TableName);

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(queryString, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("@LeftId", node.LeftId);
                    command.Parameters.AddWithValue("@RightId", node.RightId);
                    command.Parameters.AddWithValue("@Offset", offset);
                    command.ExecuteNonQuery();

                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
        }

        private HierarchyNode GetNode(HierarchyEntry hierarchyEntry, int id)
        {
            var sql = string.Format("SELECT id, left_id, right_id, target_id FROM [{0}].[{1}] WHERE id = {2}", hierarchyEntry.SchemaName, hierarchyEntry.TableName, id);

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

        private static HierarchyNode PopulateHierarchyNode(IDataRecord reader)
        {
            var node = new HierarchyNode
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                LeftId = reader.GetInt32(reader.GetOrdinal("left_id")),
                RightId = reader.GetInt32(reader.GetOrdinal("right_id")),
                TargetId = reader.GetInt32(reader.GetOrdinal("target_id"))
            };
            return node;
        }

        #endregion

        #region Hierarchy Methods

        public void PrepareForInsertNode(HierarchyEntry hierarchy, HierarchyNode parent)
        {
            HandleGap(hierarchy, parent, true);
            // update parent with newly generated right_id.  Could just get a new object from the repo, but don't want to kill the object itself.
            ReloadLeftRight(hierarchy, parent);
        }

        public HierarchyNode GetRootNode(HierarchyEntry hierarchyEntry)
        {
            var sql = string.Format("SELECT id, left_id, right_id, target_id FROM [{0}].[{1}] WHERE left_id = 1", hierarchyEntry.SchemaName, hierarchyEntry.TableName);

            HierarchyNode returnNode = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        returnNode = PopulateHierarchyNode(reader);
                    }
                }
            }
            return returnNode;
        }

        public List<HierarchyNode> GetChildren(HierarchyEntry hierarchy, HierarchyNode parent)
        {
            var children = new List<HierarchyNode>();
            var sql = 
                string.Format(
                    @"SELECT id, left_id, right_id, target_id
					FROM [{0}].[{1}] n 
					WHERE n.left_id > @ParentLeft 
						AND n.right_id < @ParentRight 
						AND NOT EXISTS (
							SELECT * 
							FROM  [{0}].[{1}] mid 
							WHERE mid.left_id BETWEEN @ParentLeft AND @ParentRight 
							AND n.left_id BETWEEN mid.left_id AND mid.right_id 
							AND mid.id != n.id AND mid.id != @ParentId) 
					ORDER BY n.left_id",
                    hierarchy.SchemaName, hierarchy.TableName);

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@ParentId", parent.Id);
                    command.Parameters.AddWithValue("@ParentLeft", parent.LeftId);
                    command.Parameters.AddWithValue("@ParentRight", parent.RightId);

                    conn.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            children.Add(PopulateHierarchyNode(reader));
                        }
                    }
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return children;
        }

        public HierarchyNode GetParent(HierarchyEntry hierarchy, HierarchyNode child)
        {
            var nodeList = GetAncestors(hierarchy, child, false, false);
            if (nodeList != null && nodeList.Count > 0)
                return nodeList[0];
            return null;
        }

        public List<HierarchyNode> GetDescendants(HierarchyEntry hierarchyEntry, HierarchyNode parent, bool orderTopDown, bool includeParent)
        {
            var descendants = new List<HierarchyNode>();
            var orderString = (orderTopDown) ? "ASC" : "DESC";
            var includeParentString = (includeParent) ? "=" : "";
            var sql =
                string.Format(
                    @"SELECT id, left_id, right_id, target_id
            			FROM [{0}].[{1}] n 
            			WHERE n.left_id >{3} @ParentLeft 
            				AND n.right_id <{3} @ParentRight
            			ORDER BY n.left_id {2}",
                    hierarchyEntry.SchemaName, hierarchyEntry.TableName, orderString, includeParentString);

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@ParentId", parent.Id);
                    command.Parameters.AddWithValue("@ParentLeft", parent.LeftId);
                    command.Parameters.AddWithValue("@ParentRight", parent.RightId);

                    conn.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            descendants.Add(PopulateHierarchyNode(reader));
                        }
                    }
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return descendants;
        }

        public List<HierarchyNode> GetAncestors(HierarchyEntry hierarchyEntry, HierarchyNode child, bool orderTopDown, bool includeChild)
        {
            var ancestors = new List<HierarchyNode>();
            var orderString = (orderTopDown) ? "ASC" : "DESC";
            var includeChildString = (includeChild) ? "=" : "";
            var sql = string.Format(@"
            		SELECT id, left_id, right_id, target_id 
            		FROM [{0}].[{1}] n 
            		WHERE n.left_id <{3} @ChildLeft 
            			AND n.right_id >{3} @ChildRight
            		ORDER BY n.left_id {2}", hierarchyEntry.SchemaName, hierarchyEntry.TableName, orderString,
                includeChildString);

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@ChildLeft", child.LeftId);
                    command.Parameters.AddWithValue("@ChildRight", child.RightId);

                    conn.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ancestors.Add(PopulateHierarchyNode(reader));
                        }
                    }
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return ancestors;
        }

        public bool HierarchyInitialized(HierarchyEntry hierarchyEntry)
        {
            if (_hierarchyInitialized.ContainsKey(hierarchyEntry.Id))
            {
                return true;
            }
            var verified = true;
            
            if (!_dataProvider.TableExists(hierarchyEntry.SchemaName, hierarchyEntry.TableName))
            {
                var createSql = string.Format(@"CREATE TABLE [{0}].[{1}](
	                [id] [int] IDENTITY(1,1) NOT NULL,
	                [left_id] [int] NOT NULL,
                    [right_id] [int] NOT NULL,
	                [target_id] [int] NOT NULL,
                    CONSTRAINT [PK_{0}_{1}] PRIMARY KEY CLUSTERED 
                        ([id] ASC)
                    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]", hierarchyEntry.SchemaName, hierarchyEntry.TableName);
                _dataProvider.ExecuteSql(createSql);
                verified = _dataProvider.TableExists(hierarchyEntry.SchemaName, hierarchyEntry.TableName);
            }
            _hierarchyInitialized.Add(hierarchyEntry.Id, hierarchyEntry.TableName);
            return verified;
        }

        public int Add(HierarchyEntry hierarchyEntry, HierarchyNode node)
        {
            if (!HierarchyInitialized(hierarchyEntry)) return 0;


            var insertQuery = node.Id == 0
                ? string.Format(@"INSERT INTO [{0}].[{1}] (target_id, left_id, right_id) OUTPUT inserted.id VALUES (@TargetId, @LeftId, @RightId)", hierarchyEntry.SchemaName, hierarchyEntry.TableName)
                : string.Format(@"UPDATE [{0}].[{1}] SET target_id=@TargetId, left_id=@LeftId, right_id=@RightId WHERE id=@Id", hierarchyEntry.SchemaName, hierarchyEntry.TableName);
            
            int result = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(insertQuery, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("@LeftId", node.LeftId);
                    command.Parameters.AddWithValue("@RightId", node.RightId);
                    command.Parameters.AddWithValue("@TargetId", node.TargetId);
                    if (node.Id == 0)
                    {
                        var rawResult = command.ExecuteScalar();
                        if (rawResult != null)
                        {
                            result = rawResult.CastTo<int>();
                        }
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Id", node.Id);
                        var rowsAffected = command.ExecuteNonQuery();
                        result = rowsAffected == 0 ? 0 : node.Id;
                    }
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return result;
        }

        public bool Delete(HierarchyEntry hierarchy, HierarchyNode node)
        {
            // can only delete leaf nodes - let consumer handle errors.
            if (node.RightId != node.LeftId + 1) return false;

            var deleteSql = string.Format(@"
                    DELETE FROM [{0}].[{1}] WHERE id = @NodeId ", 
                hierarchy.SchemaName,
                hierarchy.TableName);

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(deleteSql, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("@NodeId", node.Id);
                    command.ExecuteNonQuery();

                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }            
            HandleGap(hierarchy, node, false);
            return true;
        }

        #endregion

    }
}
