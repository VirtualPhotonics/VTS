using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using NUnit.Framework;
using Vts.Common;

namespace Vts.Test.Common
{
    [TestFixture]
    public class FloatRangeTests
    {
        [Test]
        public void validate_FloatRange_constructor_assigns_correct_values()
        {
            var r = new FloatRange(0f, 9f, 10);

            Assert.AreEqual(r.Start, 0f);
            Assert.AreEqual(r.Stop, 9f);
            Assert.AreEqual(r.Delta, 1f);
            Assert.AreEqual(r.Count, 10);
        }

        [Test]
        public void validate_FloatRange_default_constructor_assigns_correct_values()
        {
            var r = new FloatRange();

            Assert.AreEqual(r.Start, 0f);
            Assert.AreEqual(r.Stop, 1f);
            Assert.AreEqual(r.Delta, 1f);
            Assert.AreEqual(r.Count, 2);
        }

        [Test]
        public void validate_class_is_serializable()
        {
            Assert.IsNotNull(Clone(new FloatRange()));
        }

        [Test]
        public void validate_deserialized_class_is_correct()
        {
            var r = new FloatRange(0f, 9f, 10);

            var deserializedR = Clone(r);

            Assert.IsNotNull(deserializedR);

            Assert.AreEqual(deserializedR.Start, 0f);
            Assert.AreEqual(deserializedR.Stop, 9f);
            Assert.AreEqual(deserializedR.Delta, 1f);
            Assert.AreEqual(deserializedR.Count, 10);
        }

        [Test]
        public void validate_single_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new FloatRange(10f, 10f, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.AreEqual(delta, 0f);
            Assert.AreEqual(count, 1);
        }

        [Test]
        public void validate_single_count_returns_correct_delta_for_mismatched_start_stop()
        {
            var r = new FloatRange(10f, 20f, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.AreEqual(delta, 10f);
            Assert.AreEqual(count, 1);
        }

        [Test]
        public void validate_multi_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new FloatRange(10f, 10f, 3);

            var count = r.Count;
            var delta = r.Delta;

            Assert.AreEqual(count, 3);
            Assert.AreEqual(delta, 0f);
        }

        [Test]
        public void validate_multi_count_returns_multiple_values_for_matched_start_stop()
        {
            var r = new FloatRange(10f, 10f, 3);

            var values = r.AsEnumerable().ToArray();

            Assert.AreEqual(values.Length, 3);
            for (int i = 0; i < values.Length; i++)
            {
                Assert.AreEqual(values[i], 10f);
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
