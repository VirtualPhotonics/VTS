using System.Linq;
using NUnit.Framework;
using Vts.Extensions;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    public class BSplinesCoefficientsTests
    {
        private BSplinesCoefficients bSplinesCoefficients = null;

        [SetUp]
        public void SetUp()
        {
            bSplinesCoefficients = new BSplinesCoefficients();
        }
        /// <summary>
        /// Test against the BSplinesCoeficients constructor.
        /// </summary>
        [Test]
        public void BSplinesCoefficientsConstructor_KnownValues_ReturnsExpectedValues()
        {
            double[] knots = {0.0, 0.0, 0.0, 1.0, 2.0, 3.0, 4.0, 4.0, 5.0, 5.0, 5.0};
            int degree = 2;
            double[,] knownValues = {
                                    {1.0 ,  0.0, 0.0, 0.0},
                                    {-2.0,  2.0, 0.0, 0.0},
                                    {1.0 , -1.5, 0.5, 0.0}
                                    };
            NurbsValues nurbsValues = new NurbsValues(knots, degree, 1.0, knots);
            bSplinesCoefficients = new BSplinesCoefficients(nurbsValues, 2);
            Assert.That(bSplinesCoefficients.Coefficients, Is.EqualTo(knownValues), "Coefficients should be the same as the example on the Nurbs Book");
        }

        /// <summary>
        /// Test to check if the BSplinesCoefficients are = 0, over a knot span of length = 0.
        /// </summary>
        [Test]
        public void BSplinesCoefficientsConstructor_ZeroLengthKnotSpan_ReturnsExpectedValues()
        {
            double[] knots = { 0.0, 0.0, 0.0, 1.0, 2.0, 3.0, 4.0, 4.0, 5.0, 5.0, 5.0 };
            int degree = 2;
            NurbsValues nurbsValues = new NurbsValues(knots, degree, 1.0, knots);
            bSplinesCoefficients = new BSplinesCoefficients(nurbsValues, 6);
            Assert.That(bSplinesCoefficients.Coefficients.ToEnumerable<double>().Sum(), Is.EqualTo(0.0), "Coefficients should be the same as the example on the Nurbs Book");
        }

        [TearDown]
        public void TearDown()
        {
            bSplinesCoefficients = null;
        }

    }

}
