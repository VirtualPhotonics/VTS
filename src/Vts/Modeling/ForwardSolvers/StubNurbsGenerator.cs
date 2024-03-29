﻿using System;
using System.Collections.Generic;
using System.Numerics;
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
        /// <returns>Time of flight</returns>
        public double GetMinimumValidTime(double rho)
        {
            return rho / 214.0;
        }

        /// <summary>
        /// Returns always 1, used for testing.
        /// </summary>
        /// <param name="point">point coordinate</param>
        /// <param name="dimension">dimension</param>
        /// <returns>1.0</returns>
        public double ComputeCurvePoint(double point, NurbsValuesDimensions dimension)
        {
            return 1.0;
        }

        /// <summary>
        /// Returns always -1, used for testing.
        /// </summary>
        /// <param name="time">time coordinate</param>
        /// <param name="space">space coordinate</param>
        /// <returns>-1.0</returns>
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
        /// Not implemented for stub class.
        /// </summary>
        /// <param name="time">time coordinate</param>
        /// <param name="space">space coordinate</param>
        /// <param name="edgeValue">edge value</param>
        /// <returns>Point out of surface</returns>
        public double ComputePointOutOfSurface(double time, double space, double edgeValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented for stub class.
        /// </summary>
        /// <param name="space">space coordinate</param>
        /// <param name="exponentialTerm">exponential term</param>
        /// <returns>NURBS curve integral</returns>
        public double EvaluateNurbsCurveIntegral(double space, double exponentialTerm)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Not implemented for stub class.
        /// </summary>
        /// <param name="space">space coordinate</param>
        /// <param name="expTerm">exponential term</param>
        /// <param name="ft">temporal-frequency</param>
        /// <returns>NURBS curve Fourier transform</returns>
        public Complex EvaluateNurbsCurveFourierTransform(double space, double expTerm, double ft)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Native times
        /// </summary>
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
