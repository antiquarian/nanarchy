﻿using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Nanarchy.Core;
using Nanarchy.Core.Interfaces;
using UsefulUtilities;

namespace Nanarchy.Data.Mssql
{
    public class TargetEntryRepository : Repository<TargetEntry>, ITargetEntryRepository
    {
 
        public TargetEntryRepository(IDataProvider dataProvider) : base(dataProvider) { }

        public override void Initialize()
        {
            SchemaName = ConfigurationManager.AppSettings["NDB.SchemaName"];
            TableName = ConfigurationManager.AppSettings["NDB.TargetEntryTableName"];

            if (!TableExists())
            {
                var createSql = string.Format(@"CREATE TABLE [{0}].[{1}](
	                [id] [int] IDENTITY(1,1) NOT NULL,
	                [name] [nvarchar](50) NOT NULL,
                    [schema_name] nvarchar(100) NOT NULL,
	                [table_name] [nvarchar](100) NOT NULL,
                    CONSTRAINT [PK_{1}] PRIMARY KEY CLUSTERED 
                        ([id] ASC)
                    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]", SchemaName, TableName);
                DataProvider.ExecuteSql(createSql);
            }

        }
        #region Target Methods
        public override TargetEntry Get(int id)
        {
            TargetEntry target = null;
            var sql = string.Format("SELECT id, name, schema_name, table_name FROM [{0}].[{1}] WHERE id = @Id", SchemaName, TableName);
            var returnValue = DataProvider.Get(sql, id, PopulateMethod);
            if (returnValue != null)
            {
                target = returnValue.CastTo<TargetEntry>();
            }
            return target;
        }


        public override TargetEntry PopulateMethod(IDataRecord reader)
        {
            var target = new TargetEntry
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                SchemaName = reader.GetString(reader.GetOrdinal("schema_name")),
                TableName = reader.GetString(reader.GetOrdinal("table_name"))
            };

            return target;
        }


        public override int Update(TargetEntry target)
        {
            var sql = target.Id == 0
                ? string.Format("INSERT INTO [{0}].[{1}] (name, schema_name, table_name) OUTPUT inserted.id VALUES (@Name,@SchemaName,@TableName)", SchemaName, TableName)
                : string.Format("UPDATE [{0}].[{1}] SET name=@Name,schema_name=@SchemaName, table_name=@TableName WHERE id=@Id", SchemaName, TableName);

            var parameterValues = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("@Id", target.Id),
                new KeyValuePair<string, object>("@Name", target.Name),
                new KeyValuePair<string, object>("@SchemaName", target.SchemaName),
                new KeyValuePair<string, object>("@TableName", target.TableName)
            };

            return DataProvider.Update(sql, target.Id, parameterValues);
        }

        #endregion
    }
}