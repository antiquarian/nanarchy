using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Nanarchy.Core;
using Nanarchy.Core.Helpers;
using Nanarchy.Core.Interfaces;

namespace Nanarchy.Data.Mssql
{
    public interface ITargetRepository 
    {
        void Initialize(string tableName);
        ITarget Get<TTarget>(string tableName, int id) where TTarget : ITarget, new();
        bool Delete(ITarget target);
        int Update(ITarget target);
    }
    public class TargetRepository : ITargetRepository
    {
        private bool _isInitialized;

        private bool IsInitialized(string tableName)
        {
            if (!_isInitialized)
            {
                Initialize(tableName);
                _isInitialized = true;
            }
            return _isInitialized;
        }
        private readonly IDataProvider _dataProvider;
        protected string SchemaName;
        protected string DatabaseName;

        public TargetRepository(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            SchemaName = ConfigurationManager.AppSettings["NDB.SchemaName"];
            DatabaseName = ConfigurationManager.AppSettings["NDB.DatabaseName"];
        }

        
        public void Initialize(string tableName)
        {
            if (!_dataProvider.TableExists(SchemaName, tableName))
            {
                var createSql = string.Format(@"CREATE TABLE [{0}].[{1}](
	                [id] [int] IDENTITY(1,1) NOT NULL,
	                [global_identifier] [uniqueidentifier] NOT NULL,
                    [created_date] [datetime] NOT NULL,
	                [last_modified_date] [datetime] NOT NULL,
                    [target_data] [nvarchar] (MAX) NOT NULL,
                    CONSTRAINT [PK_{1}] PRIMARY KEY CLUSTERED 
                        ([id] ASC)
                    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]", SchemaName, tableName);
                _dataProvider.ExecuteSql(createSql);
            }
        }

        public ITarget Get<TTarget>(string tableName, int id) where TTarget : ITarget, new()
        {
            var target = new TTarget();
            var sql = string.Format("SELECT id, global_identifier, created_date, last_modified_date, target_data FROM [{0}].[{1}] WHERE id = @Id", SchemaName, tableName);

            using (var conn = new SqlConnection(_dataProvider.ConnectionString))
            {
                using (var command = new SqlCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        PopulateMethod(target, reader);
                    }
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }

            return target;
        }

        public bool Delete(ITarget target)
        {
            if (target.Id == 0) return false;
            return _dataProvider.Delete(SchemaName, target.TableName, target.Id);
        }

        public int Update(ITarget target)
        {
            if (!IsInitialized(target.TableName)) return 0;

            var currentDateTime = DateTime.UtcNow;
            var tableName = target.TableName;

            var sql = string.Empty;
            var parameterValues = new List<KeyValuePair<string, object>>();
            //var settings = new DataContractJsonSerializerSettings();
            
            var targetData = Json.Serialize(target.Data, true);

            if (target.Id == 0)
            {

                var globalIdentifier = Guid.NewGuid();
                sql = string.Format(
                    "INSERT INTO [{0}].[{1}] (global_identifier, created_date, last_modified_date, target_data) OUTPUT inserted.id VALUES (@GlobalIdentifier,@CurrentDateTime,@CurrentDateTime,@TargetData)",
                    SchemaName, tableName);
                parameterValues.Add(new KeyValuePair<string, object>("@GlobalIdentifier", globalIdentifier));
                parameterValues.Add(new KeyValuePair<string, object>("@CurrentDateTime", currentDateTime));
                parameterValues.Add(new KeyValuePair<string, object>("@TargetData", targetData));
            }
            else
            {
                sql = string.Format(
                    "UPDATE [{0}].[{1}] SET last_modified_date=@CurrentDateTime,target_data=@TargetData WHERE id=@Id",
                    SchemaName, tableName);
                parameterValues.Add(new KeyValuePair<string, object>("@Id", target.Id));
                parameterValues.Add(new KeyValuePair<string, object>("@CurrentDateTime", currentDateTime));
                parameterValues.Add(new KeyValuePair<string, object>("@TargetData", targetData));
            }


            return _dataProvider.Update(sql, target.Id, parameterValues);
        }

        private static void PopulateMethod(ITarget target, IDataRecord reader)
        {
            target.Id = reader.GetInt32(reader.GetOrdinal("id"));
            target.GlobalIdentifier = reader.GetGuid(reader.GetOrdinal("global_identifier"));
            target.CreatedDate = reader.GetDateTime(reader.GetOrdinal("created_date"));
            target.LastModifiedDate = reader.GetDateTime(reader.GetOrdinal("last_modified_date"));
 
            var targetDataJson = reader.GetString(reader.GetOrdinal("target_data"));
            //var targetDataType = target.Data.GetType();
            var targetData = Json.Deserialize(targetDataJson, typeof(ITargetData), true);
            target.Data = (ITargetData)targetData;
        }
    }
}