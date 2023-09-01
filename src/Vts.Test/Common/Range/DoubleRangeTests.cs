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

            Assert.AreEqual(0d, r.Start);
            Assert.AreEqual(9d, r.Stop);
            Assert.AreEqual(1d, r.Delta);
            Assert.AreEqual(10, r.Count);
        }

        [Test]
        public void Validate_default_constructor_assigns_correct_values()
        {
            var r = new DoubleRange();

            Assert.AreEqual(0d, r.Start);
            Assert.AreEqual(1d, r.Stop);
            Assert.AreEqual(1d, r.Delta);
            Assert.AreEqual(2, r.Count);
        }

        [Test]
        public void Validate_class_is_serializable()
        {
            Assert.IsInstanceOf<DoubleRange>(new DoubleRange().Clone<DoubleRange>());
        }

        [Test]
        public void Validate_deserialized_class_is_correct()
        {
            var r = new DoubleRange(0d, 9d, 10);

            var deserializedR = r.Clone<DoubleRange>();

            Assert.IsNotNull(deserializedR);

            Assert.AreEqual(0d, deserializedR.Start);
            Assert.AreEqual(9d, deserializedR.Stop);
            Assert.AreEqual(1d, deserializedR.Delta);
            Assert.AreEqual(10, deserializedR.Count);
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new DoubleRange(10D, 10D, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.AreEqual(0D, delta);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Validate_single_count_returns_correct_delta_for_mismatched_start_stop()
        {
            var r = new DoubleRange(10D, 20D, 1);

            var delta = r.Delta;
            var count = r.Count;

            Assert.AreEqual(10D, delta);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Validate_multi_count_returns_correct_delta_for_matched_start_stop()
        {
            var r = new DoubleRange(10D, 10D, 3);

            var count = r.Count;
            var delta = r.Delta;

            Assert.AreEqual(3, count);
            Assert.AreEqual(0D, delta);
        }

        [Test]
        public void Validate_multi_count_returns_multiple_values_for_matched_start_stop()
        {
            var r = new DoubleRange(10D, 10D, 3);

            var values = r.ToArray();

            Assert.AreEqual(3, values.Length);
            foreach (var value in values)
            {
                Assert.AreEqual(10D, value);
            }
        }

        [Test]
        public void Test_clone()
        {
            var doubleRange = new DoubleRange(0.1, 0.9);
            Assert.IsInstanceOf<DoubleRange>(doubleRange.Clone());
        }

        [Test]
        public void Test_ToString()
        {
            var doubleRange = new DoubleRange(0.1, 0.9, 9);
            Assert.AreEqual("Start: 0.1, Stop: 0.9, Count: 9, Delta: 0.1", doubleRange.ToString());
        }
    }
}
