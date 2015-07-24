using System.Configuration;

namespace Nanarchy.Core.Configuration
{
    public class HierarchyConfigCollection : ConfigurationElementCollection
    {
        public new string AddElementName
        {
            get { return base.AddElementName; }

            set { base.AddElementName = value; }
        }

        public new string ClearElementName
        {
            get { return base.ClearElementName; }

            set { base.ClearElementName = value; }
        }

        public new string RemoveElementName
        {
            get { return base.RemoveElementName; }
        }

        public new int Count
        {
            get { return base.Count; }
        }

        public HierarchyConfigElement this[int index]
        {
            get { return (HierarchyConfigElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public new HierarchyConfigElement this[string name]
        {
            get { return (HierarchyConfigElement)BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new HierarchyConfigElement();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new HierarchyConfigElement(elementName);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((HierarchyConfigElement)element).Name;
        }

        public int IndexOf(HierarchyConfigElement element)
        {
            return BaseIndexOf(element);
        }

        public void Add(HierarchyConfigElement element)
        {
            BaseAdd(element);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(HierarchyConfigElement element)
        {
            if (BaseIndexOf(element) >= 0)
                BaseRemove(element.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }
    }

}