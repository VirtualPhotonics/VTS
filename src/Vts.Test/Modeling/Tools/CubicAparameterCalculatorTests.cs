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
            Assert.AreEqual(68.447, result, 0.001);

        }
    }
}
