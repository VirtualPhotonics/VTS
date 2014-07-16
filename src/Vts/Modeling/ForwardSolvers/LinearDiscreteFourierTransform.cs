using System;
using System.Numerics;
using MathNet.Numerics;

namespace Vts.Modeling
{
    /// <summary>
    /// Class used to compute the discrete Fourier transform of a reflectance curve.
    /// </summary>
    public class LinearDiscreteFourierTransform 
    {
        public static double[] GetTime(double mua, double musp, out double dt)
        {
            double  tMaxFactor = 30;
            double inverseDtFactor = 30; // may want more time bins...
            dt = 1/(musp + mua)/inverseDtFactor;
            int Nt = (int)Math.Floor(tMaxFactor * inverseDtFactor);
            double[] rhoValues = new double[Nt];
            for (int i = 0; i < Nt; i++)
            {   
                rhoValues[i] = i * dt;
            }
            return rhoValues;
        }

        /// <summary>
        /// Calculate the Fourier Transform using a discrete Riemann middle sum with uniform dt
        /// </summary>
        /// <param name="time">vector of discrete time values</param>
        /// <param name="ROfTime">vector of discrete R(time) values</param>
        /// <param name="dt">delta time</param>
        /// <param name="ft">the temporal frequency at which to evaluate</param>
        /// <returns>ROfFt</returns>
        public static Complex GetFourierTransform(double[] time, double[] ROfTime, double dt, double ft)
        {
            if (time.Length != ROfTime.Length)
            {
                throw new Meta.Numerics.DimensionMismatchException();
            }
            Complex sum = 0.0;
            for (int i = 0; i < time.Length; i++)
            {
                sum += EvaluateDiscreteFourierTransform(time[i], ROfTime[i], dt, ft);
            }
            return sum;
        }

        /// <summary>
        /// Calculate the Fourier Transform using a discrete Riemann middle sum with non uniform dt
        /// </summary>
        /// <param name="time">vector of discrete time values</param>
        /// <param name="ROfTime">vector of discrete R(time) values</param>
        /// <param name="dt">delta time</param>
        /// <param name="ft">the temporal frequency at which to evaluate</param>
        /// <returns>ROfFt</returns>
        public static Complex GetFourierTransform(double[] time, double[] ROfTime, double[] dt, double ft)
        {
            if (time.Length != ROfTime.Length)
            {
                throw new Meta.Numerics.DimensionMismatchException();
            }
            Complex sum = 0.0;

            for (int i = 0; i < time.Length; i++)
            {
                sum += EvaluateDiscreteFourierTransform(time[i], ROfTime[i], dt[i], ft);
            }
            return sum;
        }

        private static Complex EvaluateDiscreteFourierTransform(double t, double R, double dt, double ft)
        {
            return R * (Math.Cos(2 * Math.PI * ft * t) -
                    Complex.ImaginaryOne * Math.Sin(2 * Math.PI * ft * t)) * dt;
        }

        // NOT SURE ABOUT following 2 methods, they haven't been checked out CKH
        /// <summary>
        /// Calculate the inverse Fourier Transform using a discrete Riemann middle sum with uniform frequencies
        /// </summary>
        /// <param name="frequencies">vector of discrete frequency values</param>
        /// <param name="ROfFt">vector of discrete R(ft) values</param>
        /// <param name="df">delta frequency</param>
        /// <param name="t">the time at which to evaluate</param>
        /// <returns>ROfTime</returns>
        public static Complex GetInverseFourierTransform(double[] frequencies, Complex[] ROfFt, double df, double t)
        {
            if (frequencies.Length != ROfFt.Length)
            {
                throw new Meta.Numerics.DimensionMismatchException();
            }
            Complex sum = 0.0;

            for (int i = 0; i < frequencies.Length; i++)
            {
                sum += EvaluateDiscreteInverseFourierTransform(frequencies[i], ROfFt[i], df, t);      
            }
            return sum;
        }

        private static Complex EvaluateDiscreteInverseFourierTransform(double ft, Complex R, double df, double t)
        {
            // dum is complex currently, I think it should have a 0 imag part, but not sure it will with this
            // algorithm, so take real part in end
             var dum = R * (Math.Cos(2 * Math.PI * ft * t) +
                    Complex.ImaginaryOne * Math.Sin(2 * Math.PI * ft * t)) * df;
            return dum;
        }
    }
}