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
        /// The method determines the phase given real and imaginary values
        /// </summary>
        /// <param name="real">The real component</param>
        /// <param name="imaginary">The imaginary component</param>
        /// <returns>A phase double</returns>
        public static double ToPhase(double real, double imaginary)
        {
            // convert to degrees
            return -System.Math.Atan2(imaginary, real) * (180 / System.Math.PI );
        }

        /// <summary>
        /// The method determines the phase given real and imaginary arrays
        /// </summary>
        /// <param name="real">The real component array</param>
        /// <param name="imaginary">The imaginary component array</param>
        /// <returns>A phase double array</returns>
        public static double[] ToPhase(double[] real, double[] imaginary)
        {
            if (real.Length != imaginary.Length)
            {
                throw new ArgumentException("Error in ToPhase: real and imaginary arrays are not the same size!");
            }
            var phase = new double[real.Length];
            for (var i = 0; i < phase.Length; i++)
            {
                phase[i] = Convert.ToPhase(real[i], imaginary[i]);
            }
            return phase;
        }
        /// <summary>
        /// The method determines the amplitude given real and imaginary values
        /// </summary>
        /// <param name="real">The real component</param>
        /// <param name="imaginary">The imaginary component</param>
        /// <returns>An amplitude double</returns>
        public static double ToAmplitude(double real, double imaginary)
        {
            return System.Math.Sqrt(real*real+imaginary*imaginary);
        }

        /// <summary>
        /// The method determines the amplitude given real and imaginary arrays
        /// </summary>
        /// <param name="real">The real component array</param>
        /// <param name="imaginary">The imaginary component array</param>
        /// <returns>An amplitude double array</returns>
        public static double[] ToAmplitude(double[] real, double[] imaginary)
        {
            if (real.Length != imaginary.Length)
            {
                throw new ArgumentException("Error in ToAmplitude: real and imaginary arrays are not the same size!");       
            }
            var amplitude = new double[real.Length];
            for (var i = 0; i < amplitude.Length; i++)
            {
                amplitude[i] = Convert.ToAmplitude(real[i], imaginary[i]);
            }
            return amplitude;
        }
    }
}