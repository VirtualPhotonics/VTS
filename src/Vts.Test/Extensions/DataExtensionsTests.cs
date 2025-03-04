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
            Assert.That(doublesWithNoise.Length, Is.EqualTo(3));
        }

        [Test]
        public void AddNoise_extension_method_to_double_enumerable_adds_noise()
        {
            var doubles = new List<double> {0.1, 0.2, 0.3};
            var doublesWithNoise = doubles.AddNoise(5.0);
            Assert.That(doublesWithNoise.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GetMidPoints_returns_correct_values()
        {
            var detectorRange = new DoubleRange(start: 0, stop: 40, number: 201);
            var midPoints = detectorRange.ToArray().GetMidpoints();
            Assert.That(midPoints.Length, Is.EqualTo(200));
            Assert.That(midPoints[0], Is.EqualTo(0.1).Within(0.001));
            Assert.That(midPoints[99], Is.EqualTo(19.9).Within(0.001));
            Assert.That(midPoints[140], Is.EqualTo(28.1).Within(0.001));
            Assert.That(midPoints[199], Is.EqualTo(39.9).Within(0.001));
        }

        [Test]
        public void GetMidPoints_returns_empty_array()
        {
            var point = new[] { 0.5 };
            var midPoints = point.GetMidpoints();
            Assert.That(midPoints, Is.InstanceOf<double[]>());
            Assert.That(midPoints.Length, Is.EqualTo(0));
        }
    }
}
