using NUnit.Framework;
using Vts.Modeling;

namespace Vts.Test.Modeling.Tools
{
    [TestFixture]
    internal class CubicAparameterCalculatorTests
    {
        [Test]
        public void Test_CubicAparameterCalculator_returns_correct_value()
        {
            var result = CubicAparameterCalculator.GetA(5.0);
            Assert.That(result, Is.EqualTo(68.447).Within(0.001));

        }
    }
}
