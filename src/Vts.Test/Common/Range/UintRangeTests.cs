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

            Assert.AreEqual(0U, r.Start);
            Assert.AreEqual(9U, r.Stop);
            Assert.AreEqual(1U, r.Delta);
            Assert.AreEqual(10, r.Count);
        }

        [Test]
        public void Validate_default_constructor_assigns_correct_values()
        {
            var r = new UIntRange();

            Assert.AreEqual(0U, r.Start);
            Assert.AreEqual(1U, r.Stop);
            Assert.AreEqual(1U, r.Delta);
            Assert.AreEqual(2, r.Count);
        }

        [Test]
        public void Validate_class_is_serializable()
        {
            Assert.IsInstanceOf<UIntRange>(new UIntRange().Clone<UIntRange>());
        }

        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var r = new UIntRange(0U, 9U, 10);

            var deserializedR = r.Clone<UIntRange>();

            Assert.IsInstanceOf<UIntRange>(deserializedR);

            Assert.AreEqual(0U, deserializedR.Start);
            Assert.AreEqual(9U, deserializedR.Stop);
            Assert.AreEqual(1U, deserializedR.Delta);
            Assert.AreEqual(10, deserializedR.Count);
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new UIntRange(10U, 10U, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.AreEqual(0U, delta);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_mismatched_start_stop()
        {
            var r = new UIntRange(10U, 20U, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.AreEqual(10U, delta);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Validate_multi_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new UIntRange(10U, 10U, 3);

            var count = r.Count;
            var delta = r.Delta;

            Assert.AreEqual(3, count);
            Assert.AreEqual(0U, delta);
        }

        [Test]
        public void Validate_multi_count_returns_multiple_values_for_matched_start_stop()
        {
            var r = new UIntRange(10U, 10U, 3);

            var values = r.ToArray();

            Assert.AreEqual(3, values.Length);
            foreach (var value in values)
            {
                Assert.AreEqual(10U, value);
            }
        }

        [Test]
        public void Test_clone()
        {
            var uIntRange = new UIntRange(0, 10);
            Assert.IsInstanceOf<UIntRange>(uIntRange.Clone());
        }

        [Test]
        public void Test_ToString()
        {
            var uIntRange = new UIntRange(0U, 9U, 10);
            Assert.AreEqual("Start: 0, Stop: 9, Count: 10, Delta: 1", uIntRange.ToString());
        }
    }
}
