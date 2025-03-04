using NUnit.Framework;
using Vts.Modeling;

namespace Vts.Test.Modeling.Tools
{
    [TestFixture]
    internal class CalculatorToolboxTests
    {
        private const double N = 1.4;

        [Test]
        public void Test_GetRadius()
        {
            var radius = CalculatorToolbox.GetRadius(4, 4, 4);
            Assert.That(radius, Is.EqualTo(6.928).Within(0.001));
            radius = CalculatorToolbox.GetRadius(4, 2);
            Assert.That(radius, Is.EqualTo(4.472).Within(0.001));
        }

        [Test]
        public void Test_GetFresnelReflectionMomentOfOrderM_using_Mu()
        {
            var result = CalculatorToolbox.GetFresnelReflectionMomentOfOrderM(1, N, 1.0);
            Assert.That(result, Is.EqualTo(0.529).Within(0.001));
        }

        [Test]
        public void Test_GetFresnelReflectionMomentOfOrderM_using_Theta()
        {
            var result = CalculatorToolbox.GetFresnelReflectionMomentOfOrderM(0, 0.3, 0.3);
            Assert.That(result, Is.EqualTo(6.123E-17d).Within(0.001));
        }

        [Test]
        public void Test_GetCubicFresnelReflectionMomentOfOrder1()
        {
            var result = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder1(N);
            Assert.That(result, Is.EqualTo(0.533).Within(0.001));
        }

        [Test]
        public void Test_GetCubicFresnelReflectionMomentOfOrder2()
        {
            var result = CalculatorToolbox.GetCubicFresnelReflectionMomentOfOrder2(N);
            Assert.That(result, Is.EqualTo(0.385).Within(0.001));
        }
    }
}
