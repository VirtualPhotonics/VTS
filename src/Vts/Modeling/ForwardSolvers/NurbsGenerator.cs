using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using Vts.IO;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// Defines the nurbs surface physical domain.
    /// </summary>
    public enum NurbsGeneratorType
    {
        /// <summary>
        /// Radial and temporal generator.
        /// </summary>
        RealDomain,
        /// <summary>
        /// Spatial frequency and temporal generator.
        /// </summary>
        SpatialFrequencyDomain,
        /// <summary>
        /// Generator used for testing of the methods of the class.
        /// </summary>
        Stub,
    }

    /// <summary>
    /// Class that contains the reference NURBS values.
    /// Its methods are used to evaluate the value of a point 
    /// on the NURBS surface or curve using B-splines interpolation,
    /// and to evaluate the integral of a NURBS curve.
    /// </summary>
    public class NurbsGenerator : INurbs
    {

        # region properties

        /// <summary>
        /// Gets or sets the physical domain rapresented by the NURBS surface.
        /// </summary>
        public NurbsGeneratorType GeneratorType { get; set; }

        /// <summary>
        /// Gets or sets the NurbsValues specific to the time dimension.
        /// </summary>
        public NurbsValues TimeValues { get; set; }

        /// <summary>
        /// Gets or sets the NurbsValues specific to the space dimension(rho or fx).
        /// </summary>
        public NurbsValues SpaceValues { get; set; }

        /// <summary>
        /// Gets or sets the NURBS control points.
        /// </summary>
        public double[,] ControlPoints { get; set; }

        /// <summary>
        /// List of the non vanishing Bsplines coefficients for each knot span.
        /// </summary>
        public List<BSplinesCoefficients> TimeKnotSpanPolynomialCoefficients { get; set; }// for each knot span there is a polynomial function [p(x) = ax^(n) + bx^(n-1) + .... + z], these coefficients are evaluated at instatiation time to improve the analytical integration efficiency.

        /// <summary>
        /// temporal coordinate of the native reference points (t_l)
        /// </summary>
        public double[] NativeTimes { get; set; }

        private double[] MinValidTimes { get; set; }// loaded only for real domain reference: for each rho (Rhos) we have measured the time of flight of the first collecte photon, t_0(r_k). If t < t_0 return 0. 

        private double[] Rhos { get; set; }// loaded only for real domain reference: native radial coordinates of real domain reference used for minimum time of flight interpolation.

        #endregion properties

        #region fields

        private double _minExponentialTerm;

        private static string _folderPath = "Modeling/Resources/ReferenceNurbs/";
        
        private static string _folder = "/v0p1";
        
        #endregion fields

        #region constructor

        /// <summary>
        /// Class constructor which loads the reference values from resources
        /// based on the generator type.
        /// </summary>
        /// <param name="generatorType">NURBS surface physical domain</param>
        public NurbsGenerator(NurbsGeneratorType generatorType)
        {
            GeneratorType = generatorType;
            // Load binary files based on generator type
            TimeValues = (NurbsValues)FileIO.ReadFromXMLInResources<NurbsValues>(_folderPath + generatorType.ToString() + _folder + @"/timeNurbsValues.xml", "Vts");
            SpaceValues = (NurbsValues)FileIO.ReadFromXMLInResources<NurbsValues>(_folderPath + generatorType.ToString() + _folder + @"/spaceNurbsValues.xml", "Vts");
            // Get the dimensions of the control point matrix
            int[] dims = { SpaceValues.KnotVector.Length - SpaceValues.Degree - 1, TimeValues.KnotVector.Length - TimeValues.Degree - 1 };
            // Load control points
            ControlPoints = (double[,])FileIO.ReadArrayFromBinaryInResources<double>
                             (_folderPath + generatorType.ToString() + _folder + @"/controlPoints", "Vts", dims);
            NativeTimes = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                             (_folderPath + generatorType.ToString() + _folder + @"/nativeTimes", "Vts", dims[1]);
            //calculate polynomial coefficients of the basis functions along the time direction.
            TimeKnotSpanPolynomialCoefficients = new List<BSplinesCoefficients>();
            for (int knotindex = TimeValues.Degree; knotindex <= TimeValues.KnotVector.Length - 2 - TimeValues.Degree; knotindex++)
            {
                TimeKnotSpanPolynomialCoefficients.Add(new BSplinesCoefficients(TimeValues, knotindex));
            }
            if (GeneratorType == NurbsGeneratorType.RealDomain)
            {
                _minExponentialTerm = 0.001 * NurbsForwardSolver.v;
                MinValidTimes = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                             (_folderPath + generatorType.ToString() + _folder + @"/minValidTimes", "Vts", dims[0]);
                Rhos = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                             (_folderPath + generatorType.ToString() + _folder + @"/rhos", "Vts", dims[0]);
            }
        }

        /// <summary>
        /// Default constructor,used for testing. 
        /// </summary>
        public NurbsGenerator()
        {
            ControlPoints = new double[4, 4];
        }

        #endregion constructor

        #region Class methods

        private Complex EvaluateKnotSpanFourierTransform(double exponentialTerm,
                                                        double[,] polynomialCoefs,
                                                        double[] controlPoints,
                                                        double lowerLimit,
                                                        double upperLimit,
                                                        double space, double ft )
        {
            if (GeneratorType == NurbsGeneratorType.RealDomain)
            {
                double minTimeOfFlight = GetMinimumValidTime(space);
                if (upperLimit < minTimeOfFlight)
                {
                    return new Complex(0,0);
                }
                else if (upperLimit > minTimeOfFlight && lowerLimit < minTimeOfFlight)
                {
                    lowerLimit = minTimeOfFlight;
                }
            }

            double[] multipliedAndSummedPolynomialCoefs = MultiplyControlPointsAndPolynomialCoefficients
                                                                 (polynomialCoefs, controlPoints);

            double real = IntegrateRealPart(exponentialTerm,multipliedAndSummedPolynomialCoefs,
                                            lowerLimit, upperLimit, ft);
            double imaginary = IntegrateImaginaryPart(exponentialTerm, multipliedAndSummedPolynomialCoefs,
                                                      lowerLimit, upperLimit, ft);

            return new Complex(real, imaginary);
        }

        private double IntegrateImaginaryPart(double expTerm, double[] polynomialCoefficients, double lowerLimit, double upperLimit, double ft)
        {
            double integralValue = 0.0;
            int maxDegree = polynomialCoefficients.Length - 1;
            for (int coefficientDegree = 0; coefficientDegree <= maxDegree; coefficientDegree++)
            {
                integralValue += IntegrateImaginaryPartOfOrderN(coefficientDegree, expTerm,
                                                                polynomialCoefficients[coefficientDegree],
                                                                lowerLimit, upperLimit, ft);
            }
            return integralValue;
        }

        private double IntegrateRealPart(double expTerm, double[] polynomialCoefficients, double lowerLimit, double upperLimit, double ft)
        {
            double integralValue = 0.0;
            int maxDegree = polynomialCoefficients.Length - 1;
            for (int coefficientDegree = 0; coefficientDegree <= maxDegree; coefficientDegree++)
            {
                integralValue += IntegrateRealPartOfOrderN(coefficientDegree,expTerm,
                                                            polynomialCoefficients[coefficientDegree],
                                                            lowerLimit, upperLimit, ft);
            }
            return integralValue;
        }

        private double IntegrateRealPartOfOrderN(int degree, double expTerm, double coefficient, double lowerLimit, double upperLimit, double ft)
        {
            Func<double, double, double, double> nthOrderIntegralFunc = GetRealFunction(degree, coefficient);
            return nthOrderIntegralFunc(upperLimit, expTerm, ft) -
                   nthOrderIntegralFunc(lowerLimit, expTerm, ft);
        }

        private double IntegrateImaginaryPartOfOrderN(int degree, double expTerm, double coefficient, double lowerLimit, double upperLimit, double ft)
        {

            Func<double, double, double, double> nthOrderIntegralFunc = GetImaginaryFunction(degree, coefficient);
            return nthOrderIntegralFunc(upperLimit, expTerm, ft) -
                   nthOrderIntegralFunc(lowerLimit, expTerm, ft);
        }

        private Func<double, double, double, double> GetRealFunction(int degree, double coef)
        {
            Func<double, double, double, double> nthOrderIntegralFunc;
            Func<double, double, double> commonFunc = (t, m) =>
            {
                return coef * Math.Exp(-t * m);
            };

            switch (degree)
            {
                case 0:
                    return nthOrderIntegralFunc = (t, m, f) =>
                    {
                        double P = 2.0 * Math.PI * f;
                        double D = (P * P + m * m);
                        return commonFunc(t, m) / D
                            * (P * Math.Sin(P * t)
                            - m * Math.Cos(P * t));
                    };
                case 1:
                    return nthOrderIntegralFunc = (t, m, f) =>
                    {
                        double P = 2.0 * Math.PI * f;
                        double D = (P * P + m * m);
                        double tm = t * m;
                        return commonFunc(t, m)/ (D * D)
                            * (P * Math.Sin(P * t)
                            * (P * P * t + m * (2.0 + tm))
                            - Math.Cos(P * t) * (P * P
                            * (tm - 1.0) + m * m * (1.0 + tm)));
                    };
                case 2:
                    return nthOrderIntegralFunc = (t, m, f) =>
                    {
                        double P = 2.0 * Math.PI * f;
                        double P2 = P * P;
                        double tm = t * m;
                        double D = (P * P + m * m);
                        return commonFunc(t, m) / (D * D * D)
                            * (-(P2 * P2 * t * (tm - 2.0)
                            + 2.0 * P2 * m * (tm * tm - 3.0)
                            + m * m * m * (2.0 + tm * (2.0 + tm))) * Math.Cos(P * t)
                            + P * (P2 * P2 * t * t
                            + 2.0 * P2
                            * (-1.0 + tm * (2.0 + tm)) + m * m * (6.0 + tm
                            * (4.0 + tm))) * Math.Sin(P * t));
                    };
                case 3:
                    return nthOrderIntegralFunc = (t, m, f) =>
                    {
                        double P = 2.0 * Math.PI * f;
                        double P2 = P * P;
                        double tm = t * m;
                        double D = (P * P + m * m);
                        return commonFunc(t, m)/ (D * D * D * D)
                            * (-(P2 * P2 * P2 * t * t
                            * (tm - 3.0) + 3.0 * P2 * P2
                            * (2.0 + tm * (tm - 3.0) * (2.0 + tm))
                            + 3.0 * P2 * m * m * (-12.0 + tm
                            * (-4.0 + tm * (1.0 + tm))) + m * m * m * m
                            * (6.0 + tm * (6.0 + tm * (3.0 + tm))))
                            * Math.Cos(P * t) + P
                            * (P2 * P2 * P2 * t * t * t
                            + 3.0 * P2 * P2 * t
                            * (-2.0 + tm * (2.0 + tm)) + 3.0 * P2 * m
                            * (-8.0 + tm * (2.0 + tm) * (2.0 + tm))
                            + m * m * m * (24.0 + tm * (18.0 + tm * (6.0 + tm))))
                            * Math.Sin(P * t));
                    };
                default:
                    throw new ArgumentException("Degree is too high.");
            }
        }

        private Func<double, double, double, double> GetImaginaryFunction(int degree, double coef)
        {
            Func<double, double, double, double> nthOrderIntegralFunc;
            Func<double, double, double> commonFunc = (t, m) =>
            {
                return coef * Math.Exp(-t * m);
            };

            switch (degree)
            {
                case 0:
                    return nthOrderIntegralFunc = (t, m, f) =>
                    {
                        double P = 2.0 * Math.PI * f;
                        double D = (P * P + m * m);

                        return commonFunc(t, m) / D
                            * (P * Math.Cos(P * t) +
                             m * Math.Sin(P * t));
                    };
                case 1:
                    return nthOrderIntegralFunc = (t, m, f) =>
                    {
                        double P = 2.0 * Math.PI * f;
                        double P2 = P * P;
                        double tm = t * m;
                        double D = (P * P + m * m);

                        return commonFunc(t, m) / (D * D)
                            * (P * Math.Cos(P * t)
                            * (P2 * t + m * (2.0 + tm))
                            + Math.Sin(P * t)
                            * (P2 * (tm - 1.0) + m * m * (1.0 + tm)));
                    };
                case 2:
                    return nthOrderIntegralFunc = (t, m, f) =>
                    {
                        double P = 2.0 * Math.PI * f;
                        double P2 = P * P;
                        double tm = t * m;
                        double D = (P * P + m * m);

                        return commonFunc(t, m) / (D * D * D)
                            * (P * (P2 * P2 * t * t
                            + 2.0 * P2 * (-1.0 + tm * (2.0 + tm))
                            + m * m * (6.0 + tm * (4.0 + tm))) * Math.Cos(P * t)
                            + (P2 * P2 * t * (tm - 2.0)
                            + 2.0 * P2 * m * (tm * tm - 3.0)
                            + m * m * m * (2.0 + tm * (2.0 + tm)))
                            * Math.Sin(P * t));
                    };
                case 3:
                    return nthOrderIntegralFunc = (t, m, f) =>
                    {
                        double P = 2.0 * Math.PI * f;
                        double P2 = P * P;
                        double tm = t * m;
                        double D = (P * P + m * m);

                        return commonFunc(t, m) / (D * D * D * D)
                            * (P * (P2 * P2 * P2 * t * t * t
                                + 3.0 * P2 * P2 * t
                                * (-2.0 + tm * (2.0 + tm)) + 3.0 * P2 * m
                                * (-8.0 + tm * (2.0 + tm) * (2.0 + tm))
                                + m * m * m * (24.0 + tm * (18.0 + tm * (6.0 + tm))))
                                * Math.Cos(P * t)
                                + (P2 * P2 * P2 * t * t
                                * (tm - 3.0) + 3.0 * P2 * P2
                                * (2.0 + tm * (tm - 3.0) * (2.0 + tm))
                                + 3.0 * P2 * m * m * (-12.0 + tm
                                * (-4.0 + tm * (1.0 + tm))) + m * m * m * m
                                * (6.0 + tm * (6.0 + tm * (3.0 + tm))))
                                * Math.Sin(P * t));
                    };
                default:
                    throw new ArgumentException("Degree is too high.");
            }
        }

        /// <summary>
        /// Binary search used to determine the span index where the parametric point belongs.
        /// </summary>
        /// <param name="nurbsValues">NurbsValues class which contains the knot,degree and control points</param>
        /// <param name="parametricPoint">parametric point mapped in the interval 0-1</param>
        /// <returns>index of the knot span where the parametric point belongs</returns>
        /// <exception cref="System.ArgumentException">Thrown when the search has to be performed on missing dimension</exception> 
        public int BinarySearch(NurbsValues nurbsValues, double parametricPoint)
        {
            int low = nurbsValues.Degree;
            int high;
            if (nurbsValues.ValuesDimensions == NurbsValuesDimensions.space)
            {
                high = ControlPoints.GetLength(0);
            }
            else if (nurbsValues.ValuesDimensions == NurbsValuesDimensions.time)
            {
                high = ControlPoints.GetLength(1);
            }
            else
            {
                throw new ArgumentException("unknown dimension.");
            }

            int mid = (low + high) / 2;

            while (parametricPoint < nurbsValues.KnotVector[mid] || parametricPoint >= nurbsValues.KnotVector[mid + 1])
            {
                if (parametricPoint < nurbsValues.KnotVector[mid])
                    high = mid;
                else
                    low = mid;
                mid = (low + high) / 2;
            }
            return (mid);
        }

        /// <summary>
        /// Computes the non vanishing basis functions on the specific knot span.
        /// Algorithm 2.2 from 'The NURBS Book' page 70.
        /// </summary>
        /// <param name="spanIndex">index of the knot span where the parametric point belongs</param>
        /// <param name="parametricPoint">parametric variable</param>
        /// <param name="nurbsValues">NurbsValues class which contains the knot,degree and control points</param>
        /// <returns>array with the value of the non vanishing basis functions evaluated at the parametric point</returns>
        public double[] EvaluateBasisFunctions(int spanIndex, double parametricPoint, NurbsValues nurbsValues)
        {
            if (parametricPoint > 1.0)
                parametricPoint = 1.0;

            double[] basisFunctions = new double[nurbsValues.Degree + 1];
            double[] left = new double[nurbsValues.Degree + 1];
            double[] right = new double[nurbsValues.Degree + 1];
            double temp, saved;

            basisFunctions[0] = 1.0;
            for (int j = 1; j <= nurbsValues.Degree; j++)
            {
                left[j] = parametricPoint - nurbsValues.KnotVector[spanIndex + 1 - j];
                right[j] = nurbsValues.KnotVector[spanIndex + j] - parametricPoint;
                saved = 0.0;
                for (int i = 0; i < j; i++)
                {
                    temp = basisFunctions[i] / (right[i + 1] + left[j - i]);
                    basisFunctions[i] = saved + right[i + 1] * temp;
                    saved = left[j - i] * temp;
                }
                basisFunctions[j] = saved;
            }
            return basisFunctions;
        }

        /// <summary>
        /// Calculates the value of a point on a NURBS curve.
        /// </summary>
        /// <param name="spanIndex">index of the knot span where the parametric point belongs</param>
        /// <param name="basisFunctions">array with the value of the non vanishing basis functions evaluated at a parametric point inside the knot span</param>
        /// <param name="nurbsValues">NurbsValues class which contains the knot,degree and control points</param>
        /// <returns>value of a point on a NURBS curve</returns>
        public double EvaluateCurvePoint(int spanIndex, double[] basisFunctions, NurbsValues nurbsValues)
        {
            double outputValue = 0.0;
            for (int i = 0; i <= nurbsValues.Degree; i++)
            {
                outputValue += basisFunctions[i] * nurbsValues.ControlPoints[spanIndex - nurbsValues.Degree + i];
            }
            return outputValue;
        }

        /// <summary>
        /// Calculate the value of a point on a NURBS surface.
        /// </summary>
        /// <param name="timeSpanIndex">knot span index along the temporal dimension</param>
        /// <param name="timeBasisFunctions">array with the value of the non vanishing basis functions along the temporal coordinate</param>
        /// <param name="spaceSpanIndex">knot span index along the spatial dimension</param>
        /// <param name="spaceBasisFunctions">array with the value of the non vanishing basis functions along the spatial coordinate(rho or fx)</param>
        /// <returns>value of a point on a NURBS surface</returns>
        public double EvaluateSurfacePoint(int timeSpanIndex, double[] timeBasisFunctions, int spaceSpanIndex, double[] spaceBasisFunctions)
        {
            double pointValue = 0.0;
            double[] temp = new double[TimeValues.Degree + 1];
            for (int j = 0; j <= TimeValues.Degree; j++)
            {
                temp[j] = 0.0;
                for (int i = 0; i <= SpaceValues.Degree; i++)
                {
                    temp[j] = temp[j] + spaceBasisFunctions[i] *
                        ControlPoints[spaceSpanIndex - SpaceValues.Degree + i, timeSpanIndex - TimeValues.Degree + j];
                }
            }
            for (int j = 0; j <= TimeValues.Degree; j++)
            {
                pointValue = pointValue + timeBasisFunctions[j] * temp[j];
            }
            return pointValue;
        }

        /// <summary>
        /// Searches for the knot span where the parametric point lies, with a classic 
        /// binary search.If the point is larger then the last knot element returns the last span index
        /// to evaluate the value of a  point on the edge of the surface, which is later used for derivative
        /// extrapolation.
        /// Adapted from algorithm 2.1 from 'The NURBS Book' page 68.
        /// </summary>
        /// <param name="nurbsValues">NurbsValues class which contains the knot,degree and control points</param>
        /// <param name="parametricPoint">parametric point</param>
        /// <returns>knot span where the parametric point belongs</returns>
        /// <exception cref ="System.ArgumentException">Thrown when the input parametric point is negative</exception> 
        public int FindSpan(NurbsValues nurbsValues, double parametricPoint)
        {
            if (parametricPoint < 0.0)
            {
                throw new ArgumentException("Negative parametric point not acceptable as input.");
            }
            if (parametricPoint >= 1.0)
            {
                if (nurbsValues.ValuesDimensions == NurbsValuesDimensions.space)
                {
                    return ControlPoints.GetLength(0) - 1;
                }
                else if (nurbsValues.ValuesDimensions == NurbsValuesDimensions.time)
                {
                    return ControlPoints.GetLength(1) - 1;
                }
                else
                {
                    throw new ArgumentException("Invalid NurbsValuesDimensions.");
                }
            }
            else
                return BinarySearch(nurbsValues, parametricPoint);
        }

        /// <summary>
        /// Multiplies each polynomial coefficient with its corresponding control point.
        /// 'The NURBS Book' page 81.
        /// </summary>
        /// <param name="polynomialCoefs">polynmial coefficients</param>
        /// <param name="controlPoints">control point of the isoparamentric Nurbs curve</param>
        /// <returns>polynomial coefficients multiplied by the respective control point</returns>
        public double[] MultiplyControlPointsAndPolynomialCoefficients(double[,] polynomialCoefs,
                                                                         double[] controlPoints)
        {
            double[] multipliedPolynomialCoefs = new double[polynomialCoefs.GetLength(0)];
            int numRows = polynomialCoefs.GetLength(0);
            for (int row = 0; row < numRows; row++)
            {
                for (int column = 0; column < controlPoints.Length; column++)
                {
                    multipliedPolynomialCoefs[row] += polynomialCoefs[row, column] * controlPoints[column];
                }
            }
            return multipliedPolynomialCoefs;
        }
       
        /// <summary>
        /// Returns the integral of a NURBS curve over a single knotspan.
        /// If the integral has to be performed to evaluate ROfRho the algorithm checks if the
        /// lower time limit of the knot span is larger then the minimum time 
        /// of flight necessary to reach the detector. The integration is analytical
        /// only for the real domain and only if the time span is larger then a threshold value
        /// and only if the value of the exponential decay due to absorption is larger then a
        /// threshold value.
        /// </summary>
        /// <param name="exponentialTerm"> exponential decay due to absorption</param>
        /// <param name="polynomialCoefs">polynomial coefficients of the non null B-splines curves over the knot span</param>
        /// <param name="controlPoints">tensor product control points of the isoparametric curve</param>
        /// <param name="lowerLimit">knot span lower limit mapped to the physical value of the parametrized variable</param>
        /// <param name="upperLimit">knot span upper limit mapped to the physical value of the parametrized variable</param>
        /// <param name="space">spatial coordiante mapped to the refernce space</param>
        /// <returns>integral value of a NURBS curve over a single knotspan</returns>
        public double EvaluateKnotSpanIntegralValue(double exponentialTerm,
                                                    double[,] polynomialCoefs,
                                                    double[] controlPoints,
                                                    double lowerLimit,
                                                    double upperLimit,
                                                    double space)
        {
            double integralValue = 0.0;

            //check for causality
            if (GeneratorType == NurbsGeneratorType.RealDomain)
            {
                double minTimeOfFlight = GetMinimumValidTime(space);
                if (upperLimit < minTimeOfFlight)
                {
                    return integralValue;
                }
                else if (upperLimit > minTimeOfFlight && lowerLimit < minTimeOfFlight)
                {
                    lowerLimit = minTimeOfFlight;
                }
            }

            double[] multipliedAndSummedPolynomialCoefs = MultiplyControlPointsAndPolynomialCoefficients
                                                                 (polynomialCoefs, controlPoints);
            double deltaT = upperLimit - lowerLimit;
            //analytical integration: performed only for real domain and only for bins larger than 0.01 ns
            if ((deltaT > 0.01 &&  exponentialTerm >= _minExponentialTerm) && (GeneratorType == NurbsGeneratorType.RealDomain || GeneratorType == NurbsGeneratorType.Stub))
            {
                integralValue = IntegrateExponentialMultipliedByPolynomial(exponentialTerm,
                                                                       multipliedAndSummedPolynomialCoefs,
                                                                       lowerLimit,
                                                                       upperLimit);
            }
            //discrete integration: for each knot span (s) evaluates function (f(u)) at the span middle point (u) and evaluates the area as A = s*f(u)
            else
            {
                double t = lowerLimit + deltaT/2.0;
                for (int i = 0; i < multipliedAndSummedPolynomialCoefs.Length; i++)
                {
                    integralValue += multipliedAndSummedPolynomialCoefs[i] * Math.Pow(t, i);
                }
                integralValue *= Math.Exp(-exponentialTerm * t) * deltaT;
            }
            
            return integralValue;
        }

        /// <summary>
        /// Evaluates for each term of the polynomial the
        /// integral value of its multiplication with the
        /// exponential decay within the lower and upper 
        /// limit.
        /// </summary>
        /// <param name="exponentialTerm">exponential term</param>
        /// <param name="polynomialCoefficients">polynomial coefficients sorted in degree</param>
        /// <param name="lowerLimit">lower integration limit</param>
        /// <param name="upperLimit">upper integration limit</param>
        /// <returns>integral of the polynomial multiplied by an exponential over the specified range</returns>
        public double IntegrateExponentialMultipliedByPolynomial(double exponentialTerm,
                                                                 double[] polynomialCoefficients,
                                                                 double lowerLimit,
                                                                 double upperLimit)
        {
            double integralValue = 0.0;
            int maxDegree = polynomialCoefficients.Length - 1;
            for (int coefficientDegree = 0; coefficientDegree <= maxDegree; coefficientDegree++)
            {
                double value = IntegrateExponentialMultipliedByMomomial(coefficientDegree,
                                                                          exponentialTerm,
                                                                          polynomialCoefficients[coefficientDegree],
                                                                          lowerLimit, upperLimit);
                value = 0;
                integralValue += IntegrateExponentialMultipliedByMomomial(coefficientDegree,
                                                                          exponentialTerm,
                                                                          polynomialCoefficients[coefficientDegree],
                                                                          lowerLimit, upperLimit);
                integralValue += value;
            }
            return integralValue;
        }

        /// <summary>
        /// Evaluates the integral value of the multiplication 
        /// between a monomial function and an exponential 
        /// function within the lower and upper limit.
        /// </summary>
        /// <param name="degree">degree of the monomial term</param>
        /// <param name="exponentialTerm">exonential term</param>
        /// <param name="monomialCoefficient">monomial coefficient</param>
        /// <param name="lowerLimit">lower limit of integration</param>
        /// <param name="upperLimit">upper limit of ntegration</param>
        /// <returns>integral value over the range</returns>
        public double IntegrateExponentialMultipliedByMomomial(int degree,
                                                               double exponentialTerm,
                                                               double monomialCoefficient,
                                                               double lowerLimit,
                                                               double upperLimit)
        {
            Func<double, double, double, double> nthOrderIntegralFunc = GetIntegralFunction(degree, exponentialTerm);
            return nthOrderIntegralFunc(exponentialTerm, monomialCoefficient, upperLimit) -
                   nthOrderIntegralFunc(exponentialTerm, monomialCoefficient, lowerLimit);
        }

        /// <summary>
        /// According to the degree of the monomial term and to the value of the exponential
        /// term returns the analytical integral function.
        /// </summary>
        /// <param name="degree">degree of the monomial coefficient</param>
        /// <param name="exponentialTerm">exponential decay</param>
        /// <returns>integral function</returns>
        /// <exception cref="System.ArgumentException">Thrown if the degree of the function is too high.</exception> 
        public Func<double, double, double, double> GetIntegralFunction(int degree, double exponentialTerm)
        {
            Func<double, double, double, double> nthOrderIntegralFunc;

            if (exponentialTerm == 0.0)
            {
                return nthOrderIntegralFunc = (a, b, x) =>
                {
                    return b * Math.Pow(x, (Double)degree + 1.0) / ((Double)degree + 1.0);
                };
            }
            else
            {
                switch (degree)
                {
                    case 0:
                        return nthOrderIntegralFunc = (a, b, x) =>
                        {
                            return -b * Math.Exp(-a * x) / (a);
                        };
                    case 1:
                        return nthOrderIntegralFunc = (a, b, x) =>
                        {
                            return -b * Math.Exp(-a * x) * (a * x + 1.0) / (a * a);
                        };
                    case 2:
                        return nthOrderIntegralFunc = (a, b, x) =>
                        {
                            return -b * Math.Exp(-a * x) * (a * x * (a * x + 2.0) + 2.0) / (a * a * a);
                        };
                    case 3:
                        return nthOrderIntegralFunc = (a, b, x) =>
                        {
                            return - b * Math.Exp(-a * x) * (a * x * (a * x * (a * x + 3.0) + 6.0) + 6.0) / (a * a * a * a);
                        };
                    default:
                        throw new ArgumentException("Degree is too high.");
                }
            }
        }

        #endregion Class methods

        #region INurbs Members

        /// <summary>
        /// Evaluates a point on a NURBS curve.
        /// </summary>
        /// <param name="point">coordinate of the point on the NURBS curve, which is mapped to the parametric space</param>
        /// <param name="dimension">spatial(rho or fx) or temporal dimension identifier</param>
        /// <returns>value of the NURBS curve at a specific location</returns>
        /// <exception cref="System.ArgumentException">Thrown when the dimension is not valid</exception> 
        public double ComputeCurvePoint(double point, NurbsValuesDimensions dimension)
        {
            NurbsValues nurbsValues;
            if (dimension == NurbsValuesDimensions.time)
                nurbsValues = TimeValues;
            else if (dimension == NurbsValuesDimensions.space)
                nurbsValues = SpaceValues;
            else
                throw new ArgumentException("non valid NurbsValuesDimensions enum");

            int spanIndex = FindSpan(nurbsValues, point / nurbsValues.MaxValue);
            double[] basisFunctions = EvaluateBasisFunctions(spanIndex, point / nurbsValues.MaxValue, nurbsValues);
            return EvaluateCurvePoint(spanIndex, basisFunctions, nurbsValues);
        }

        /// <summary>
        /// Evaluates the value of a point on a NURBS surface.
        /// </summary>
        /// <param name="time">time point</param>
        /// <param name="space">space point (rho or fx)</param>
        /// <returns>value of a point, identified by its spatial and temporal coordinates, on a NURBS surface</returns>
        public double ComputeSurfacePoint(double time, double space)
        {
            int timeSpanIndex = FindSpan(TimeValues, time / TimeValues.MaxValue);
            double[] timeBasisFunctions = EvaluateBasisFunctions(timeSpanIndex, time / TimeValues.MaxValue, TimeValues);
            int spaceSpanIndex = FindSpan(SpaceValues, space / SpaceValues.MaxValue);
            double[] spaceBasisFunctions = EvaluateBasisFunctions(spaceSpanIndex, space / SpaceValues.MaxValue, SpaceValues);
            return EvaluateSurfacePoint(timeSpanIndex, timeBasisFunctions, spaceSpanIndex, spaceBasisFunctions);
        }

        /// <summary>
        /// Evaluates the value of a point out of the reference nurbs surface using the 
        /// derivative of the surfece using the derivative generators for the specific domain.
        /// </summary>
        /// <param name="time">time value mapped to the reference time</param>
        /// <param name="space">spatial value (rho or fx) mapped to the reference spatial value</param>
        /// <param name="edgeValue">reflectance value on the boundary 'edge' of the reference range </param>
        /// <returns>scaled reflectance value out of reference domain for the specific solution domain</returns>
        public double ComputePointOutOfSurface(double time, double space, double edgeValue)
        {
            double slope, intercept;

            if (time > TimeValues.MaxValue)
            {
                double deltaT = 0.01;//ns
                slope = (Math.Log10(edgeValue) -
                         Math.Log10(ComputeSurfacePoint(TimeValues.MaxValue - deltaT, space))) /
                         (deltaT);
                intercept = -slope * TimeValues.MaxValue + Math.Log10(ComputeSurfacePoint(TimeValues.MaxValue, space));
                edgeValue = Math.Pow(10.0, slope * time + intercept);
            }
            if (space > SpaceValues.MaxValue)
            {
                if (GeneratorType != NurbsGeneratorType.SpatialFrequencyDomain)
                {
                    double deltaRho = 5;//mm
                    slope = (Math.Log10(edgeValue) -
                             Math.Log10(ComputeSurfacePoint(time, SpaceValues.MaxValue - deltaRho))) /
                             (deltaRho);
                    intercept = Math.Log10(edgeValue) - slope * SpaceValues.MaxValue;
                    edgeValue = Math.Pow(10.0, slope * space + intercept);
                }
                else
                {
                    edgeValue = 0.0;
                }
            }
            return edgeValue;
        }

        /// <summary>
        /// Calculates the integral value of an isoparametric NURBS curve.
        /// </summary>
        /// <param name="space">radial position or spatial frequency mapped to the reference spatial value</param>
        /// <param name="exponentialTerm">exponential decay due to absorption</param>
        /// <returns>integral value of an isoparametric NURBS curve</returns>
        public double EvaluateNurbsCurveIntegral(double space, double exponentialTerm)
        {
            List<double[]> tensorProductControlPoints = EvaluateTensorProductControlPoints(space);

            var Value = System.Linq.Enumerable.Zip( 
                tensorProductControlPoints,
                TimeKnotSpanPolynomialCoefficients,
                (controlPoints, Bsplines) =>
                EvaluateKnotSpanIntegralValue(exponentialTerm,
                                            Bsplines.Coefficients,
                                            controlPoints,
                                            Bsplines.LowerLimit,
                                            Bsplines.UpperLimit,
                                            space));
            return Value.Sum();
        }

        /// <summary>
        /// Evaluates the Fourier transform of an isoparametric Nurbs curve analitically.
        /// </summary>
        /// <param name="space">spatial coordinate</param>
        /// <param name="expTerm">exponential coefficients</param>
        /// <param name="ft">temporal frequency</param>
        /// <returns>R(ft) at fixed rho</returns>
        public Complex EvaluateNurbsCurveFourierTransform(double space, double expTerm, double ft)
        {
            double real = 0.0;
            double imaginary = 0.0;
            List<double[]> tensorProductControlPoints = EvaluateTensorProductControlPoints(space);

            var Value = Enumerable.Zip(
                tensorProductControlPoints,
                TimeKnotSpanPolynomialCoefficients,
                (controlPoints, Bsplines) =>
                EvaluateKnotSpanFourierTransform(expTerm,
                                            Bsplines.Coefficients,
                                            controlPoints,
                                            Bsplines.LowerLimit,
                                            Bsplines.UpperLimit,
                                            space,ft));
            for (int i = 0; i < Value.Count(); i++)
            {
                real += Value.ElementAt(i).Real;
                imaginary += Value.ElementAt(i).Imaginary;

            }
            return new Complex(real,imaginary);
        }

        /// <summary>
        /// Evaluates the tensor product control points of an isoparametric curve on 
        /// a NURBS surface. The curve has to be isoparametric in the space dimension.
        /// </summary>
        /// <param name="space_ref">spatial coordinate mapped to the reference domain</param>
        /// <returns>control points effective on a specific isoparametric curve along the time direction</returns>
        public List<double[]> EvaluateTensorProductControlPoints(double space_ref)
        {
            List<double[]> tensorProductControlPoints = new List<double[]>();
            double[] tempTensorProductControlPoints = new double[ControlPoints.GetLength(1)];
            int spaceSpanIndex = FindSpan(SpaceValues, space_ref / SpaceValues.MaxValue);
            double[] spaceBasisFunctions = EvaluateBasisFunctions(spaceSpanIndex, space_ref / SpaceValues.MaxValue, SpaceValues);

            for (int column = 0; column <= tempTensorProductControlPoints.Length - 1; column++)
            {
                for (int i = 0; i <= SpaceValues.Degree; i++)
                {
                    tempTensorProductControlPoints[column] += spaceBasisFunctions[i] * ControlPoints[spaceSpanIndex - SpaceValues.Degree + i, column];
                }
            }

            for (int knotSpan = TimeValues.Degree; knotSpan <= TimeValues.KnotVector.Length - TimeValues.Degree - 2; knotSpan++)
            {
                double[] knotSpanControlPoints = new double[TimeValues.Degree + 2];
                for (int j = knotSpanControlPoints.Length - 2; j >= 0; j--)
                {
                    knotSpanControlPoints[j] = tempTensorProductControlPoints[knotSpan + (j - TimeValues.Degree)];
                }
                tensorProductControlPoints.Add(knotSpanControlPoints);
            }
            return tensorProductControlPoints;
        }

        /// <summary>
        /// Returns the first valid time of a R(t) curve.
        /// </summary>
        /// <param name="rho">source detector distance</param>
        /// <returns>first valid time</returns>
        public double GetMinimumValidTime(double rho)
        {
            return Vts.Common.Math.Interpolation.interp1(Rhos, MinValidTimes, rho);
        }

        #endregion INurbs Members

    }
}
