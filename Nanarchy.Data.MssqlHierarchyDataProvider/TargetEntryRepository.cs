using System;
using System.Collections.Generic;
using System.Data;
using Nanarchy.Core;
using Nanarchy.Core.Interfaces;
using UsefulUtilities;

namespace Nanarchy.Data.MssqlHierarchyDataProvider
{
    public class TargetEntryRepository : Repository<TargetEntry>, ITargetEntryRepository
    {
        public TargetEntryRepository(IDataProvider dataProvider) : base(dataProvider)
        {
            SchemaName = "dbo";
            TableName = "TargetEntry";
        }
        public override void Initialize()
        {
            throw new NotImplementedException();
        }
        #region Target Methods
        public override TargetEntry Get(int id)
        {
            TargetEntry target = null;
            var sql = string.Format("SELECT id, name, table_name FROM [{0}].[{1}] WHERE id = @Id", SchemaName, TableName);
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
                TableName = reader.GetString(reader.GetOrdinal("table_name"))
            };

            return target;
        }


        public override int Update(TargetEntry target)
        {
            var sql = target.Id == 0
                ? string.Format("INSERT INTO [{0}].[{1}] (name, table_name) OUTPUT inserted.id VALUES (@Name,@TableName)", SchemaName, TableName)
                : string.Format("UPDATE [{0}].[{1}] SET name=@Name, table_name=@TableName WHERE id=@Id", SchemaName, TableName);

            var parameterValues = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("@Id", target.Id),
                new KeyValuePair<string, object>("@Name", target.Name),
                new KeyValuePair<string, object>("@TableName", target.TableName)
            };

            return DataProvider.Update(sql, target.Id, parameterValues);
        }

        #endregion
    }
}