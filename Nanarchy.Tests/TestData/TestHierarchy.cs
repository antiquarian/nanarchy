using System;
using Nanarchy.Core;
using Nanarchy.Core.Interfaces;

namespace Nanarchy.Tests.TestData
{
    public class TestHierarchyEntry : HierarchyEntry
    {
        public TestHierarchyEntry(string name) : this(name, name) { }

        public TestHierarchyEntry(string name, string tableName)
        {
            Name = name;
            TableName = tableName;
        }
        public string Name { get; set; }
        public string TableName { get; set; }
        public int Id { get; set; }
        public Guid GlobalIdentifier { get; set; }
    }

    public class TestTarget : ITarget
    {
        public int Id { get; set; }
        public Guid GlobalIdentifier { get; set; }
        public string Name { get; set; }
    }
}