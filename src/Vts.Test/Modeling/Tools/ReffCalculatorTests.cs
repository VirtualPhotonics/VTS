using NUnit.Framework;
using Vts.Modeling;

namespace Vts.Test.Modeling.Tools
{
    [TestFixture]
    internal class ReffCalculatorTests
    {
        [Test]
        public void Test_ReffCalculator_returns_correct_value()
        {
            var result = ReffCalculator.GetReff(5.0);
            Assert.AreEqual(2.221, result, 0.001);

        }
    }
}
