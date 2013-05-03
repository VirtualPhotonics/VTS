using System;

namespace Vts.Common.Math
{
    /// <summary>
    /// Conversion utilities
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// method determines the phase given real and imag values
        /// </summary>
        public static double ToPhase(double real, double imag)
        {
            // convert to degrees
            return -System.Math.Atan2(imag, real) * (180 / System.Math.PI );
        }

        /// <summary>
        /// method determines the phase given real and imag arrays
        /// </summary>
        public static double[] ToPhase(double[] real, double[] imag)
        {
            if (real.Length != imag.Length)
            {
                throw new ArgumentException("Error in ToPhase: real and imag arrays are not the same size!");
            }
            var phase = new double[real.Length];
            for (int i = 0; i < phase.Length; i++)
            {
                phase[i] = Convert.ToPhase(real[i], imag[i]);
            }
            return phase;
        }
        /// <summary>
        /// method determines the amplitude given real and imag values
        /// </summary>
        public static double ToAmplitude(double real, double imag)
        {
            return System.Math.Sqrt(real*real+imag*imag);
        }

        /// <summary>
        /// method determines the amplitude given real and imag arrays
        /// </summary>
        public static double[] ToAmplitude(double[] real, double[] imag)
        {
            if (real.Length != imag.Length)
            {
                throw new ArgumentException("Error in ToAmplitude: real and imag arrays are not the same size!");       
            }
            var amplitude = new double[real.Length];
            for (int i = 0; i < amplitude.Length; i++)
            {
                amplitude[i] = Convert.ToAmplitude(real[i], imag[i]);
            }
            return amplitude;
        }
    }
}