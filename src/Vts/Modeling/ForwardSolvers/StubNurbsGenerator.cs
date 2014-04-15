using System;
using System.Collections.Generic;
using MathNet.Numerics;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// Class used for Unit testing of the NurbsGenerator methods and of the NurbsForwardSolver
    /// class.
    /// </summary>
    public class StubNurbsGenerator : INurbs
    {
        #region INurbs Members

        /// <summary>
        /// Returns minimum physical time of flight based on speed of light for n = 1,4.
        /// </summary>
        /// <param name="rho">source detector separation</param>
        /// <returns>t</returns>
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

        public double[] NativeTimes
        {
            get { throw new NotImplementedException(); }
        }

        List<double[]> INurbs.EvaluateTensorProductControlPoints(double space_ref)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
