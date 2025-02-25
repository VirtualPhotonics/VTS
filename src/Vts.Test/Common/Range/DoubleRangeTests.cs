using NUnit.Framework;
using System.Linq;
using Vts.Common;
using Vts.IO;

namespace Vts.Test.Common
{
    [TestFixture]
    public class DoubleRangeTests
    {
        [Test]
        public void Validate_parameterized_constructor_assigns_correct_values()
        {
            var r = new DoubleRange(0d, 9d, 10);

            Assert.That(r.Start, Is.EqualTo(0d));
            Assert.That(r.Stop, Is.EqualTo(9d));
            Assert.That(r.Delta, Is.EqualTo(1d));
            Assert.That(r.Count, Is.EqualTo(10));
        }

        [Test]
        public void Validate_default_constructor_assigns_correct_values()
        {
            var r = new DoubleRange();

            Assert.That(r.Start, Is.EqualTo(0d));
            Assert.That(r.Stop, Is.EqualTo(1d));
            Assert.That(r.Delta, Is.EqualTo(1d));
            Assert.That(r.Count, Is.EqualTo(2));
        }

        [Test]
        public void Validate_class_is_serializable()
        {
            Assert.That(new DoubleRange().Clone<DoubleRange>(), Is.InstanceOf<DoubleRange>());
        }

        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var r = new DoubleRange(0d, 9d, 10);

            var deserializedR = r.Clone<DoubleRange>();

            Assert.IsNotNull(deserializedR);

            Assert.That(deserializedR.Start, Is.EqualTo(0d));
            Assert.That(deserializedR.Stop, Is.EqualTo(9d));
            Assert.That(deserializedR.Delta, Is.EqualTo(1d));
            Assert.That(deserializedR.Count, Is.EqualTo(10));
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new DoubleRange(10D, 10D, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.That(delta, Is.EqualTo(0D));
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_mismatched_start_stop()
        {
            var r = new DoubleRange(10D, 20D, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.That(delta, Is.EqualTo(10D));
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Validate_multi_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new DoubleRange(10D, 10D, 3);

            var count = r.Count;
            var delta = r.Delta;

            Assert.That(count, Is.EqualTo(3));
            Assert.That(delta, Is.EqualTo(0D));
        }

        [Test]
        public void Validate_multi_count_returns_multiple_values_for_matched_start_stop()
        {
            var r = new DoubleRange(10D, 10D, 3);

            var values = r.ToArray();

            Assert.That(values.Length, Is.EqualTo(3));
            foreach (var value in values)
            {
                Assert.That(value, Is.EqualTo(10D));
            }
        }

        [Test]
        public void Test_clone()
        {
            var doubleRange = new DoubleRange(0.1, 0.9);
            Assert.That(doubleRange.Clone(), Is.InstanceOf<DoubleRange>());
        }

        [Test]
        public void Test_ToString()
        {
            var doubleRange = new DoubleRange(0.1, 0.9, 9);
            Assert.That(doubleRange.ToString(), Is.EqualTo("Start: 0.1, Stop: 0.9, Count: 9, Delta: 0.1"));
        }
    }
}
