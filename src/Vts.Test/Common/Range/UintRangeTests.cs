using NUnit.Framework;
using System.Linq;
using Vts.Common;
using Vts.IO;

namespace Vts.Test.Common
{
    [TestFixture]
    public class UintRangeTests
    {
        [Test]
        public void Validate_parameterized_constructor_assigns_correct_values()
        {
            var r = new UIntRange(0, 9, 10);

            Assert.That(r.Start, Is.EqualTo(0U));
            Assert.That(r.Stop, Is.EqualTo(9U));
            Assert.That(r.Delta, Is.EqualTo(1U));
            Assert.That(r.Count, Is.EqualTo(10));
        }

        [Test]
        public void Validate_default_constructor_assigns_correct_values()
        {
            var r = new UIntRange();

            Assert.That(r.Start, Is.EqualTo(0U));
            Assert.That(r.Stop, Is.EqualTo(1U));
            Assert.That(r.Delta, Is.EqualTo(1U));
            Assert.That(r.Count, Is.EqualTo(2));
        }

        [Test]
        public void Validate_class_is_serializable()
        {
            Assert.That(new UIntRange().Clone<UIntRange>(), Is.InstanceOf<UIntRange>());
        }

        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var r = new UIntRange(0U, 9U, 10);

            var deserializedR = r.Clone<UIntRange>();

            Assert.That(deserializedR, Is.InstanceOf<UIntRange>());

            Assert.That(deserializedR.Start, Is.EqualTo(0U));
            Assert.That(deserializedR.Stop, Is.EqualTo(9U));
            Assert.That(deserializedR.Delta, Is.EqualTo(1U));
            Assert.That(deserializedR.Count, Is.EqualTo(10));
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new UIntRange(10U, 10U, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.That(delta, Is.EqualTo(0U));
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_mismatched_start_stop()
        {
            var r = new UIntRange(10U, 20U, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.That(delta, Is.EqualTo(10U));
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Validate_multi_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new UIntRange(10U, 10U, 3);

            var count = r.Count;
            var delta = r.Delta;

            Assert.That(count, Is.EqualTo(3));
            Assert.That(delta, Is.EqualTo(0U));
        }

        [Test]
        public void Validate_multi_count_returns_multiple_values_for_matched_start_stop()
        {
            var r = new UIntRange(10U, 10U, 3);

            var values = r.ToArray();

            Assert.That(values.Length, Is.EqualTo(3));
            foreach (var value in values)
            {
                Assert.That(value, Is.EqualTo(10U));
            }
        }

        [Test]
        public void Test_clone()
        {
            var uIntRange = new UIntRange(0, 10);
            Assert.That(uIntRange.Clone(), Is.InstanceOf<UIntRange>());
        }

        [Test]
        public void Test_ToString()
        {
            var uIntRange = new UIntRange(0U, 9U, 10);
            Assert.That(uIntRange.ToString(), Is.EqualTo("Start: 0, Stop: 9, Count: 10, Delta: 1"));
        }
    }
}
