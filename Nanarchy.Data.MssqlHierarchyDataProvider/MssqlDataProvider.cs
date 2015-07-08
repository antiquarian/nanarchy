using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nanarchy.Service;

namespace Nanarchy.Data.MssqlHierarchyDataProvider
{
    public class MssqlDataProvider : IDataProvider
    {
        private readonly string _connectionString;

        public MssqlDataProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Hierarchy Methods

        public Hierarchy GetHierarchy(int id)
        {
            Hierarchy target = null;
            var sql = "SELECT id, name, table_name FROM [Hierarchy] WHERE id = @Id";
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        target = PopulateHierarchy(reader);
                    }
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            // returns NULL if not found
            return target;
        }

        public bool DeleteHierarchy(int id)
        {
            var result = 0;
            var sql = "DELETE FROM [Hierarchy] WHERE id = @Id";
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    result = command.ExecuteNonQuery();
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return result == 1;
        }

        private static Hierarchy PopulateHierarchy(IDataRecord reader)
        {
            var target = new Hierarchy
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                TableName = reader.GetString(reader.GetOrdinal("table_name"))
            };

            return target;
        }

        public int UpdateHierarchy(Hierarchy hierarchy)
        {
            var result = 0;
            var sql = hierarchy.Id == 0
                ? "INSERT INTO [Hierarchy] (name, table_name) OUTPUT inserted.id VALUES (@Name,@TableName)" 
                : "UPDATE [Hierarchy] SET name=@Name, table_name=@TableName WHERE id=@Id";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@Id", hierarchy.Id);
                    command.Parameters.AddWithValue("@Name", hierarchy.Name);
                    command.Parameters.AddWithValue("@TableName", hierarchy.TableName);
                    conn.Open();
                    if (hierarchy.Id == 0)
                    {
                        result = (int) command.ExecuteScalar();
                    }
                    else
                    {
                        var rowsAffected = command.ExecuteNonQuery();
                        result = rowsAffected == 0 ? 0 : hierarchy.Id;
                    }
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return result;
        }

        #endregion

        #region Target Methods
        public Target GetTarget(int id)
        {
            Target target = null;
            var sql = "SELECT id, name, table_name FROM [Target] WHERE id = @Id";
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        target = PopulateTarget(reader);
                    }
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            // returns NULL if not found
            return target;
        }

        public bool DeleteTarget(int id)
        {
            var result = 0;
            var sql = "DELETE FROM [Target] WHERE id = @Id";
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    result = command.ExecuteNonQuery();
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return result == 1;
        }

        private static Target PopulateTarget(IDataRecord reader)
        {
            var target = new Target
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                TableName = reader.GetString(reader.GetOrdinal("table_name"))
            };

            return target;
        }

        public int UpdateTarget(Target target)
        {
            var result = 0;
            var sql = target.Id == 0
                ? "INSERT INTO [Target] (name, table_name) OUTPUT inserted.id VALUES (@Name,@TableName)"
                : "UPDATE [Target] SET name=@Name, table_name=@TableName WHERE id=@Id";

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@Id", target.Id);
                    command.Parameters.AddWithValue("@Name", target.Name);
                    command.Parameters.AddWithValue("@TableName", target.TableName);
                    conn.Open();
                    if (target.Id == 0)
                    {
                        result = (int)command.ExecuteScalar();
                    }
                    else
                    {
                        var rowsAffected = command.ExecuteNonQuery();
                        result = rowsAffected == 0 ? 0 : target.Id;
                    }
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return result;
        }

        #endregion

        public int ExecuteSql(string sql)
        {
            int result = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(sql, connection);
                connection.Open();
                result = command.ExecuteNonQuery();
            }
            return result;
        }

        public bool InitializeDatabase(Hierarchy hierarchy, List<Target> nodeTargets)
        {
            var check = VerifyManagementTables();
            if (!check) return false;

            UpdateHierarchy(hierarchy);
            foreach (var nodeTarget in nodeTargets)
            {
                UpdateTarget(nodeTarget);
            }
            return true;
        }

        private bool VerifyManagementTables()
        {
            var checkSql = @"SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Hierarchy'";
            var hierarchyTableExists = ExecuteSql(checkSql);
            if (hierarchyTableExists == 0)
            {
                var createHierarchyTableSql = @"CREATE TABLE [dbo].[Hierarchy](
	                [id] [int] IDENTITY(1,1) NOT NULL,
	                [hierarchy_name] [nvarchar](50) NOT NULL,
	                [table_name] [nvarchar](100) NOT NULL,
                    CONSTRAINT [PK_Hierarchy] PRIMARY KEY CLUSTERED 
                        ([id] ASC)
                    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]";
                ExecuteSql(createHierarchyTableSql);
                hierarchyTableExists = ExecuteSql(checkSql);
                if (hierarchyTableExists == 0) return false;
            }

            checkSql = @"SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Target'";
            var targetTableExists = ExecuteSql(checkSql);
            if (targetTableExists == 0)
            {
                var createTargetTableSql = @"CREATE TABLE [dbo].[Target](
	                [id] [int] IDENTITY(1,1) NOT NULL,
	                [target_name] [nvarchar](50) NOT NULL,
	                [table_name] [nvarchar](100) NOT NULL,
                    CONSTRAINT [PK_Hierarchy] PRIMARY KEY CLUSTERED 
                        ([id] ASC)
                    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]";
                ExecuteSql(createTargetTableSql);
                targetTableExists = ExecuteSql(checkSql);
                if (targetTableExists == 0) return false;
            }
            return true;
        }

    }
}