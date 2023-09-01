using NUnit.Framework;
using System.Linq;
using Vts.Common;
using Vts.IO;

namespace Vts.Test.Common
{
    [TestFixture]
    public class LongRangeTests
    {

        [Test]
        public void Validate_LongRange_constructor_assigns_correct_values()
        {
            var r = new LongRange(0, 9, 10);

            Assert.AreEqual(0L, r.Start);
            Assert.AreEqual(9L, r.Stop);
            Assert.AreEqual(1L,r.Delta);
            Assert.AreEqual(10, r.Count);
        }

        [Test]
        public void Validate_LongRange_default_constructor_assigns_correct_values()
        {
            var r = new LongRange();

            Assert.AreEqual(0L, r.Start);
            Assert.AreEqual(1L, r.Stop);
            Assert.AreEqual(1L, r.Delta);
            Assert.AreEqual(2, r.Count);
        }

        [Test]
        public void Validate_class_is_serializable()
        {
            Assert.IsInstanceOf<LongRange>(new LongRange().Clone<LongRange>());
        }

        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var r = new LongRange(0L, 9L, 10);

            var deserializedR = r.Clone<LongRange>();

            Assert.IsNotNull(deserializedR);

            Assert.AreEqual(0L, deserializedR.Start);
            Assert.AreEqual(9L, deserializedR.Stop);
            Assert.AreEqual(1L, deserializedR.Delta);
            Assert.AreEqual(10, deserializedR.Count);
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new LongRange(10L, 10L, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.AreEqual(0L, delta);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_mismatched_start_stop()
        {
            var r = new LongRange(10L, 20L, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.AreEqual(10L, delta);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Validate_multi_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new LongRange(10L, 10L, 3);

            var count = r.Count;
            var delta = r.Delta;

            Assert.AreEqual(3, count);
            Assert.AreEqual(0L, delta);
        }

        [Test]
        public void Validate_multi_count_returns_multiple_values_for_matched_start_stop()
        {
            var r = new LongRange(10L, 10L, 3);

            var values = r.ToArray();

            Assert.AreEqual(3, values.Length);
            foreach (var value in values)
            {
                Assert.AreEqual(10L, value);
            }
        }

        [Test]
        public void Test_clone()
        {
            var longRange = new LongRange(0L, 10L);
            Assert.IsInstanceOf<LongRange>(longRange.Clone());
        }

        [Test]
        public void Test_ToString()
        {
            var longRange = new LongRange(0L, 9L, 10);
            Assert.AreEqual("Start: 0, Stop: 9, Count: 10, Delta: 1", longRange.ToString());
        }
    }
}
