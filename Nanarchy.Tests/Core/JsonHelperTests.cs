using System;
using Nanarchy.Core.Helpers;
using Nanarchy.Tests.TestData;
using NUnit.Framework;
using UsefulUtilities;

namespace Nanarchy.Tests.Core
{
    [TestFixture]
    public class JsonHelperTests
    {
        [Test]
        public void Should_properly_serialize_any_object()
        {
            var baseDate = DateTime.UtcNow;
            var obj = new TestTargetData
            {
                TestString = "String",
                TestDate = baseDate,
                TestBool = true,
                TestDecimal = 123.45m,
                TestInt = 678
            };

            var serialized = Json.Serialize(obj);

            Assert.That(serialized.Length, Is.GreaterThan(5));

            var deserializedObj = Json.Deserialize(serialized, typeof (TestTargetData));
            var deserialized = deserializedObj.CastTo<TestTargetData>();

            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized.TestString, Is.EqualTo("String"));
            Assert.That(deserialized.TestBool, Is.True);
            Assert.That(deserialized.TestDate, Is.EqualTo(baseDate));
            Assert.That(deserialized.TestDecimal, Is.EqualTo(123.45m));
            Assert.That(deserialized.TestInt, Is.EqualTo(678));
        }
    }
}