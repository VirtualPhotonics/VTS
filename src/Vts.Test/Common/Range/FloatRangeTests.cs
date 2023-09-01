using NUnit.Framework;
using System.Linq;
using Vts.Common;
using Vts.IO;

namespace Vts.Test.Common
{
    [TestFixture]
    public class FloatRangeTests
    {
        [Test]
        public void Validate_FloatRange_constructor_assigns_correct_values()
        {
            var r = new FloatRange(0f, 9f, 10);

            Assert.AreEqual(0f, r.Start);
            Assert.AreEqual(9f, r.Stop);
            Assert.AreEqual(1f, r.Delta);
            Assert.AreEqual(10, r.Count);
        }

        [Test]
        public void Validate_FloatRange_default_constructor_assigns_correct_values()
        {
            var r = new FloatRange();

            Assert.AreEqual(0f, r.Start);
            Assert.AreEqual(1f, r.Stop);
            Assert.AreEqual(1f, r.Delta);
            Assert.AreEqual(2, r.Count);
        }

        [Test]
        public void Validate_class_is_serializable()
        {
            Assert.IsInstanceOf<FloatRange>(new FloatRange().Clone<FloatRange>());
        }

        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var r = new FloatRange(0f, 9f, 10);

            var deserializedR = r.Clone<FloatRange>();

            Assert.IsNotNull(deserializedR);

            Assert.AreEqual(0f, deserializedR.Start);
            Assert.AreEqual(9f, deserializedR.Stop);
            Assert.AreEqual(1f, deserializedR.Delta);
            Assert.AreEqual(10, deserializedR.Count);
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new FloatRange(10f, 10f, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.AreEqual(0f, delta);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_mismatched_start_stop()
        {
            var r = new FloatRange(10f, 20f, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.AreEqual(10f, delta);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Validate_multi_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new FloatRange(10f, 10f, 3);

            var count = r.Count;
            var delta = r.Delta;

            Assert.AreEqual(3, count);
            Assert.AreEqual(0f, delta);
        }

        [Test]
        public void Validate_multi_count_returns_multiple_values_for_matched_start_stop()
        {
            var r = new FloatRange(10f, 10f, 3);

            var values = r.ToArray();

            Assert.AreEqual(3, values.Length);
            foreach (var value in values)
            {
                Assert.AreEqual(10f, value);
            }
        }

        [Test]
        public void Test_clone()
        {
            var floatRange = new FloatRange(0f, 10f);
            Assert.IsInstanceOf<FloatRange>(floatRange.Clone());
        }

        [Test]
        public void Test_ToString()
        {
            var floatRange = new FloatRange(0f, 9f, 10);
            Assert.AreEqual("Start: 0, Stop: 9, Count: 10, Delta: 1", floatRange.ToString());
        }
    }
}
