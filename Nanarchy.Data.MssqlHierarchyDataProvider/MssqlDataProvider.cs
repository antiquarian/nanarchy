using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nanarchy.Core.Interfaces;

namespace Nanarchy.Data.MssqlHierarchyDataProvider
{
    public class MssqlDataProvider : IDataProvider
    {
        #region Public Members

        public string ConnectionString
        {
            get {  return _connectionString; }
        }

        #endregion

        private readonly string _connectionString;

        public MssqlDataProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Database Methods

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

        public bool DropTable(string schemaName, string tableName)
        {
            var result = 0;
            var sql = string.Format("DROP TABLE [{0}].[{1}]", schemaName, tableName);
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    try
                    {
                        result = command.ExecuteNonQuery();
                    }
                    catch
                    {
                        // only fails if table was missing, so no need to drop. Silly developers.
                    }
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return result == 1;
        }

        public bool Delete(string schemaName, string tableName, int id)
        {
            var result = 0;
            var sql = string.Format("DELETE FROM [{0}].[{1}] WHERE id = {2}", schemaName, tableName, id);
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    result = command.ExecuteNonQuery();
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return result == 1;
        }

        public int Update(string sql, int id, IEnumerable<KeyValuePair<string, object>> parameterValues)
        {
            var result = 0;
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {

                    foreach (var parameterValue in parameterValues)
                    {
                        command.Parameters.AddWithValue(parameterValue.Key, parameterValue.Value);
                    }
                    conn.Open();
                    if (id == 0)
                    {
                        result = (int)command.ExecuteScalar();
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        var rowsAffected = command.ExecuteNonQuery();
                        result = rowsAffected == 0 ? 0 : id;
                    }
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }

            return result;
        }

        public T Get<T>(string sql, int id, Func<IDataRecord, T> populateMethod)
        {
            T target = default(T);
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
                        target = populateMethod(reader);
                    }
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            // returns NULL if not found
            return target;
        }


        public bool TableExists(string schemaName, string tableName)
        {
            var checkSql = @"SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = @SchemaName 
                 AND  TABLE_NAME = @TableName";
            var tableExists = false;
            using (var conn = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(checkSql, conn))
                {
                    command.Parameters.AddWithValue("@SchemaName", schemaName);
                    command.Parameters.AddWithValue("@TableName", tableName);
                    conn.Open();
                    var reader = command.ExecuteReader();
                    tableExists = reader.HasRows;
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
            return tableExists;
        }

        #endregion

        #region Private Methods


        #endregion
    }
}