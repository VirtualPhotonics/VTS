using Meta.Numerics;
using NUnit.Framework;
using Vts.Modeling;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    internal class LinearDiscreteHankelTransformTests
    {
        [Test]
        public void Test_GetRho_returns_correct_values()
        {
            double dRho;
            var rhoValues = LinearDiscreteHankelTransform.GetRho(0.1, 0.01, out dRho);
            Assert.IsTrue(rhoValues.Length > 0);
            Assert.AreEqual(0.9, dRho, 0.01);
            Assert.AreEqual(0, rhoValues[0]);
            Assert.AreEqual(0.909, rhoValues[1], 0.0001);
        }

        [Test]
        public void Test_GetHankelTransform_returns_correct_value()
        {

            double dRho;
            var rhoValues = LinearDiscreteHankelTransform.GetRho(0.1, 0.01, out dRho);
            var monteCarloForwardSolver = new MonteCarloForwardSolver();
            var rOfRho = monteCarloForwardSolver.ROfRho(new OpticalProperties(), rhoValues);
            var value = LinearDiscreteHankelTransform.GetHankelTransform(rhoValues, rOfRho, dRho, 0.1);
            Assert.AreEqual(0.185, value, 0.001);
            Assert.AreEqual(0.9, dRho, 0.01);
            Assert.AreEqual(0, rhoValues[0]);
            Assert.AreEqual(0.909, rhoValues[1], 0.0001);
        }

        [Test]
        public void Test_GetHankelTransform_throws_exception()
        {
            var rhos = new double[] { 0, 0.1, 0.2 };
            var rOfRho = new double[] { 0, 0.9, 1.1, 2.2 };
            Assert.Throws<DimensionMismatchException>(() => LinearDiscreteHankelTransform.GetHankelTransform(rhos, rOfRho, 0.1, 0.1));

        }
    }
}
