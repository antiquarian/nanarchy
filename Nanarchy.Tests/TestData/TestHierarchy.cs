using Dell.Hierarchy.Service;

namespace Dell.Hierarchy.Tests.TestData
{
    public class TestHierarchy : IHierarchy
    {
        public TestHierarchy(string name) : this(name, name) { }

        public TestHierarchy(string name, string tableName)
        {
            Name = name;
            TableName = tableName;
        }
        public string Name { get; set; }
        public string TableName { get; set; }
    }

    public class TestTarget : INodeTarget
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}