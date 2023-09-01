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
            Assert.IsInstanceOf<IntRange>(new IntRange().Clone<IntRange>());
        }

        [Test]
        public void Validate_IntRange_constructor_assigns_correct_values()
        {
            var r = new IntRange(0, 9, 10);

            Assert.AreEqual(0, r.Start);
            Assert.AreEqual(9, r.Stop);
            Assert.AreEqual(1, r.Delta);
            Assert.AreEqual(10, r.Count);
        }
        
        [Test]
        public void Validate_IntRange_default_constructor_assigns_correct_values()
        {
            var r = new IntRange();

            Assert.AreEqual(0, r.Start);
            Assert.AreEqual(1, r.Stop);
            Assert.AreEqual(1, r.Delta);
            Assert.AreEqual(2, r.Count);
        }

        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var r = new IntRange(0, 9, 10);

            var deserializedR = r.Clone<IntRange>();

            Assert.IsNotNull(deserializedR);

            Assert.AreEqual(0, deserializedR.Start);
            Assert.AreEqual(9, deserializedR.Stop);
            Assert.AreEqual(1, deserializedR.Delta);
            Assert.AreEqual(10, deserializedR.Count);
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new IntRange(10, 10, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.AreEqual(0, delta);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_mismatched_start_stop()
        {
            var r = new IntRange(10, 20, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.AreEqual(10, delta);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Validate_multi_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new IntRange(10, 10, 3);

            var count = r.Count;
            var delta = r.Delta;

            Assert.AreEqual(3, count);
            Assert.AreEqual(0, delta);
        }

        [Test]
        public void Validate_multi_count_returns_multiple_values_for_matched_start_stop()
        {
            var r = new IntRange(10, 10, 3);

            var values = r.ToArray();

            Assert.AreEqual(3, values.Length);
            foreach (var value in values)
            {
                Assert.AreEqual(10, value);
            }
        }

        [Test]
        public void Test_clone()
        {
            var intRange = new IntRange(0, 10);
            Assert.IsInstanceOf<IntRange>(intRange.Clone());
        }

        [Test]
        public void Test_ToString()
        {
            var intRange = new IntRange(1, 9, 9);
            Assert.AreEqual("Start: 1, Stop: 9, Count: 9, Delta: 1", intRange.ToString());
        }
    }
}
