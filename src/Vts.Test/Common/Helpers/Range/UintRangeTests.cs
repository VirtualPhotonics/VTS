﻿using System.IO;
using System.Runtime.Serialization;
using NUnit.Framework;
using Vts.Common;

namespace Vts.Test.Common
{
    [TestFixture]
    public class UintRangeTests
    {
        [Test]
        public void validate_parameterized_constructor_assigns_correct_values()
        {
            var r = new UintRange(0, 9, 10);

            Assert.AreEqual(r.Start, 0U);
            Assert.AreEqual(r.Stop, 9U);
            Assert.AreEqual(r.Delta, 1U);
            Assert.AreEqual(r.Count, 10);
        }

        [Test]
        public void validate_default_constructor_assigns_correct_values()
        {
            var r = new UintRange();

            Assert.AreEqual(r.Start, 0U);
            Assert.AreEqual(r.Stop, 1U);
            Assert.AreEqual(r.Delta, 1U);
            Assert.AreEqual(r.Count, 2);
        }

        [Test]
        public void validate_class_is_serializable()
        {
            Assert.IsNotNull(Clone(new UintRange()));
        }

        [Test]
        public void validate_deserialized_class_is_correct()
        {
            var r = new UintRange(0U, 9U, 10);

            var deserializedR = Clone(r);

            Assert.IsNotNull(deserializedR);

            Assert.AreEqual(deserializedR.Start, 0U);
            Assert.AreEqual(deserializedR.Stop, 9U);
            Assert.AreEqual(deserializedR.Delta, 1U);
            Assert.AreEqual(deserializedR.Count, 10);
        }

        private static T Clone<T>(T myObject)
        {
            using (MemoryStream ms = new MemoryStream(1024))
            {
                var dcs = new DataContractSerializer(typeof(T));
                dcs.WriteObject(ms, myObject);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)dcs.ReadObject(ms);
            }
        }
    }
}
