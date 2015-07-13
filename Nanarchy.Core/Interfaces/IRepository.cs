using System.Data;

namespace Nanarchy.Core.Interfaces
{
    public interface IRepository<T>
    {
        void Initialize();
        bool Delete(int id);
        T Get(int id);
        int Update(T item);
        bool TableExists();
        T PopulateMethod(IDataRecord reader);
    }

}