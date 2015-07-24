using System.Collections.Generic;
using System.Configuration;

namespace Nanarchy.Core.Configuration
{
    public class HierarchyConfigSection : ConfigurationSection
    {
        private static readonly HierarchyConfigSection section = ConfigurationManager.GetSection("HierarchyConfiguration") as HierarchyConfigSection;
        public static HierarchyConfigSection Section
        {
            get { return section; }
        }

        [ConfigurationProperty("hierarchies", IsDefaultCollection = false)]
        public HierarchyConfigCollection QueueCollection
        {
            get
            {
                var queueCollection = (HierarchyConfigCollection)base["hierarchies"];
                return queueCollection;
            }
        }

        protected override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
        {
            var s = base.SerializeSection(parentElement, name, saveMode);
            return s;
        }

        public IEnumerable<HierarchyEntry> Hierarchies
        {
            get
            {
                for (var i = 0; i < QueueCollection.Count; i++)
                {
                    yield return
                        new HierarchyEntry
                        {
                            Name = QueueCollection[i].Name,
                        };
                }
            }
        }
    }
}