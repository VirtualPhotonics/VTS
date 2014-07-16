using System;
using NUnit.Framework;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    public class NurbsGeneratorTests
    {
        private NurbsGenerator nurbsGenerator;

        /// <summary>
        /// Setup for the NurbsGenerator tests.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            nurbsGenerator = new NurbsGenerator();
            nurbsGenerator.GeneratorType = NurbsGeneratorType.Stub;
        }

        /// <summary>
        /// Test against the NurbsGenerator class.
        /// </summary>
        [Test]
        public void MultiplyControlPointsAndPolynomialCoefficients_KnownValues_ReturnsExpectedValues()
        {
            double[,] polynomialCoefs = { { 1.0, 1.0, 1.0 }, { 2.0, 2.0, 2.0 }, { 3.0, 3.0, 3.0 } };
            double[] controlPoints = { 1.0, 2.0, 3.0};
            double[] result = nurbsGenerator.MultiplyControlPointsAndPolynomialCoefficients(polynomialCoefs, controlPoints);
            double[] expectedResult = {6.0,12.0,18.0};
            Assert.AreEqual(expectedResult, result, "The multiplied control points should be equal to the expected values.");
        }

        /// <summary>
        /// Test against the NurbsGenerator class.
        /// </summary>
        [Test]
        //[ExpectedException(typeof(ArgumentException), ExpectedMessage = "Negative parametric point not acceptable as input.")]
        public void FindSpan_ParametricPointSmallerThenZero_ThowsException()
        {
            //Assert.Throws<ArgumentException>(delegate { nurbsGenerator.FindSpan(nurbsValues, -1.0); }, "Negative parametric point not acceptable as input.");
            try
            {
                NurbsValues nurbsValues = new NurbsValues(1.0);
                nurbsGenerator.FindSpan(nurbsValues, -1.0);
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(e.Message, "Negative parametric point not acceptable as input.");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Test against the NurbsGenerator class.
        /// </summary>
        [Test]
        public void BinarySearch_PointInsideKnotSpan_ReturnsCorrectSpanIndex()
        {
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0 };
            double[] controlPoints = { 1.0, 1.0, 1.0, 1.0 };
            NurbsValues nurbsValues = new NurbsValues(knots, 1, 1.0, controlPoints);
            int result = nurbsGenerator.BinarySearch(nurbsValues, 0.1);
            Assert.AreEqual(2, result, "The point lies in the third span, the returned index should be 2.");
        }

        /// <summary>
        /// Test against the NurbsGenerator class.
        /// </summary>
        [Test]
        public void BinarySearch_PointOnKnotSpanBoundary_ReturnsCorrectSpanIndex()
        {
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0 };
            double[] controlPoints = {1.0, 1.0, 1.0, 1.0};
            NurbsValues nurbsValues = new NurbsValues(knots, 2, 1.0, controlPoints);
            int result = nurbsGenerator.BinarySearch(nurbsValues, 0.5);
            Assert.AreEqual(3, result, "The point lies in the fourth span, the returned index should be 3.");
        }

        /// <summary>
        /// Test against the NurbsGenerator class.
        /// </summary>
        [Test]
        public void EvaluateBasisFunctions_KnownValues_ReturnsExpectedValues()
        {
            double[] knots = { 0.0, 0.0, 0.0, 1.0 / 5.0, 2.0 / 5.0, 3.0 / 5.0, 4.0 / 5.0, 4.0 / 5.0, 1.0, 1.0, 1.0 };
            NurbsValues nurbsValues = new NurbsValues(knots, 2, 1.0, knots);
            double[] basisFunctions = nurbsGenerator.EvaluateBasisFunctions(4, 0.5, nurbsValues);
            double result = 0.0;
            for (int i = 0; i < basisFunctions.Length; i++)
            {
                result += basisFunctions[i];
            }
            
            Assert.Less(1.0-result,0.00001,"The sum of the basis functions should be equal to one, within the rounding error.");
        }

        /// <summary>
        /// Test against the NurbsGenerator class.
        /// </summary>
        [Test]
        public void EvaluateCurvePoint_KnownValues_ReturnsExpectedValues()
        {
            double[] knots = { 0.0, 0.0, 0.0, 1.45 / 3.0, 2.382 / 3.0, 1.0, 1.0, 1.0 };
            double[] controlPoints = { 0.0, 6.0, 3.0, 6.0, 6.0 };
            double[] basisFunction = { 0.430, 0.4980, 0.0720 };
            NurbsValues nurbsValues = new NurbsValues(knots, 2, 3.0, controlPoints);

            double result = 3.204 - nurbsGenerator.EvaluateCurvePoint(2, basisFunction, nurbsValues);
            Assert.Less(result, 0.00001, "The returned value should be the same as the example at page 69 of the book 'An introduction to NURBS', within the rounding error.");
        }

        /// <summary>
        /// Test against the NurbsGenerator class.
        /// </summary>
        [Test]
        public void EvaluateSurfacePoint_KnownValues_ReturnsExpectedValues()
        {
            NurbsValues nurbsValues = new NurbsValues(3);
            nurbsGenerator.TimeValues = nurbsValues;
            nurbsGenerator.SpaceValues = nurbsValues;
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                                            {0.0 , 25.0, 50.0, 25.0, 0.0},
                                            {0.0 , 25.0, 50.0, 25.0, 0.0},
                                            {0.0 , 25.0, 150.0, 25.0, 0.0},
                                            {0.0, 0.0, 0.0, 0.0, 0.0}};
            nurbsGenerator.ControlPoints = controlPoints;
            int timeSpanIndex = 4;
            int spaceSpanIndex = 4;
            
            double[] timeBasisFunctions = { 1.0 / 4.0, 0.5, 1.0 / 4.0, 0.0 };
            double[] spaceBasisFunctions = { 1.0 / 32.0, 0.25, 19.0 / 32.0, 1.0 / 8.0 };

            double result = 62.5 - nurbsGenerator.EvaluateSurfacePoint(timeSpanIndex, timeBasisFunctions, spaceSpanIndex, spaceBasisFunctions);
            Assert.Less(result, 0.00001, "The returned value should be the same as the example at page 216 of the book 'An introduction to NURBS', within the rounding error.");
        }

        /// <summary>
        /// Test against NurbsGenerator.
        /// </summary>
        [Test]
        public void EvaluateKnotSpanIntegralValue_ExponentiaTermEqualZero_ReturnsExpectedValue()
        {
            double[,] polynomialCoefs = { { 1.0, 0.0, 0.0 }, { -2.0, 2.0, 0.0 }, { 1.0, -3.0 / 2.0, 1.0 / 2.0 } };
            double[] controlPoints = { 1.0, 1.0, 1.0 };
            double result = Math.Abs(1.0 - nurbsGenerator.EvaluateKnotSpanIntegralValue(0.0, polynomialCoefs, controlPoints, 0.0, 1.0, 1.0));
            Assert.Less(result, 0.000001, "The Value should be as the example from the Nurbs Book at page 55, within the rounding error.");
        }

        /// <summary>
        /// Test against NurbsGenerator.
        /// </summary>
        [Test]
        public void EvaluateKnotSpanIntegralValue_ZeroDegreePolynomialExponentiaTermEqualMinusOne_ReturnsExpectedValue()
        {
            double[,] polynomialCoefs = { { 1.0, 1.0, 1.0 }, { 0.0, 0.0, 0.0 }, { 0.0, 0.0, 0.0 } };
            double[] controlPoints = { 1.0, 2.0, 3.0 };
            double result = nurbsGenerator.EvaluateKnotSpanIntegralValue(1.0, polynomialCoefs, controlPoints, 0.0, 1.0, 1.0);
            Assert.AreEqual(6.0 * (1.0 - 1.0 / Math.E), result, "The Value should be as the example from the Nurbs Book at page 55.");
        }

        /// <summary>
        /// Test against NurbsGenerator.
        /// </summary>
        [Test]
        public void EvaluateKnotSpanIntegralValue_FirstDegreePolynomialExponentiaTermEqualMinusOne_ReturnsExpectedValue()
        {
            double[,] polynomialCoefs = { { 0.0, 0.0, 0.0 }, { 1.0, 1.0, 1.0 }, { 0.0, 0.0, 0.0 } };
            double[] controlPoints = { 1.0, 2.0, 3.0 };
            double result = nurbsGenerator.EvaluateKnotSpanIntegralValue(1.0, polynomialCoefs, controlPoints, 0.0, 1.0, 1.0);
            Assert.AreEqual(6.0 * (1.0 - 2.0 / Math.E), result, "The Value should be as the example from the Nurbs Book at page 55.");
        }

        /// <summary>
        /// Test against NurbsGenerator.
        /// </summary>
        [Test]
        public void EvaluateKnotSpanIntegralValue_SecondDegreePolynomialExponentiaTermEqualMinusOne_ReturnsExpectedValue()
        {
            double[,] polynomialCoefs = { { 0.0, 0.0, 0.0, 0.0 },
                                          { 0.0, 0.0, 0.0, 0.0 },
                                          { 1.0, 1.0, 1.0, 0.0 } };
            double[] controlPoints = { 1.0, 2.0, 3.0, 4.0 };

            double result = Math.Abs(6.0 * (2.0 - 5.0 / Math.E) -
                nurbsGenerator.EvaluateKnotSpanIntegralValue(1.0, polynomialCoefs, controlPoints, 0.0, 1.0, 1.0));
            Assert.Less(result, 0.000001, "The Value should be as the example from the Nurbs Book at page 55, within the rounding error.");
        }

        /// <summary>
        /// Test against NurbsGenerator.
        /// </summary>
        [Test]
        public void EvaluateKnotSpanIntegralValue_ThirdDegreePolynomialExponentiaTermEqualMinusOne_ReturnsExpectedValue()
        {
            double[,] polynomialCoefs = { { 0.0, 0.0, 0.0, 0.0, 0.0 },
                                          { 0.0, 0.0, 0.0, 0.0, 0.0},
                                          { 0.0, 0.0, 0.0, 0.0, 0.0 },
                                          { 1.0, 1.0, 1.0, 1.0, 0.0 }};
            double[] controlPoints = { 1.0, 2.0, 3.0, 4.0, 5.0 };
            double result = nurbsGenerator.EvaluateKnotSpanIntegralValue(1.0,
                                                                             polynomialCoefs,
                                                                             controlPoints,
                                                                             0.0, 1.0, 1.0);
            Assert.AreEqual(10.0 * (6.0 - 16.0 / Math.E), result, "The Value should be as the example from the Nurbs Book at page 55.");
        }

        /// <summary>
        /// Test against NurbsGenerator.
        /// </summary>
        [Test]
        //[ExpectedException(typeof(ArgumentException), ExpectedMessage = "Degree is too high.")]
        public void GetIntegralFunction_DegreeHigherThenMaxDegreeExpNotNull_ThrowsException()
        {
            // Assert.Throws<ArgumentException>(delegate { nurbsGenerator.GetIntegralFunction(4, 1.0); }, "Degree is too high.");
            try
            {
                nurbsGenerator.GetIntegralFunction(4, 1.0);
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(e.Message, "Degree is too high.");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Test against NurbsGenerator.
        /// </summary>
        [Test]
        public void IntegrateExponentialMultipliedBySinglePolinomial_ConstantFunction_ReturnsCorrectArea()
        {
            double result = nurbsGenerator.IntegrateExponentialMultipliedByMomomial(0, 0, 1.0, 0.0, 1.0);
            Assert.AreEqual(1.0, result, "the integral value of a constant function equal 1 in the interval [0,1] should be 1.");
        }

        /// <summary>
        /// Test against NurbsGenerator.
        /// </summary>
        [Test]
        public void IntegrateExponentialMultipliedByMultiplePolinomial_NullFunction_ReturnsZero()
        {
            double[] nullCoefficents = { 0.0, 0.0, 0.0 };
            double result = nurbsGenerator.IntegrateExponentialMultipliedByPolynomial(0.0, nullCoefficents, 0, 100);
            Assert.AreEqual(0.0, result, "The integral value of a null function should be zero.");
        }


        /// <summary>
        /// Tear down for the NurbsGenerator tests.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            nurbsGenerator = null;
        }
    }
}
