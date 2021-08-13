using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.Extensions;

namespace Vts.Test.Common.Extensions
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
    }
}
