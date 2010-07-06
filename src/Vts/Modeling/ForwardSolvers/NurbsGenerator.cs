using System;
using System.Collections.Generic;
using Vts.IO;
using Vts.Extensions;
using System.Linq;
using Vts.Common;
using MathNet.Numerics;

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
        /// Generator used for testing of teh methods of the class.
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
        public List<BSplinesCoefficients> TimeKnotSpanPolynomialCoefficients { get; set; }

        private double[] MinValidTimes { get; set; }

        private double[] Rhos { get; set; }

        #endregion properties

        #region fields
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
            //calculate polynomial coefficients of the basis functions along the time direction.
            TimeKnotSpanPolynomialCoefficients = new List<BSplinesCoefficients>();
            for (int knotindex = TimeValues.Degree; knotindex <= TimeValues.KnotVector.Length - 2 - TimeValues.Degree; knotindex++)
            {
                TimeKnotSpanPolynomialCoefficients.Add(new BSplinesCoefficients(TimeValues, knotindex));
            }
            if (GeneratorType == NurbsGeneratorType.RealDomain)
            {
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

            //double PI = Math.PI;
            //Func<double, double, double, double> nthOrderIntegralFunc;
            //Func<double, double, double, double> commonFunc = (t, m, f) =>
            //    {
            //        return coef * Math.Exp(-t * m) /
            //            (4.0 * f * f * PI * PI + m * m);
            //    };
        
            //switch (degree)
            //    {
            //        case 0:
            //            return nthOrderIntegralFunc = (t, m, f) =>
            //            {
            //                return commonFunc(t,m,f) *
            //                    (2.0 * f * PI * Math.Sin(2.0 * f * PI * t) 
            //                    - m * Math.Cos(2.0 * f * PI * t));
            //            };
            //        case 1:
            //            return nthOrderIntegralFunc = (t, m, f) =>
            //            {
            //                return commonFunc(t, m, f) 
            //                    / (4.0 * f * f * PI * PI + m * m) 
            //                    * (2.0 * f * PI * Math.Sin(2.0 * f * PI * t) 
            //                    * (4.0 * f * f * PI * PI * t + m * (2.0 + t * m)) 
            //                    - Math.Cos(2 * f * PI * t) * (4.0 * f * f * PI * PI 
            //                    * (t * m - 1.0)  + m * m * (1.0 + t * m)));
            //            };
            //        case 2:
            //            return nthOrderIntegralFunc = (t, m, f) =>
            //            {
            //                return commonFunc(t, m, f)  
            //                    / ((4.0 * f * f * PI * PI + m * m)
            //                    * (4.0 * f * f * PI * PI + m * m)) 
            //                    * (-(16.0 * f * f * f * f * PI * PI * PI * PI * t * (t * m - 2.0)
            //                    + 8.0 * f * f * PI * PI * m * (t * t * m * m - 3.0)
            //                    + m * m * m * (2.0 + t * m * (2.0 + t * m))) * Math.Cos(2.0 * f * t * PI)
            //                    + 2.0 * f * PI * (16.0 * f * f * f * f * PI * PI * PI * PI * t * t 
            //                    + 8.0 * f * f * PI * PI
            //                    * (-1.0 + t * m * (2.0 + t * m)) + m * m * (6.0 + t * m 
            //                    * (4.0 + t * m))) * Math.Sin(2.0 * f * t * PI));
            //            };
            //        case 3:
            //            return nthOrderIntegralFunc = (t, m, f) =>
            //            {
            //                return commonFunc(t, m, f)  
            //                    / ((4.0 * f * f * PI * PI + m * m)
            //                    * (4.0 * f * f * PI * PI + m * m)
            //                    * (4.0 * f * f * PI * PI + m * m))
            //                    * (-(64.0 * f * f * f * f * f * f * PI * PI * PI * PI * PI * PI * t * t
            //                    * (t * m - 3.0) + 48.0 * f * f * f * f * PI * PI * PI * PI
            //                    * (2.0 + t * m * (t * m - 3.0) * (2.0 + t * m))
            //                    + 12.0 * f * f * PI * PI * m * m * (-12.0 + t * m
            //                    * (-4.0 + t * m * (1.0 + t * m))) + m * m * m * m
            //                    * (6.0 + t * m * (6.0 + t * m * (3.0 + t * m))))
            //                    * Math.Cos(2.0 * f * PI * t) + 2.0 * f * PI
            //                    * (64.0 * f * f * f * f * f * f * PI * PI * PI * PI * PI * PI * t * t * t
            //                    + 48.0 * f * f * f * f * PI * PI * PI * PI * t
            //                    * (-2.0 + t * m * (2.0 + t * m)) + 12.0 * f * f * PI * PI * m
            //                    * (-8.0 + t * m * (2.0 + t * m) * (2.0 + t * m))
            //                    + m * m * m * (24.0 + t * m * (18.0 + t * m * (6.0 + t * m))))
            //                    * Math.Sin(2 * f * PI * t));
            //            };
            //        default:
            //            throw new ArgumentException("Degree is too high.");
            //    }
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

            //double PI = Math.PI;
            //Func<double, double, double, double> nthOrderIntegralFunc;
            //Func<double, double, double, double> commonFunc = (t, m, f) =>
            //{
            //    return coef * Math.Exp(-t * m) /
            //        (4.0 * f * f * PI * PI + m * m);
            //};

            //switch (degree)
            //{
            //    case 0:
            //        return nthOrderIntegralFunc = (t, m, f) =>
            //        {
            //            return commonFunc(t, m, f) *
            //                (2.0 * f * PI * Math.Cos(2.0 * f * PI * t) +
            //                 m * Math.Sin(2.0 * f * PI * t));
            //        };
            //    case 1:
            //        return nthOrderIntegralFunc = (t, m, f) =>
            //        {
            //            return commonFunc(t, m, f) / (4.0 * f * f * PI * PI + m * m)
            //                * (2.0 * f * PI * Math.Cos(2.0 * f * PI * t)
            //                * (4.0 * f * f * PI * PI * t + m * (2.0 + t * m))
            //                + Math.Sin(2 * f * PI * t) * (4.0 * f * f * PI * PI
            //                * (t * m - 1.0) + m * m * (1.0 + t * m)));
            //        };
            //    case 2:
            //        return nthOrderIntegralFunc = (t, m, f) =>
            //        {
            //            return commonFunc(t, m, f) / ((4.0 * f * f * PI * PI + m * m)
            //                * (4.0 * f * f * PI * PI + m * m))
            //                * (2.0 * f * PI * (16.0 * f * f * f * f * PI * PI * PI * PI * t * t
            //                + 8.0 * f * f * PI * PI * (-1.0 + t * m * (2.0 + t * m))
            //                + m * m * (6.0 + t * m * (4.0 + t * m))) * Math.Cos(2.0 * f * PI * t)
            //                + (16.0 * f * f * f * f * PI * PI * PI * PI * t * (t * m - 2.0)
            //                + 8.0 * f * f * PI * PI * m * (t * t * m * m - 3.0)
            //                + m * m * m * (2.0 + t * m * (2.0 + t * m)))
            //                * Math.Sin(2.0 * f * PI * t)); 
            //        };
            //    case 3:
            //        return nthOrderIntegralFunc = (t, m, f) =>
            //        {
            //            return commonFunc(t, m, f) / ((4.0 * f * f * PI * PI + m * m)
            //                * (4.0 * f * f * PI * PI + m * m)
            //                * (4.0 * f * f * PI * PI + m * m))
            //                * (2.0 * f * PI * (64.0 * f * f * f * f * f * f * PI * PI * PI * PI * PI * PI * t * t * t
            //                    + 48.0 * f * f * f * f * PI * PI * PI * PI * t
            //                    * (-2.0 + t * m * (2.0 + t * m)) + 12.0 * f * f * PI * PI * m
            //                    * (-8.0 + t * m * (2.0 + t * m) * (2.0 + t * m))
            //                    + m * m * m * (24.0 + t * m * (18.0 + t * m * (6.0 + t * m))))
            //                    * Math.Cos(2 * f * PI * t)
            //                    + (64.0 * f * f * f * f * f * f * PI * PI * PI * PI * PI * PI * t * t
            //                    * (t * m - 3.0) + 48.0 * f * f * f * f * PI * PI * PI * PI
            //                    * (2.0 + t * m * (t * m - 3.0) * (2.0 + t * m))
            //                    + 12.0 * f * f * PI * PI * m * m * (-12.0 + t * m
            //                    * (-4.0 + t * m * (1.0 + t * m))) + m * m * m * m
            //                    * (6.0 + t * m * (6.0 + t * m * (3.0 + t * m))))
            //                    * Math.Sin(2.0 * f * PI * t));
            //        };
            //    default:
            //        throw new ArgumentException("Degree is too high.");
            //}
        }

        /// <summary>
        /// Binary search used to determine the span index where the parametric point belongs.
        /// </summary>
        /// <param name="nurbsValues">NurbsValues class which contains the knot,degree and control points</param>
        /// <param name="parametricPoint">parametric point mapped in the interval 0-1</param>
        /// <returns>index of the knot span where the parametric point belongs</returns>
        /// <exception cref = System.ArgumentException>Thrown when the search has to be performed on missing dimension</exception> 
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
        /// <exception cref = System.ArgumentException>Thrown when the input parametric point is negative</exception> 
        public int FindSpan(NurbsValues nurbsValues, double parametricPoint)
        {
            if (parametricPoint < 0.0)
            {
                throw new ArgumentException("Negative parametric point not accetable as input.");
            }
            if (parametricPoint >= 1.0)
            {
                if (nurbsValues.ValuesDimensions == NurbsValuesDimensions.space)
                {
                    return ControlPoints.GetLength(0) - 1;
                }
                else
                {
                    return ControlPoints.GetLength(1) - 1;
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
        /// If the integral has to be performed to evaluate RofRho the algorithm checks if the
        /// lower time limit of the knot span is larger then the minimum time 
        /// of flight necessary to reach the detector.
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

            integralValue = IntegrateExponentialMultipliedByPolynomial(exponentialTerm,
                                                                       multipliedAndSummedPolynomialCoefs,
                                                                       lowerLimit,
                                                                       upperLimit);
            if (integralValue < 0.0)
            {
                //TODO throw exception if happens
                integralValue = 0.0;
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
        /// <exception cref = System.ArgumentException>Thrown if the degree of the function is too high.</exception> 
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
                            return -b * Math.Exp(-a * x) * (a * x * (a * x * (a * x + 3.0) + 6.0) + 6.0) / (a * a * a * a);
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
        /// <exception cref = System.ArgumentException>Thrown when the dimension is not valid</exception> 
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
        /// Calculates the integral value of an isoparametric NURBS curve analitically.
        /// </summary>
        /// <param name="space">radial position or spatial frequency mapped to the reference spatial value</param>
        /// <param name="exponentialTerm">exponential decay due to absorption</param>
        /// <returns>integral value of an isoparametric NURBS curve</returns>
        public double EvaluateNurbsCurveIntegral(double space, double exponentialTerm)
        {
            List<double[]> tensorProductControlPoints = EvaluateTensorProductControlPoints(space);

            var Value = tensorProductControlPoints.Zip(TimeKnotSpanPolynomialCoefficients,
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

            var Value = tensorProductControlPoints.Zip(TimeKnotSpanPolynomialCoefficients,
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
            return 0.98 * Vts.Common.Math.Interpolation.interp1(Rhos, MinValidTimes, rho);
        }

        #endregion INurbs Members
    }

    #region stub class for UnitTest

    /// <summary>
    /// Class used for Unit testing of the NurbsGenerator methods and of the NurbsForwardSolver
    /// class.
    /// </summary>
    public class StubNurbsGenerator : INurbs
    {
        #region INurbs Members

        /// <summary>
        /// Not implemented for stub class.
        /// </summary>
        /// <param name="rho"></param>
        /// <returns></returns>
        public double GetMinimumValidTime(double rho)
        {
            return rho / 214.0;
        }

        /// <summary>
        /// Returns always 1, used for testing.
        /// </summary>
        /// <param name="point">point coordinate</param>
        /// <param name="dimension">dimension</param>
        /// <returns>1</returns>
        public double ComputeCurvePoint(double point, NurbsValuesDimensions dimension)
        {
            return 1.0;
        }

        /// <summary>
        /// Returns always -1, used for testing.
        /// </summary>
        /// <param name="time">time coordinate</param>
        /// <param name="space">space coordinate</param>
        /// <returns>-1</returns>
        public double ComputeSurfacePoint(double time, double space)
        {
            return -1.0;
        }

        /// <summary>
        /// Gets Nurbs values for the time dimensions with max = 1.0
        /// </summary>
        public NurbsValues TimeValues
        {
            get
            {
                NurbsValues timeValues = new NurbsValues(1.0);
                return timeValues;
            }
        }

        /// <summary>
        /// Gets Nurbs values for the time dimensions with max = 1.0
        /// </summary>
        public NurbsValues SpaceValues
        {
            get
            {
                NurbsValues spaceValues = new NurbsValues(1.0);
                return spaceValues;
            }
        }

        /// <summary>
        /// Not implemented for stub class.
        /// </summary>
        /// <param name="space_ref"></param>
        /// <returns></returns>
        List<double[]> INurbs.EvaluateTensorProductControlPoints(double space_ref)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented for stub class.
        /// </summary>
        public List<BSplinesCoefficients> TimeKnotSpanPolynomialCoefficients
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Not implemented fo stub class.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="space"></param>
        /// <param name="edgeValue"></param>
        /// <returns></returns>
        public double ComputePointOutOfSurface(double time, double space, double edgeValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented for stub class.
        /// </summary>
        /// <param name="space"></param>
        /// <param name="exponentialTerm"></param>
        /// <returns></returns>
        public double EvaluateNurbsCurveIntegral(double space, double exponentialTerm)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented for stub class.
        /// </summary>
        /// <param name="space"></param>
        /// <param name="expTerm"></param>
        /// <param name="ft"></param>
        /// <returns></returns>
        public Complex EvaluateNurbsCurveFourierTransform(double space, double expTerm, double ft)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    #endregion stub class for UnitTest

    /// <summary>
    /// Defines the methods and properties that need to be implemented by the NurbsGenerator 
    /// class and by its stub version, StubNurbsGenerator used for Unit Testing.  
    /// </summary>
    public interface INurbs
    {
        /// <summary>
        /// Defines signature for a method used to consider cousality of the photon migration.
        /// </summary>
        /// <param name="rho">source detector separation</param>
        /// <returns>minimal valid time</returns>
        double GetMinimumValidTime(double rho);

        /// <summary>
        /// Defines the signature of the method used to compute a point on a NURBS curve.
        /// </summary>
        /// <param name="point">physical coordinate of the point</param>
        /// <param name="dimension">physical dimension represented by the NURBS curve</param>
        /// <returns>Value of a point on the curve</returns>
        double ComputeCurvePoint(double point, NurbsValuesDimensions dimension);

        /// <summary>
        /// Defines the signature of the method used to compute the point on a NURBS surface.
        /// </summary>
        /// <param name="time">time coordinate</param>
        /// <param name="space">space coordinate(rho or fx)</param>
        /// <returns>Value of a point  on the surface </returns>
        double ComputeSurfacePoint(double time, double space);

        /// <summary>
        /// Defines the signature of the method used to compute the point outside the surface range.
        /// </summary>
        /// <param name="time">time coordinate</param>
        /// <param name="space">space coordinate(rho or fx)</param>
        /// <param name="edgeValue">point calculated on a position on the limit of the surface</param>
        /// <returns>Value of a point outside the surface</returns>
        double ComputePointOutOfSurface(double time, double space, double edgeValue);

        /// <summary>
        /// Defines the signature of the method used to calculate the integral value of an
        /// isoparametric NURBS curve multiplied by an exponential function analitically.
        /// </summary>
        /// <param name="space">radial position or spatial frequency mapped to the reference spatial value</param>
        /// <param name="exponentialTerm">exponential decay due to absorption</param>
        /// <returns>integral value of an isoparametric NURBS curve</returns>
        double EvaluateNurbsCurveIntegral(double space, double exponentialTerm);

        /// <summary>
        /// Defines the signature for the method used to evaluate the FT of an isoparametric curve.
        /// </summary>
        /// <param name="space">spatial coordinate</param>
        /// <param name="expTerm">exponential coefficient</param>
        /// <param name="ft">temporal frequency</param>
        /// <returns>R(ft) at  fixed rho</returns>
        Complex EvaluateNurbsCurveFourierTransform(double space, double expTerm, double ft);

        /// <summary>
        /// Defines the signature of the method used to evaluate the tensor product control points
        /// necessary to evaluate the integral of an isoparametric curve.
        /// </summary>
        /// <param name="space_ref">spatial coordinate mapped to teh reference space</param>
        /// <returns>Tensor product control points for an isoparametric curve on a surface</returns>
        List<double[]> EvaluateTensorProductControlPoints(double space_ref);

        /// <summary>
        /// Gets the NurbsValues along the time dimension.
        /// </summary>
        NurbsValues TimeValues { get; }

        /// <summary>
        /// Gets the NurbsValues along the space dimension.
        /// </summary>
        NurbsValues SpaceValues { get; }

        /// <summary>
        /// Gets or sets the coefficients of the non vanishing B-splines over each knot span.
        /// </summary>
        List<BSplinesCoefficients> TimeKnotSpanPolynomialCoefficients { get; set; }

    }
}
