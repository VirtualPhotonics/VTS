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

            Assert.That(r.Start, Is.EqualTo(0L));
            Assert.That(r.Stop, Is.EqualTo(9L));
            Assert.That(r.Delta, Is.EqualTo(1L));
            Assert.That(r.Count, Is.EqualTo(10));
        }

        [Test]
        public void Validate_LongRange_default_constructor_assigns_correct_values()
        {
            var r = new LongRange();

            Assert.That(r.Start, Is.EqualTo(0L));
            Assert.That(r.Stop, Is.EqualTo(1L));
            Assert.That(r.Delta, Is.EqualTo(1L));
            Assert.That(r.Count, Is.EqualTo(2));
        }

        [Test]
        public void Validate_class_is_serializable()
        {
            Assert.That(new LongRange().Clone<LongRange>(), Is.InstanceOf<LongRange>());
        }

        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var r = new LongRange(0L, 9L, 10);

            var deserializedR = r.Clone<LongRange>();

            Assert.IsNotNull(deserializedR);

            Assert.That(deserializedR.Start, Is.EqualTo(0L));
            Assert.That(deserializedR.Stop, Is.EqualTo(9L));
            Assert.That(deserializedR.Delta, Is.EqualTo(1L));
            Assert.That(deserializedR.Count, Is.EqualTo(10));
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new LongRange(10L, 10L, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.That(delta, Is.EqualTo(0L));
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_mismatched_start_stop()
        {
            var r = new LongRange(10L, 20L, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.That(delta, Is.EqualTo(10L));
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Validate_multi_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new LongRange(10L, 10L, 3);

            var count = r.Count;
            var delta = r.Delta;

            Assert.That(count, Is.EqualTo(3));
            Assert.That(delta, Is.EqualTo(0L));
        }

        [Test]
        public void Validate_multi_count_returns_multiple_values_for_matched_start_stop()
        {
            var r = new LongRange(10L, 10L, 3);

            var values = r.ToArray();

            Assert.That(values.Length, Is.EqualTo(3));
            foreach (var value in values)
            {
                Assert.That(value, Is.EqualTo(10L));
            }
        }

        [Test]
        public void Test_clone()
        {
            var longRange = new LongRange(0L, 10L);
            Assert.That(longRange.Clone(), Is.InstanceOf<LongRange>());
        }

        [Test]
        public void Test_ToString()
        {
            var longRange = new LongRange(0L, 9L, 10);
            Assert.That(longRange.ToString(), Is.EqualTo("Start: 0, Stop: 9, Count: 10, Delta: 1"));
        }
    }
}
