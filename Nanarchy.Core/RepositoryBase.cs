using System.Data;
using Nanarchy.Core.Interfaces;

namespace Nanarchy.Core
{
    public abstract class Repository<T> : IRepository<T>
    {
        protected readonly IDataProvider DataProvider;

        protected Repository(IDataProvider dataProvider)
        {
            DataProvider = dataProvider;
        }

        protected string SchemaName { get; set; }
        protected string TableName { get; set; }

        public abstract void Initialize();
        public abstract T Get(int id);
        public abstract int Update(T item);
        public abstract T PopulateMethod(IDataRecord reader);

        public bool Delete(int id)
        {
            if (string.IsNullOrWhiteSpace(SchemaName) || string.IsNullOrWhiteSpace(TableName)) return false;
            return DataProvider.Delete(SchemaName, TableName, id);
        }
        public bool TableExists()
        {
            if (string.IsNullOrWhiteSpace(SchemaName) || string.IsNullOrWhiteSpace(TableName)) return false;
            return DataProvider.TableExists(SchemaName, TableName);
        }
    }
}