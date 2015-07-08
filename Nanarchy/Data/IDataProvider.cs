using System.Collections.Generic;
using Nanarchy.Service;

namespace Nanarchy.Data
{
    public interface IDataProvider
    {
        Hierarchy GetHierarchy(int id);
        int UpdateHierarchy(Hierarchy hierarchy);
        Target GetTarget(int id);
        int UpdateTarget(Target target); 
        int ExecuteSql(string sql);
        bool InitializeDatabase(Hierarchy hierarchy, List<Target> nodeTargets);
        bool DeleteHierarchy(int id);
    }
}