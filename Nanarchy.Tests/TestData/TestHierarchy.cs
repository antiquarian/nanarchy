using System;
using Nanarchy.Core;
using Nanarchy.Core.Interfaces;

namespace Nanarchy.Tests.TestData
{
    public class TestTarget : ITarget
    {
        public int Id { get; set; }
        public Guid GlobalIdentifier { get; set; }
        public string Name { get; set; }
    }
}