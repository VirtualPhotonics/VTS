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

            Assert.That(r.Start, Is.EqualTo(0f));
            Assert.That(r.Stop, Is.EqualTo(9f));
            Assert.That(r.Delta, Is.EqualTo(1f));
            Assert.That(r.Count, Is.EqualTo(10));
        }

        [Test]
        public void Validate_FloatRange_default_constructor_assigns_correct_values()
        {
            var r = new FloatRange();

            Assert.That(r.Start, Is.EqualTo(0f));
            Assert.That(r.Stop, Is.EqualTo(1f));
            Assert.That(r.Delta, Is.EqualTo(1f));
            Assert.That(r.Count, Is.EqualTo(2));
        }

        [Test]
        public void Validate_class_is_serializable()
        {
            Assert.That(new FloatRange().Clone<FloatRange>(), Is.InstanceOf<FloatRange>());
        }

        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var r = new FloatRange(0f, 9f, 10);

            var deserializedR = r.Clone<FloatRange>();

            Assert.IsNotNull(deserializedR);

            Assert.That(deserializedR.Start, Is.EqualTo(0f));
            Assert.That(deserializedR.Stop, Is.EqualTo(9f));
            Assert.That(deserializedR.Delta, Is.EqualTo(1f));
            Assert.That(deserializedR.Count, Is.EqualTo(10));
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new FloatRange(10f, 10f, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.That(delta, Is.EqualTo(0f));
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_mismatched_start_stop()
        {
            var r = new FloatRange(10f, 20f, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.That(delta, Is.EqualTo(10f));
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Validate_multi_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new FloatRange(10f, 10f, 3);

            var count = r.Count;
            var delta = r.Delta;

            Assert.That(count, Is.EqualTo(3));
            Assert.That(delta, Is.EqualTo(0f));
        }

        [Test]
        public void Validate_multi_count_returns_multiple_values_for_matched_start_stop()
        {
            var r = new FloatRange(10f, 10f, 3);

            var values = r.ToArray();

            Assert.That(values.Length, Is.EqualTo(3));
            foreach (var value in values)
            {
                Assert.That(value, Is.EqualTo(10f));
            }
        }

        [Test]
        public void Test_clone()
        {
            var floatRange = new FloatRange(0f, 10f);
            Assert.That(floatRange.Clone(), Is.InstanceOf<FloatRange>());
        }

        [Test]
        public void Test_ToString()
        {
            var floatRange = new FloatRange(0f, 9f, 10);
            Assert.That(floatRange.ToString(), Is.EqualTo("Start: 0, Stop: 9, Count: 10, Delta: 1"));
        }
    }
}
