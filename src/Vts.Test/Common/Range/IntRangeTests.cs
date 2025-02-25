using NUnit.Framework;
using System.Linq;
using Vts.Common;
using Vts.IO;

namespace Vts.Test.Common
{
    [TestFixture]
    public class IntRangeTests
    {

        [Test]
        public void Validate_class_is_serializable()
        {
            Assert.That(new IntRange().Clone<IntRange>(), Is.InstanceOf<IntRange>());
        }

        [Test]
        public void Validate_IntRange_constructor_assigns_correct_values()
        {
            var r = new IntRange(0, 9, 10);

            Assert.That(r.Start, Is.EqualTo(0));
            Assert.That(r.Stop, Is.EqualTo(9));
            Assert.That(r.Delta, Is.EqualTo(1));
            Assert.That(r.Count, Is.EqualTo(10));
        }
        
        [Test]
        public void Validate_IntRange_default_constructor_assigns_correct_values()
        {
            var r = new IntRange();

            Assert.That(r.Start, Is.EqualTo(0));
            Assert.That(r.Stop, Is.EqualTo(1));
            Assert.That(r.Delta, Is.EqualTo(1));
            Assert.That(r.Count, Is.EqualTo(2));
        }

        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var r = new IntRange(0, 9, 10);

            var deserializedR = r.Clone<IntRange>();

            Assert.IsNotNull(deserializedR);

            Assert.That(deserializedR.Start, Is.EqualTo(0));
            Assert.That(deserializedR.Stop, Is.EqualTo(9));
            Assert.That(deserializedR.Delta, Is.EqualTo(1));
            Assert.That(deserializedR.Count, Is.EqualTo(10));
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new IntRange(10, 10, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.That(delta, Is.EqualTo(0));
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_mismatched_start_stop()
        {
            var r = new IntRange(10, 20, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.That(delta, Is.EqualTo(10));
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Validate_multi_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new IntRange(10, 10, 3);

            var count = r.Count;
            var delta = r.Delta;

            Assert.That(count, Is.EqualTo(3));
            Assert.That(delta, Is.EqualTo(0));
        }

        [Test]
        public void Validate_multi_count_returns_multiple_values_for_matched_start_stop()
        {
            var r = new IntRange(10, 10, 3);

            var values = r.ToArray();

            Assert.That(values.Length, Is.EqualTo(3));
            foreach (var value in values)
            {
                Assert.That(value, Is.EqualTo(10));
            }
        }

        [Test]
        public void Test_clone()
        {
            var intRange = new IntRange(0, 10);
            Assert.That(intRange.Clone(), Is.InstanceOf<IntRange>());
        }

        [Test]
        public void Test_ToString()
        {
            var intRange = new IntRange(1, 9, 9);
            Assert.That(intRange.ToString(), Is.EqualTo("Start: 1, Stop: 9, Count: 9, Delta: 1"));
        }
    }
}
