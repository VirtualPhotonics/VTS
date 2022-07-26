using System;

namespace Vts.Common.Math
{
    /// <summary>
    /// The <see cref="Math"/> namespace contains the math classes for the Virtual Tissue Simulator
    /// </summary>

    [System.Runtime.CompilerServices.CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Conversion utilities
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// method determines the phase given real and imag values
        /// </summary>
        /// <param name="real">real component</param>
        /// <param name="imag">imaginery component</param>
        /// <returns>phase double</returns>
        public static double ToPhase(double real, double imag)
        {
            // convert to degrees
            return -System.Math.Atan2(imag, real) * (180 / System.Math.PI );
        }

        /// <summary>
        /// method determines the phase given real and imag arrays
        /// </summary>
        /// <param name="real">real component array</param>
        /// <param name="imag">imaginary component array</param>
        /// <returns>phase double array</returns>
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
        /// <param name="real">real component</param>
        /// <param name="imag">imaginery component</param>
        /// <returns>amplitude double</returns>
        public static double ToAmplitude(double real, double imag)
        {
            return System.Math.Sqrt(real*real+imag*imag);
        }

        /// <summary>
        /// method determines the amplitude given real and imag arrays
        /// </summary>
        /// <param name="real">real component array</param>
        /// <param name="imag">imaginary component array</param>
        /// <returns>amplitude double array</returns>
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