using System.Configuration;

namespace Nanarchy.Core.Configuration
{
    public class HierarchyConfigElement : ConfigurationElement
    {
        public HierarchyConfigElement(string elementName)
        {
            Name = elementName;
        }

        public HierarchyConfigElement()
        {

        }
        [ConfigurationProperty("name", DefaultValue = "Sample", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
    }
}