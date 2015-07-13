using System;
using Nanarchy.Core.Interfaces;

namespace Nanarchy.Tests.TestData
{
    public class TestTargetOne : ITarget
    {
        public int Id { get; set; }
        public Guid GlobalIdentifier { get; set; }
        public string TestString { get; set; }
        public DateTime TestDate { get; set; }
        public decimal TestDecimal { get; set; }
        public int TestInt { get; set; }
        public bool TestBool { get; set; }
    }
}