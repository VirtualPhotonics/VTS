using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    public class NurbsGeneratorTests
    {
        private NurbsGenerator _nurbsGenerator;

        [SetUp]
        public void Setup()
        {
            _nurbsGenerator = new NurbsGenerator
            {
                GeneratorType = NurbsGeneratorType.Stub
            };
        }

        [Test]
        public void Constructor_test()
        {
            _nurbsGenerator = new NurbsGenerator(NurbsGeneratorType.SpatialFrequencyDomain);
            Assert.AreEqual(NurbsGeneratorType.SpatialFrequencyDomain, _nurbsGenerator.GeneratorType);
            Assert.IsInstanceOf<NurbsValues>(_nurbsGenerator.TimeValues);
            Assert.IsInstanceOf<NurbsValues>(_nurbsGenerator.SpaceValues);
            Assert.IsInstanceOf<double[,]>(_nurbsGenerator.ControlPoints);
            Assert.IsInstanceOf<double[]>(_nurbsGenerator.NativeTimes);
            Assert.IsInstanceOf<List<BSplinesCoefficients>>(_nurbsGenerator.TimeKnotSpanPolynomialCoefficients);

        }

        [Test]
        public void MultiplyControlPointsAndPolynomialCoefficients_KnownValues_ReturnsExpectedValues()
        {
            double[,] polynomialCoefficients = { { 1.0, 1.0, 1.0 }, { 2.0, 2.0, 2.0 }, { 3.0, 3.0, 3.0 } };
            double[] controlPoints = { 1.0, 2.0, 3.0 };
            var result = _nurbsGenerator.MultiplyControlPointsAndPolynomialCoefficients(polynomialCoefficients, controlPoints);
            double[] expectedResult = { 6.0, 12.0, 18.0 };
            Assert.AreEqual(expectedResult, result, "The multiplied control points should be equal to the expected values.");
        }

        [Test]
        public void FindSpan_ParametricPointSmallerThenZero_ThrowsException()
        {
            try
            {
                var nurbsValues = new NurbsValues(1.0);
                _nurbsGenerator.FindSpan(nurbsValues, -1.0);
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
            var nurbsValues = new NurbsValues(knots, 1, 1.0, controlPoints);
            var result = _nurbsGenerator.BinarySearch(nurbsValues, 0.1);
            Assert.AreEqual(2, result, "The point lies in the third span, the returned index should be 2.");
        }

        [Test]
        public void BinarySearch_PointOnKnotSpanBoundary_ReturnsCorrectSpanIndex()
        {
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0 };
            double[] controlPoints = { 1.0, 1.0, 1.0, 1.0 };
            var nurbsValues = new NurbsValues(knots, 2, 1.0, controlPoints);
            var result = _nurbsGenerator.BinarySearch(nurbsValues, 0.5);
            Assert.AreEqual(3, result, "The point lies in the fourth span, the returned index should be 3.");
        }

        [Test]
        public void EvaluateBasisFunctions_KnownValues_ReturnsExpectedValues()
        {
            double[] knots = { 0.0, 0.0, 0.0, 1.0 / 5.0, 2.0 / 5.0, 3.0 / 5.0, 4.0 / 5.0, 4.0 / 5.0, 1.0, 1.0, 1.0 };
            var nurbsValues = new NurbsValues(knots, 2, 1.0, knots);
            var basisFunctions = _nurbsGenerator.EvaluateBasisFunctions(4, 0.5, nurbsValues);
            var result = basisFunctions.Sum();

            Assert.Less(1.0 - result, 0.00001, "The sum of the basis functions should be equal to one, within the rounding error.");
        }

        [Test]
        public void EvaluateCurvePoint_KnownValues_ReturnsExpectedValues()
        {
            double[] knots = { 0.0, 0.0, 0.0, 1.45 / 3.0, 2.382 / 3.0, 1.0, 1.0, 1.0 };
            double[] controlPoints = { 0.0, 6.0, 3.0, 6.0, 6.0 };
            double[] basisFunction = { 0.430, 0.4980, 0.0720 };
            var nurbsValues = new NurbsValues(knots, 2, 3.0, controlPoints);

            var result = 3.204 - _nurbsGenerator.EvaluateCurvePoint(2, basisFunction, nurbsValues);
            Assert.Less(result, 0.00001, "The returned value should be the same as the example at page 69 of the book 'An introduction to NURBS', within the rounding error.");
        }

        [Test]
        public void EvaluateSurfacePoint_KnownValues_ReturnsExpectedValues()
        {
            var nurbsValues = new NurbsValues(3);
            _nurbsGenerator.TimeValues = nurbsValues;
            _nurbsGenerator.SpaceValues = nurbsValues;
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                                            {0.0 , 25.0, 50.0, 25.0, 0.0},
                                            {0.0 , 25.0, 50.0, 25.0, 0.0},
                                            {0.0 , 25.0, 150.0, 25.0, 0.0},
                                            {0.0, 0.0, 0.0, 0.0, 0.0}};
            _nurbsGenerator.ControlPoints = controlPoints;
            const int timeSpanIndex = 4;
            const int spaceSpanIndex = 4;

            double[] timeBasisFunctions = { 1.0 / 4.0, 0.5, 1.0 / 4.0, 0.0 };
            double[] spaceBasisFunctions = { 1.0 / 32.0, 0.25, 19.0 / 32.0, 1.0 / 8.0 };

            var result = 62.5 - _nurbsGenerator.EvaluateSurfacePoint(timeSpanIndex, timeBasisFunctions, spaceSpanIndex, spaceBasisFunctions);
            Assert.Less(result, 0.00001, "The returned value should be the same as the example at page 216 of the book 'An introduction to NURBS', within the rounding error.");
        }

        [Test]
        public void EvaluateKnotSpanIntegralValue_ExponentialTermEqualZero_ReturnsExpectedValue()
        {
            double[,] polynomialCoefs = { { 1.0, 0.0, 0.0 }, { -2.0, 2.0, 0.0 }, { 1.0, -3.0 / 2.0, 1.0 / 2.0 } };
            double[] controlPoints = { 1.0, 1.0, 1.0 };
            var result = Math.Abs(1.0 - _nurbsGenerator.EvaluateKnotSpanIntegralValue(0.0, polynomialCoefs, controlPoints, 0.0, 1.0, 1.0));
            Assert.Less(result, 0.000001, "The Value should be as the example from the Nurbs Book at page 55, within the rounding error.");
        }

        [Test]
        public void EvaluateKnotSpanIntegralValue_ZeroDegreePolynomialExponentialTermEqualMinusOne_ReturnsExpectedValue()
        {
            double[,] polynomialCoefs = { { 1.0, 1.0, 1.0 }, { 0.0, 0.0, 0.0 }, { 0.0, 0.0, 0.0 } };
            double[] controlPoints = { 1.0, 2.0, 3.0 };
            var result = _nurbsGenerator.EvaluateKnotSpanIntegralValue(1.0, polynomialCoefs, controlPoints, 0.0, 1.0, 1.0);
            Assert.AreEqual(6.0 * (1.0 - 1.0 / Math.E), result, "The Value should be as the example from the Nurbs Book at page 55.");
        }

        [Test]
        public void EvaluateKnotSpanIntegralValue_FirstDegreePolynomialExponentialTermEqualMinusOne_ReturnsExpectedValue()
        {
            double[,] polynomialCoefs = { { 0.0, 0.0, 0.0 }, { 1.0, 1.0, 1.0 }, { 0.0, 0.0, 0.0 } };
            double[] controlPoints = { 1.0, 2.0, 3.0 };
            var result = _nurbsGenerator.EvaluateKnotSpanIntegralValue(1.0, polynomialCoefs, controlPoints, 0.0, 1.0, 1.0);
            Assert.AreEqual(6.0 * (1.0 - 2.0 / Math.E), result, "The Value should be as the example from the Nurbs Book at page 55.");
        }

        [Test]
        public void EvaluateKnotSpanIntegralValue_SecondDegreePolynomialExponentialTermEqualMinusOne_ReturnsExpectedValue()
        {
            double[,] polynomialCoefs = { { 0.0, 0.0, 0.0, 0.0 },
                                          { 0.0, 0.0, 0.0, 0.0 },
                                          { 1.0, 1.0, 1.0, 0.0 } };
            double[] controlPoints = { 1.0, 2.0, 3.0, 4.0 };

            var result = Math.Abs(6.0 * (2.0 - 5.0 / Math.E) -
                                  _nurbsGenerator.EvaluateKnotSpanIntegralValue(1.0, polynomialCoefs, controlPoints, 0.0, 1.0, 1.0));
            Assert.Less(result, 0.000001, "The Value should be as the example from the Nurbs Book at page 55, within the rounding error.");
        }

        [Test]
        public void EvaluateKnotSpanIntegralValue_ThirdDegreePolynomialExponentialTermEqualMinusOne_ReturnsExpectedValue()
        {
            double[,] polynomialCoefs = { { 0.0, 0.0, 0.0, 0.0, 0.0 },
                                          { 0.0, 0.0, 0.0, 0.0, 0.0},
                                          { 0.0, 0.0, 0.0, 0.0, 0.0 },
                                          { 1.0, 1.0, 1.0, 1.0, 0.0 }};
            double[] controlPoints = { 1.0, 2.0, 3.0, 4.0, 5.0 };
            var result = _nurbsGenerator.EvaluateKnotSpanIntegralValue(1.0, polynomialCoefs, controlPoints, 0.0, 1.0, 1.0);
            Assert.AreEqual(10.0 * (6.0 - 16.0 / Math.E), result, "The Value should be as the example from the Nurbs Book at page 55.");
        }

        [Test]
        public void GetIntegralFunction_DegreeHigherThenMaxDegreeExpNotNull_ThrowsException()
        {
            try
            {
                _nurbsGenerator.GetIntegralFunction(4, 1.0);
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

        [Test]
        public void IntegrateExponentialMultipliedBySinglePolynomial_ConstantFunction_ReturnsCorrectArea()
        {
            var result = _nurbsGenerator.IntegrateExponentialMultipliedByMomomial(0, 0, 1.0, 0.0, 1.0);
            Assert.AreEqual(1.0, result, "the integral value of a constant function equal 1 in the interval [0,1] should be 1.");
        }

        [Test]
        public void IntegrateExponentialMultipliedByMultiplePolynomial_NullFunction_ReturnsZero()
        {
            double[] nullCoefficents = { 0.0, 0.0, 0.0 };
            var result = _nurbsGenerator.IntegrateExponentialMultipliedByPolynomial(0.0, nullCoefficents, 0, 100);
            Assert.AreEqual(0.0, result, "The integral value of a null function should be zero.");
        }

        [Test]
        public void EvaluateNurbsCurveFourierTransform_returns()
        {
            var nurbsValues = new NurbsValues(3);
            _nurbsGenerator.TimeValues = nurbsValues;
            _nurbsGenerator.SpaceValues = nurbsValues;
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 150.0, 25.0, 0.0},
                {0.0, 0.0, 0.0, 0.0, 0.0}};
            _nurbsGenerator.ControlPoints = controlPoints;
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.KnotVector = knots;
            _nurbsGenerator.TimeKnotSpanPolynomialCoefficients = new List<BSplinesCoefficients>
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

            var result = _nurbsGenerator.EvaluateNurbsCurveFourierTransform(1.0, 2.0, 3.0);
            Assert.IsInstanceOf<Complex>(result);
        }

        [Test]
        public void EvaluateNurbsCurveFourierTransform_RealDomain_returns()
        {
            _nurbsGenerator = new NurbsGenerator(NurbsGeneratorType.RealDomain);
            var nurbsValues = new NurbsValues(3);
            _nurbsGenerator.TimeValues = nurbsValues;
            _nurbsGenerator.SpaceValues = nurbsValues;
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 150.0, 25.0, 0.0},
                {0.0, 0.0, 0.0, 0.0, 0.0}};
            _nurbsGenerator.ControlPoints = controlPoints;
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.KnotVector = knots;
            _nurbsGenerator.TimeKnotSpanPolynomialCoefficients = new List<BSplinesCoefficients>
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

            var result = _nurbsGenerator.EvaluateNurbsCurveFourierTransform(1.0, 2.0, 3.0);
            Assert.IsInstanceOf<Complex>(result);
        }

        [Test]
        public void ComputeCurvePoint_time()
        {
            var nurbsValues = new NurbsValues(3);
            double[] controlPoints = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.ControlPoints = controlPoints;
            _nurbsGenerator.TimeValues = nurbsValues;
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.KnotVector = knots;

            var result = _nurbsGenerator.ComputeCurvePoint(0.5, NurbsValuesDimensions.time);
            Assert.IsInstanceOf<double>(result);
            Assert.AreEqual(0.5, result);
        }

        [Test]
        public void ComputeCurvePoint_space()
        {
            var nurbsValues = new NurbsValues(3);
            double[] controlPoints = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.ControlPoints = controlPoints;
            _nurbsGenerator.SpaceValues = nurbsValues;
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.KnotVector = knots;

            var result = _nurbsGenerator.ComputeCurvePoint(0.5, NurbsValuesDimensions.space);
            Assert.IsInstanceOf<double>(result);
            Assert.AreEqual(0.5, result);
        }

        [Test]
        public void ComputeCurvePoint_throws_ArgumentException()
        {
            var nurbsValues = new NurbsValues(3);
            double[] controlPoints = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.ControlPoints = controlPoints;
            _nurbsGenerator.SpaceValues = nurbsValues;
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.KnotVector = knots;

            Assert.Throws<ArgumentException>(() => _nurbsGenerator.ComputeCurvePoint(0.5, (NurbsValuesDimensions)Enum.GetValues(typeof(NurbsValuesDimensions)).Length + 1));
        }

        [Test]
        public void ComputePointOutOfSurface_returns_expected_result()
        {
            _nurbsGenerator = new NurbsGenerator();
            var nurbsValues = new NurbsValues(3);
            _nurbsGenerator.TimeValues = nurbsValues;
            _nurbsGenerator.SpaceValues = nurbsValues;
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 150.0, 25.0, 0.0},
                {0.0, 0.0, 0.0, 0.0, 0.0}};
            _nurbsGenerator.ControlPoints = controlPoints;
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.KnotVector = knots;
            nurbsValues.MaxValue = 0.5;

            var result = _nurbsGenerator.ComputePointOutOfSurface(0.5, 0.5, 0.1);
            Assert.IsInstanceOf<double>(result);
            Assert.AreEqual(0.1, result, 0.01);
        }

        [Test]
        public void BinarySearch_throws_exception()
        {
            var nurbsValues = new NurbsValues(3)
            {
                ValuesDimensions = (NurbsValuesDimensions)Enum.GetValues(typeof(NurbsValuesDimensions)).Length + 1
            };
            Assert.Throws<ArgumentException>(() => _nurbsGenerator.BinarySearch(nurbsValues, 0.1));
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
            _nurbsGenerator.ControlPoints = controlPoints;
            nurbsValues.ValuesDimensions = NurbsValuesDimensions.space;
            var result = _nurbsGenerator.FindSpan(nurbsValues, 1.0);
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
            _nurbsGenerator.ControlPoints = controlPoints;
            nurbsValues.ValuesDimensions = NurbsValuesDimensions.space;
            var result = _nurbsGenerator.FindSpan(nurbsValues, 1.0);
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
            _nurbsGenerator.ControlPoints = controlPoints;
            nurbsValues.ValuesDimensions = (NurbsValuesDimensions)Enum.GetValues(typeof(NurbsValuesDimensions)).Length + 1;
            Assert.Throws<ArgumentException>(() => _nurbsGenerator.FindSpan(nurbsValues, 1.0));
        }

        [Test]
        public void ComputePointOutOfSurface_time_space_greater_than_max_value()
        {
            _nurbsGenerator = new NurbsGenerator();
            var nurbsValues = new NurbsValues(3);
            _nurbsGenerator.GeneratorType = NurbsGeneratorType.SpatialFrequencyDomain;
            nurbsValues.MaxValue = 0.8;
            _nurbsGenerator.TimeValues = nurbsValues;
            _nurbsGenerator.SpaceValues = nurbsValues;
            nurbsValues.ValuesDimensions = NurbsValuesDimensions.space;
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 150.0, 25.0, 0.0},
                {0.0, 0.0, 0.0, 0.0, 0.0}};
            _nurbsGenerator.ControlPoints = controlPoints;
            double[] knots = { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0, 1.0 };
            nurbsValues.KnotVector = knots;
            var result = _nurbsGenerator.ComputePointOutOfSurface(1.0, 1.0, 0);
            Assert.AreEqual(0.0, result);
        }

        [Test]
        public void ComputePointOutOfSurface_not_spatial_frequency_domain()
        {
            _nurbsGenerator = new NurbsGenerator();
            var nurbsValues = new NurbsValues(3);
            _nurbsGenerator.GeneratorType = NurbsGeneratorType.RealDomain;
            nurbsValues.MaxValue = 6;
            _nurbsGenerator.TimeValues = nurbsValues;
            _nurbsGenerator.SpaceValues = nurbsValues;
            nurbsValues.ValuesDimensions = NurbsValuesDimensions.space;
            double[,] controlPoints = {{0.0, 0.0, 0.0, 0.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 50.0, 25.0, 0.0},
                {0.0 , 25.0, 150.0, 25.0, 0.0},
                {0.0, 0.0, 0.0, 0.0, 0.0}};
            _nurbsGenerator.ControlPoints = controlPoints;
            double[] knots = { 0, 0.1, 0.01, 0.02, 0.1, 0.2, 0.3, 0.4 };
            nurbsValues.KnotVector = knots;
            var result = _nurbsGenerator.ComputePointOutOfSurface(1.0, 7, 0);
            Assert.AreEqual(double.NaN, result);
        }

        [Test]
        public void ComputePointOutOfSurface_time_space_less_than_max_value()
        {
            var nurbsValues = new NurbsValues(3)
            {
                MaxValue = 1.2
            };
            _nurbsGenerator.TimeValues = nurbsValues;
            _nurbsGenerator.SpaceValues = nurbsValues;
            nurbsValues.ValuesDimensions = NurbsValuesDimensions.space;
            var result = _nurbsGenerator.ComputePointOutOfSurface(1.0, 1.0, 0);
            Assert.AreEqual(0.0, result);
        }

        /// <summary>
        /// Tear down for the NurbsGenerator tests.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            _nurbsGenerator = null;
        }
    }
}
