using System;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using NUnit.Framework.Constraints;
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

        [Test]
        public void Constructor_test()
        {
            nurbsGenerator = new NurbsGenerator(NurbsGeneratorType.SpatialFrequencyDomain);
            Assert.AreEqual(NurbsGeneratorType.SpatialFrequencyDomain, nurbsGenerator.GeneratorType);
            Assert.IsInstanceOf<NurbsValues>(nurbsGenerator.TimeValues);
            Assert.IsInstanceOf<NurbsValues>(nurbsGenerator.SpaceValues);
            Assert.IsInstanceOf<double [,]>(nurbsGenerator.ControlPoints);
            Assert.IsInstanceOf<double[]>(nurbsGenerator.NativeTimes);
            Assert.IsInstanceOf<List<BSplinesCoefficients>>(nurbsGenerator.TimeKnotSpanPolynomialCoefficients);
            
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
            try
            {
                NurbsValues nurbsValues = new NurbsValues(1.0);
                nurbsGenerator.FindSpan(nurbsValues, -1.0);
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("Negative parametric point not acceptable as input.", e.Message);
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
            try
            {
                nurbsGenerator.GetIntegralFunction(4, 1.0);
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("Degree is too high.", e.Message);
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

        [Test]
        public void EvaluateNurbsCurveFourierTransform_returns()
        {
            var nurbsValues = new NurbsValues(3);
            nurbsGenerator.TimeValues = nurbsValues;
            nurbsGenerator.SpaceValues = nurbsValues;
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 150.0, 25.0, 0.0},
                {0.0, 0.0, 0.0, 0.0, 0.0}};
            nurbsGenerator.ControlPoints = controlPoints;
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.KnotVector = knots;
            nurbsGenerator.TimeKnotSpanPolynomialCoefficients = new List<BSplinesCoefficients>
            {
                new BSplinesCoefficients(nurbsValues, 0),
                new BSplinesCoefficients(nurbsValues, 0),
                new BSplinesCoefficients(nurbsValues, 0),
                new BSplinesCoefficients(nurbsValues, 0),
                new BSplinesCoefficients(nurbsValues, 0),
                new BSplinesCoefficients(nurbsValues, 0),
                new BSplinesCoefficients(nurbsValues, 0),
                new BSplinesCoefficients(nurbsValues, 0)
            };

            var result = nurbsGenerator.EvaluateNurbsCurveFourierTransform(1.0, 2.0, 3.0);
            Assert.IsInstanceOf<Complex>(result);
        }

        [Test]
        public void EvaluateNurbsCurveFourierTransform_RealDomain_returns()
        {
            nurbsGenerator = new NurbsGenerator(NurbsGeneratorType.RealDomain);
            var nurbsValues = new NurbsValues(3);
            nurbsGenerator.TimeValues = nurbsValues;
            nurbsGenerator.SpaceValues = nurbsValues;
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 150.0, 25.0, 0.0},
                {0.0, 0.0, 0.0, 0.0, 0.0}};
            nurbsGenerator.ControlPoints = controlPoints;
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.KnotVector = knots;
            nurbsGenerator.TimeKnotSpanPolynomialCoefficients = new List<BSplinesCoefficients>
            {
                new BSplinesCoefficients(nurbsValues, 0),
                new BSplinesCoefficients(nurbsValues, 0),
                new BSplinesCoefficients(nurbsValues, 0),
                new BSplinesCoefficients(nurbsValues, 0),
                new BSplinesCoefficients(nurbsValues, 0),
                new BSplinesCoefficients(nurbsValues, 0),
                new BSplinesCoefficients(nurbsValues, 0),
                new BSplinesCoefficients(nurbsValues, 0)
            };

            var result = nurbsGenerator.EvaluateNurbsCurveFourierTransform(1.0, 2.0, 3.0);
            Assert.IsInstanceOf<Complex>(result);
        }

        [Test]
        public void ComputeCurvePoint_time()
        {
            var nurbsValues = new NurbsValues(3);
            double[] controlPoints = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.ControlPoints = controlPoints;
            nurbsGenerator.TimeValues = nurbsValues;
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.KnotVector = knots;

            var result = nurbsGenerator.ComputeCurvePoint(0.5, NurbsValuesDimensions.time);
            Assert.IsInstanceOf<double>(result);
            Assert.AreEqual(0.5, result);
        }

        [Test]
        public void ComputeCurvePoint_space()
        {
            var nurbsValues = new NurbsValues(3);
            double[] controlPoints = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.ControlPoints = controlPoints;
            nurbsGenerator.SpaceValues = nurbsValues;
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.KnotVector = knots;

            var result = nurbsGenerator.ComputeCurvePoint(0.5, NurbsValuesDimensions.space);
            Assert.IsInstanceOf<double>(result);
            Assert.AreEqual(0.5, result);
        }

        [Test]
        public void ComputeCurvePoint_throws_ArgumentException()
        {
            var nurbsValues = new NurbsValues(3);
            double[] controlPoints = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.ControlPoints = controlPoints;
            nurbsGenerator.SpaceValues = nurbsValues;
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.KnotVector = knots;

            Assert.Throws<ArgumentException>(() => nurbsGenerator.ComputeCurvePoint(0.5, (NurbsValuesDimensions)Enum.GetValues(typeof(NurbsValuesDimensions)).Length + 1));
        }

        [Test]
        public void ComputePointOutOfSurface_returns_expected_result()
        {
            nurbsGenerator = new NurbsGenerator();
            var nurbsValues = new NurbsValues(3);
            nurbsGenerator.TimeValues = nurbsValues;
            nurbsGenerator.SpaceValues = nurbsValues;
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 150.0, 25.0, 0.0},
                {0.0, 0.0, 0.0, 0.0, 0.0}};
            nurbsGenerator.ControlPoints = controlPoints;
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.KnotVector = knots;
            nurbsValues.MaxValue = 0.5;

            var result = nurbsGenerator.ComputePointOutOfSurface(0.5, 0.5, 0.1);
            Assert.IsInstanceOf<double>(result);
            Assert.AreEqual(0.1, result, 0.01);
        }

        [Test]
        public void BinarySearch_throws_exception()
        {
            var nurbsValues = new NurbsValues(3);
            nurbsValues.ValuesDimensions =
                (NurbsValuesDimensions)Enum.GetValues(typeof(NurbsValuesDimensions)).Length + 1;
            Assert.Throws<ArgumentException>(() => nurbsGenerator.BinarySearch(nurbsValues, 0.1));
        }

        [Test]
        public void FindSpan_space_test()
        {
            var nurbsValues = new NurbsValues(3);
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 150.0, 25.0, 0.0},
                {0.0, 0.0, 0.0, 0.0, 0.0}};
            nurbsGenerator.ControlPoints = controlPoints;
            nurbsValues.ValuesDimensions = NurbsValuesDimensions.space;
            var result = nurbsGenerator.FindSpan(nurbsValues, 1.0);
            Assert.AreEqual(4, result);
        }

        [Test]
        public void FindSpan_time_test()
        {
            var nurbsValues = new NurbsValues(3);
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 150.0, 25.0, 0.0},
                {0.0, 0.0, 0.0, 0.0, 0.0}};
            nurbsGenerator.ControlPoints = controlPoints;
            nurbsValues.ValuesDimensions = NurbsValuesDimensions.space;
            var result = nurbsGenerator.FindSpan(nurbsValues, 1.0);
            Assert.AreEqual(4, result);
        }

        [Test]
        public void FindSpan_throws_ArgumentException()
        {
            var nurbsValues = new NurbsValues(3);
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 150.0, 25.0, 0.0},
                {0.0, 0.0, 0.0, 0.0, 0.0}};
            nurbsGenerator.ControlPoints = controlPoints;
            nurbsValues.ValuesDimensions = (NurbsValuesDimensions)Enum.GetValues(typeof(NurbsValuesDimensions)).Length + 1;
            Assert.Throws<ArgumentException>(() => nurbsGenerator.FindSpan(nurbsValues, 1.0));
        }

        [Test]
        public void ComputePointOutOfSurface_time_space_greater_than_max_value()
        {
            nurbsGenerator = new NurbsGenerator();
            var nurbsValues = new NurbsValues(3);
            nurbsGenerator.GeneratorType = NurbsGeneratorType.SpatialFrequencyDomain;
            nurbsValues.MaxValue = 0.8;
            nurbsGenerator.TimeValues = nurbsValues;
            nurbsGenerator.SpaceValues = nurbsValues;
            nurbsValues.ValuesDimensions = NurbsValuesDimensions.space;
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 150.0, 25.0, 0.0},
                {0.0, 0.0, 0.0, 0.0, 0.0}};
            nurbsGenerator.ControlPoints = controlPoints;
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.KnotVector = knots;
            var result = nurbsGenerator.ComputePointOutOfSurface(1.0, 1.0, 0);
            Assert.AreEqual(0.0, result);
        }

        [Test]
        public void ComputePointOutOfSurface_not_spatial_frequency_domain()
        {
            nurbsGenerator = new NurbsGenerator();
            var nurbsValues = new NurbsValues(3);
            nurbsGenerator.GeneratorType = NurbsGeneratorType.RealDomain;
            nurbsValues.MaxValue = 6;
            nurbsGenerator.TimeValues = nurbsValues;
            nurbsGenerator.SpaceValues = nurbsValues;
            nurbsValues.ValuesDimensions = NurbsValuesDimensions.space;
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 150.0, 25.0, 0.0},
                {0.0, 0.0, 0.0, 0.0, 0.0}};
            nurbsGenerator.ControlPoints = controlPoints;
            double[] knots = { 0, 0.1, 0.01, 0.02, 0.1, 0.2, 0.3, 0.4 };
            nurbsValues.KnotVector = knots;
            var result = nurbsGenerator.ComputePointOutOfSurface(1.0, 7, 0);
            Assert.AreEqual(double.NaN, result);
        }

        [Test]
        public void ComputePointOutOfSurface_time_space_less_than_max_value()
        {
            var nurbsValues = new NurbsValues(3);
            nurbsValues.MaxValue = 1.2;
            nurbsGenerator.TimeValues = nurbsValues;
            nurbsGenerator.SpaceValues = nurbsValues;
            nurbsValues.ValuesDimensions = NurbsValuesDimensions.space;
            var result = nurbsGenerator.ComputePointOutOfSurface(1.0, 1.0, 0);
            Assert.AreEqual(0.0, result);
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
