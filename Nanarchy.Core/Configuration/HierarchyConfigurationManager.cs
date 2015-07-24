using System.Collections.Generic;

namespace Nanarchy.Core.Configuration
{
    public static class HierarchyConfigurationManager
    {
        public static IEnumerable<HierarchyEntry> Items
        {
            get { return HierarchyConfigSection.Section.Hierarchies; }
        }
    }
}