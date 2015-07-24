using System;
using System.Runtime.Serialization;
using Nanarchy.Core;
using Nanarchy.Core.Interfaces;

namespace Nanarchy.Tests.TestData
{
    public class TestTarget : Target
    {
        public override ITargetData Data { get; set; }
    }
    [DataContract]
    public class TestTargetData : ITargetData
    {
        [DataMember]
        public string TestString { get; set; }
        [DataMember]
        public DateTime TestDate { get; set; }
        [DataMember]
        public decimal TestDecimal { get; set; }
        [DataMember]
        public int TestInt { get; set; }
        [DataMember]
        public bool TestBool { get; set; }
    }
}