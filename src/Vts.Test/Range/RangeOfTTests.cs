using System;
using NSubstitute;
using NUnit.Framework;
using Vts.Common;

namespace Vts.Test.Range
{
    [TestFixture]
    internal class RangeOfTTests
    {
        [Test]
        public void Test_default_constructor()
        {
            var rangeMock = Substitute.ForPartsOf<TestRange>();
            Assert.AreEqual("Start: 0, Stop: 0, Count: 1, Delta: 0", rangeMock.ToString());
        }

        [Test]
        public void Test_range_to_string()
        {
            var range = new IntRange(0, 9, 10);
            Assert.AreEqual("Start: 0, Stop: 9, Count: 10, Delta: 1", range.ToString());
        }

        [Test]
        public void Test_range_delta()
        {
            var range = new IntRange(0, 9, 10);
            Assert.AreEqual("Start: 0, Stop: 9, Count: 10, Delta: 1", range.ToString());
            range.Delta = 2;
            Assert.AreEqual("Start: 0, Stop: 9, Count: 10, Delta: 2", range.ToString());
        }
    }

    public class TestRange : Range<int>
    {
        protected override Func<int, int> GetIncrement()
        {
            throw new NotImplementedException();
        }

        protected override int GetDelta()
        {
            return 0;
        }
    }
}
