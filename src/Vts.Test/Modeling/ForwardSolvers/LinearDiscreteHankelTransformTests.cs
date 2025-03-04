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
            Assert.That(rhoValues.Length > 0, Is.True);
            Assert.That(dRho, Is.EqualTo(0.9).Within(0.01));
            Assert.That(rhoValues[0], Is.EqualTo(0));
            Assert.That(rhoValues[1], Is.EqualTo(0.909).Within(0.0001));
        }

        [Test]
        public void Test_GetHankelTransform_returns_correct_value()
        {

            double dRho;
            var rhoValues = LinearDiscreteHankelTransform.GetRho(0.1, 0.01, out dRho);
            var monteCarloForwardSolver = new MonteCarloForwardSolver();
            var rOfRho = monteCarloForwardSolver.ROfRho(new OpticalProperties(), rhoValues);
            var value = LinearDiscreteHankelTransform.GetHankelTransform(rhoValues, rOfRho, dRho, 0.1);
            Assert.That(value, Is.EqualTo(0.185).Within(0.001));
            Assert.That(dRho, Is.EqualTo(0.9).Within(0.01));
            Assert.That(rhoValues[0], Is.EqualTo(0));
            Assert.That(rhoValues[1], Is.EqualTo(0.909).Within(0.0001));
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
