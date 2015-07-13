using System;
using System.Collections.Generic;
using System.Data;

namespace Nanarchy.Core.Interfaces
{
    public interface IDataProvider
    {
        string ConnectionString { get; }
        int ExecuteSql(string sql);
        bool TableExists(string schemaName, string tableName);
        bool DropTable(string schemaName, string tableName);
        bool Delete(string schemaName, string tableName, int id);

        int Update(string sql, int id,IEnumerable<KeyValuePair<string, object>> parameterValues);
        T Get<T>(string sql, int id, Func<IDataRecord, T> populateMethod);
    }
}