using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.Extensions;

namespace Vts.Test.Extensions
{
    [TestFixture]
    internal class DataExtensionsTests
    {
        [Test]
        public void AddNoise_extension_method_to_double_array_adds_noise()
        {
            var doubles = new[] {0.1, 0.2, 0.3};
            var doublesWithNoise = doubles.AddNoise(5.0);
            Assert.AreEqual(3, doublesWithNoise.Length);
        }

        [Test]
        public void AddNoise_extension_method_to_double_enumerable_adds_noise()
        {
            var doubles = new List<double> {0.1, 0.2, 0.3};
            var doublesWithNoise = doubles.AddNoise(5.0);
            Assert.AreEqual(3, doublesWithNoise.Count());
        }

        [Test]
        public void GetMidPoints_returns_correct_values()
        {
            var detectorRange = new DoubleRange(start: 0, stop: 40, number: 201);
            var midPoints = detectorRange.ToArray().GetMidpoints();
            Assert.AreEqual(200, midPoints.Length);
            Assert.AreEqual(0.1, midPoints[0], 0.001);
            Assert.AreEqual(19.9, midPoints[99], 0.001);
            Assert.AreEqual(28.1, midPoints[140], 0.001);
            Assert.AreEqual(39.9, midPoints[199], 0.001);
        }

        [Test]
        public void GetMidPoints_returns_empty_array()
        {
            var point = new[] { 0.5 };
            var midPoints = point.GetMidpoints();
            Assert.IsInstanceOf<double[]>(midPoints);
            Assert.AreEqual(0, midPoints.Length);
        }
    }
}
