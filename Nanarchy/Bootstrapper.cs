using System.Collections.Generic;
using System.Reflection;
using Nanarchy.Core;
using Nanarchy.Data.Mssql;
using Nanarchy.Service;
using UsefulUtilities;

namespace Nanarchy
{
    /// <summary>
    /// Only used for old school NEW type initialization
    /// </summary>
    public static class Bootstrapper
    {
        public static void Initialize()
        {
            // verify tables
            var dataProvider = new MssqlDataProvider();
            var hierarchyDataProvider = new MssqlHierarchyDataProvider(dataProvider);
            
            var hierarchyEntryRepository = new HierarchyEntryRepository(dataProvider);
            var targetEntryRepository = new TargetEntryRepository(dataProvider);
            var targetRepository = new TargetRepository(dataProvider);
            

            var assembly = Assembly.GetCallingAssembly();
            var targetTypes = assembly.GetTypesOfType<ITarget>();

            var hierarchyService = new HierarchyService(hierarchyDataProvider, hierarchyEntryRepository, targetEntryRepository, targetRepository, targetTypes);

            

            // build missing tables



            // register structureMap
        }
    }
}