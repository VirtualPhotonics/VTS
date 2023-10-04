using NUnit.Framework;
using System;
using System.Linq;
using Vts.Common.Math;

namespace Vts.Test.Common.Math
{
    [TestFixture]
    internal class StatisticsTests
    {
        [Test]
        public void Test_ExpectedValue_returns_expected_value()
        {
            var p = new double[] { 1, 2, 3 };
            var pOfX = new double[] { 2, 4, 6 };
            var expectedValue = Statistics.ExpectedValue(p, pOfX);
            Assert.AreEqual(28.0, expectedValue);
        }

        [Test]
        public void Test_MeanSamplingDepth_returns_mean_sampling_depth()
        {
            var array = Enumerable.Repeat(2.0, 16).ToArray();
            var x = new double[] { 1, 2, 3, 4 };
            var y = new double[] { 1, 2, 3, 4 };
            var dx = new double[] { 2, 2, 2, 2 };
            var dy = new double[] { 2, 2, 2, 2 };
            var meanSamplingDepth = Statistics.MeanSamplingDepth(array, x, y, dx, dy);
            Assert.AreEqual(2.5, meanSamplingDepth);
        }

        [Test]
        public void Test_MeanSamplingDepth_throws_argument_exception()
        {
            var array = new double[10];
            var x = new double[] { 1, 2, 3, 4 };
            var y = new double[] { 1, 2, 3, 4 };
            var dx = Array.Empty<double>();
            var dy = Array.Empty<double>();
            Assert.Throws<ArgumentException>(() =>
            {
                try
                {
                    Statistics.MeanSamplingDepth(array, x, y, dx, dy);
                }
                catch (Exception e)
                {
                    Assert.AreEqual("Dimensions of array must be dimension of x * dimension of y", e.Message);
                    throw;
                }
            });
        }
    }
}
