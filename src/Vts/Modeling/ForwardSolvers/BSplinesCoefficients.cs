using System;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// This class holds the coefficients of the non vanishing B-splines
    /// over a single knot span and the upper and lower limit of the knot span
    /// mapped to the non parametric space.
    /// These values are used to evaluate analytically the steady state reflectance
    /// for the real domain and for the spatial frequency domain through the integration of 
    /// the time resolved curves at the required locations.
    /// The effect of the linear mapping from the original space to the parametric space
    /// is embedded within these coefficients.
    /// </summary>
    public class BSplinesCoefficients
    {
        #region fields and properties

        /// <summary>
        /// Gets or sets the coefficients of the non vanishing B-splines
        /// coefficients over a single knot span.
        /// </summary>
        public double[,] Coefficients { get; set; }

        /// <summary>
        /// Gets or sets the lower limit of the knot span,mapped to the non parametric space. 
        /// </summary>
        public double LowerLimit { get; set; }

        /// <summary>
        /// Gets or sets the upper limit of the knot span,mapped to the non parametric space.
        /// </summary>
        public double UpperLimit { get; set; }

        #endregion fields and properties

        #region constructor
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BSplinesCoefficients()
        {
 
        }

        /// <summary>
        /// Constructor used to evaluate the coefficients of the non vanishing
        /// B-splines over the knot span, and to evaluate the limits of the knot span,
        /// mapped to the non parametric space.
        /// </summary>
        /// <param name="nurbsValues">NurbsValues class with the NURBS characteristic values</param>
        /// <param name="knotIndex">Index of the lower limit knot</param>
        public BSplinesCoefficients(NurbsValues nurbsValues, int knotIndex)
        {
            LowerLimit = nurbsValues.KnotVector[knotIndex] * nurbsValues.MaxValue;
            UpperLimit = nurbsValues.KnotVector[knotIndex + 1] * nurbsValues.MaxValue;
            
            Coefficients = new double[nurbsValues.Degree + 1, nurbsValues.Degree + 2];
            if (nurbsValues.KnotVector[knotIndex] != nurbsValues.KnotVector[knotIndex + 1])
            {
                Coefficients[0, nurbsValues.Degree] = 1.0;
                double[,] tempCoefficients = Coefficients;
                double a = 0.0;
                double b = 0.0;
                double c = 0.0;
                double d = 0.0;

                for (int degree = 1; degree <= nurbsValues.Degree; degree++)
                {
                    tempCoefficients = new double[nurbsValues.Degree + 1, nurbsValues.Degree + 2];
                    for (int column = nurbsValues.Degree - degree; column <= nurbsValues.Degree; column++)
                    {
                        int j = knotIndex + (column - nurbsValues.Degree);
                        EvaluateMuliplyingValues(ref a, ref b, ref c, ref d, nurbsValues, degree, j);

                        for (int row = 0; row < degree; row++)
                        {
                            tempCoefficients[row, column] +=
                                Coefficients[row, column] * a + Coefficients[row, column + 1] * b;
                            tempCoefficients[row + 1, column] +=
                                Coefficients[row, column] * c + Coefficients[row, column + 1] * d;
                        }
                    }
                    Coefficients = tempCoefficients;
                }
                Coefficients = Normalize(Coefficients, nurbsValues.MaxValue);
            }
        }

        #endregion constructor

        #region methods
        /// <summary>
        /// Returns the middle time of a knot span mapped to the reference space.
        /// </summary>
        /// <returns>middle time coordinate</returns>
        public double GetKnotSpanMidTime()
        {
            return LowerLimit + (UpperLimit - LowerLimit) / 2.0;
        }

        /// <summary>
        /// Returns the time span of a knot span mapped to the reference space.
        /// </summary>
        /// <returns>delta time</returns>
        public double GetKnotSpanDeltaT()
        {
            return UpperLimit - LowerLimit;
        }

        /// <summary>
        /// If the input value is <see cref="double.NaN"/>, <see cref="double.PositiveInfinity"/>
        /// or <see cref="double.NegativeInfinity"/>, it changes it to zero, as defined in the
        /// recursive formula used to evaluate the B-splines coefficients. 'The NURBS Book' page 51.
        /// </summary>
        /// <param name="num">multiplying values of the recursive formula</param>
        /// <returns>0 or the input value</returns>
        public double ModifyIfNotValid(double num)
        {

            if (double.IsNaN(num) || double.IsInfinity(num))
            {
                num = 0.0;
            }
            return num;
        }

        /// <summary>
        /// Adjusts the B-splines coefficients to take into account the mapping from the
        /// original space to the parametric space, dividing by the maximum value of the 
        /// original space elevated to the same degree of each coefficient.
        /// </summary>
        /// <param name="coefficients">B-spline coefficients</param>
        /// <param name="max">max value of the original space</param>
        /// <returns>B-splines coefficients divided by the maximal value of the original space elevated to the same power of the coefficients</returns>
        private double[,] Normalize(double[,] coefficients, double max)
        {
            int numRows = coefficients.GetLength(0);
            int numColumns = coefficients.GetLength(1);

            for (int row = 1; row < numRows; row++)
            {
                for (int column = 0; column < numColumns; column++)
                {
                    coefficients[row, column] /= Math.Pow(max, row);
                }
            }
            return coefficients;
        } 
     
        /// <summary>
        /// Computes the coefficients of the B-spline recursive formula.
        /// Based on the Cox - De Boor algorithm. 'The NURBS Book' page 50.
        /// </summary>
        /// <param name="a">u_j / (u_j - u_(j+p))</param>
        /// <param name="b">u_(j+p+1) / (u_(j+p+1) - u_(j+1))</param>
        /// <param name="c">1 / (u_(j+p) - u_j)</param>
        /// <param name="d">1 / (u_(j+1) - u_(j+p+1))</param>
        /// <param name="nurbsValues">NurbsValues class with the knots vector and degree</param>
        /// <param name="degree">degree of the iteration of the recursive formula</param>
        /// <param name="j">knot index</param>
        public void EvaluateMuliplyingValues(ref double a,
                                             ref double b,
                                             ref double c,
                                             ref double d,
                                             NurbsValues nurbsValues,
                                             int degree,
                                             int j)
        {
            a = nurbsValues.KnotVector[j] / (nurbsValues.KnotVector[j] - nurbsValues.KnotVector[j + degree]);
            a = ModifyIfNotValid(a);
            b = nurbsValues.KnotVector[j + degree + 1] / (nurbsValues.KnotVector[j + degree + 1] - nurbsValues.KnotVector[j + 1]);
            b = ModifyIfNotValid(b);
            c = 1.0 / (nurbsValues.KnotVector[j + degree] - nurbsValues.KnotVector[j]);
            c = ModifyIfNotValid(c);
            d = 1.0 / (nurbsValues.KnotVector[j + 1] - nurbsValues.KnotVector[j + degree + 1]);
            d = ModifyIfNotValid(d);
 
        }
        #endregion methods
    }
}
