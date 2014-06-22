using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using NUnit.Framework;
using Vts.Common;

namespace Vts.Test.Common
{
    [TestFixture]
    public class IntRangeTests
    {

        [Test]
        public void validate_class_is_serializable()
        {
            Assert.IsNotNull(Clone(new IntRange()));
        }

        [Test]
        public void validate_IntRange_constructor_assigns_correct_values()
        {
            var r = new IntRange(0, 9, 10);

            Assert.AreEqual(r.Start, 0);
            Assert.AreEqual(r.Stop, 9);
            Assert.AreEqual(r.Delta, 1);
            Assert.AreEqual(r.Count, 10);
        }
        
        [Test]
        public void validate_IntRange_default_constructor_assigns_correct_values()
        {
            var r = new IntRange();

            Assert.AreEqual(r.Start, 0);
            Assert.AreEqual(r.Stop, 1);
            Assert.AreEqual(r.Delta, 1);
            Assert.AreEqual(r.Count, 2);
        }

        [Test]
        public void validate_deserialized_class_is_correct()
        {
            var r = new IntRange(0, 9, 10);

            var deserializedR = Clone(r);

            Assert.IsNotNull(deserializedR);

            Assert.AreEqual(deserializedR.Start, 0);
            Assert.AreEqual(deserializedR.Stop, 9);
            Assert.AreEqual(deserializedR.Delta, 1);
            Assert.AreEqual(deserializedR.Count, 10);
        }

        [Test]
        public void validate_single_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new IntRange(10, 10, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.AreEqual(delta, 0);
            Assert.AreEqual(count, 1);
        }

        [Test]
        public void validate_single_count_returns_correct_delta_for_mismatched_start_stop()
        {
            var r = new IntRange(10, 20, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.AreEqual(delta, 10);
            Assert.AreEqual(count, 1);
        }

        [Test]
        public void validate_multi_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new IntRange(10, 10, 3);

            var count = r.Count;
            var delta = r.Delta;

            Assert.AreEqual(count, 3);
            Assert.AreEqual(delta, 0);
        }

        [Test]
        public void validate_multi_count_returns_multiple_values_for_matched_start_stop()
        {
            var r = new IntRange(10, 10, 3);

            var values = r.AsEnumerable().ToArray();

            Assert.AreEqual(values.Length, 3);
            for (int i = 0; i < values.Length; i++)
            {
                Assert.AreEqual(values[i], 10);
            }
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
