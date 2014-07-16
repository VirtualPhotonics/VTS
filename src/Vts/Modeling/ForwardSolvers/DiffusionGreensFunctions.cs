using System;
using System.Numerics;
using MathNet.Numerics;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// This class contains Green's functions based on the infinite media diffusion case. 
    /// Source-sink(image) configurations are then composed for the semi-infinite half-space too.
    /// </summary>
    public static class DiffusionGreensFunctions
    {

        #region Infinite medium diffusion Greens Function : State Space
        /// <summary>
        /// Infinite Media Diffusion Greens Functions Definitions
        /// </summary>
        /// <param name="dp">diffusion parameters</param>
        /// <param name="r">radial value</param>
        /// <returns>evaluation of the Green's function</returns>
        public static double StationaryPointSourceGreensFunction(
            DiffusionParameters dp, double r)
        {
            return 1 / (4 * Math.PI * dp.D) * Math.Exp(-dp.mueff * r) / r;
        }
        /// <summary>
        /// Infinite media diffusion Green's functions for space and time for an
        /// origin of r = 0 and t = 0.
        /// </summary>
        /// <param name="dp">diffusion parameters object</param>
        /// <param name="r">radial location</param>
        /// <param name="t">time</param>
        /// <returns>evaluation of the Green's function</returns>
        public static double TemporalPointSourceGreensFunction(
            DiffusionParameters dp, double r, double t)
        {
            var tempVar = 4.0 * dp.D * dp.cn * t;
            return dp.cn * Math.Exp(-dp.mua * dp.cn * t) / Math.Pow(Math.PI * tempVar, 1.5) *
                Math.Exp(-r * r / tempVar);
        }
        /// <summary>
        /// Infinte media diffusion Green's function for space and temporal frequency
        /// </summary>
        /// <param name="dp">diffusion parameters object</param>
        /// <param name="r">radial location</param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Complex TemporalFrequencyPointSourceGreensFunction(
            DiffusionParameters dp, double r, Complex k)
        {
            return Math.Exp(-k.Real * r) / r * (
                Math.Cos(k.Imaginary * r) -
                Complex.ImaginaryOne * Math.Sin(k.Imaginary * r)
                ) / (4.0 * Math.PI * dp.D);
        }

        #endregion Infinite medium diffusion Greens Function : State Space


        #region Infinite media diffusion Green Function z-Flux component : State Space

        public static double StationaryPointSourceGreensFunctionZFlux(
            DiffusionParameters dp, double r, double zr)
        {
            return zr * (dp.mueff + 1 / r) * Math.Exp(-dp.mueff * r) / r / r /
                4 / Math.PI;
        }

        public static double TemporalPointSourceGreensFunctionZFlux(
            DiffusionParameters dp, double r, double zr, double t)
        {
            return 0.5 / Math.Pow(4.0 * Math.PI * dp.D * dp.cn, 1.5) / Math.Pow(t, 2.5) *
                Math.Exp(-dp.mua * dp.cn * t) * zr *
                Math.Exp(-r * r / (4.0 * dp.D * dp.cn * t));
        }

        public static Complex TemporalFrequencyPointSourceGreensFunctionZFlux(
            DiffusionParameters dp, double r, double zr, Complex k)
        {
            return zr * (k + 1 / r) * Math.Exp(-k.Real * r) / r / r *
                (Math.Cos(k.Imaginary * r) - Complex.ImaginaryOne * Math.Sin(k.Imaginary * r)) / 4 / Math.PI;

        }
        #endregion

        #region Infinite media Greens Functions for planar media via source image solution
        // Green's function composition
        public static double StationaryPointSourceImageGreensFunction(
           DiffusionParameters dp, double rSource, double rImage)
        {
            return DiffusionGreensFunctions.StationaryPointSourceGreensFunction(dp, rSource) -
                DiffusionGreensFunctions.StationaryPointSourceGreensFunction(dp, rImage);
        }

        public static double TemporalPointSourceImageGreensFunction(
           DiffusionParameters dp, double rSource, double rImage, double t)
        {
            return DiffusionGreensFunctions.TemporalPointSourceGreensFunction(dp, rSource, t) -
                DiffusionGreensFunctions.TemporalPointSourceGreensFunction(dp, rImage, t);
        }

        public static Complex TemporalFrequencyPointSourceImageGreensFunction(
            DiffusionParameters dp, double rSource, double rImage, Complex k)
        {

            return DiffusionGreensFunctions.TemporalFrequencyPointSourceGreensFunction(dp, rSource, k) -
                DiffusionGreensFunctions.TemporalFrequencyPointSourceGreensFunction(dp, rImage, k);
        }

        #endregion

        #region Infinite media Greens Functions z-Flux for planar media via source image solution

        // z-Flux of Green's functions in source-image configuration
        public static double StationaryPointSourceImageGreensFunctionZFlux(
            DiffusionParameters dp, double rSource, double zSource, double rImage, double zImage)
        {
            return StationaryPointSourceGreensFunctionZFlux(dp, rSource, zSource)
                - StationaryPointSourceGreensFunctionZFlux(dp, rImage, zImage);
        }
        // z-Flux of Green's functions in source-image configuration
        public static double TemporalPointSourceImageGreensFunctionZFlux(
            DiffusionParameters dp, double rSource, double zSource, double rImage, double zImage, double t)
        {
            return DiffusionGreensFunctions.TemporalPointSourceGreensFunctionZFlux(dp, rSource, zSource, t) -
                DiffusionGreensFunctions.TemporalPointSourceGreensFunctionZFlux(dp, rImage, zImage, t);
        }
        public static Complex TemporalFrequencyPointSourceImageGreensFunctionZFlux(
          DiffusionParameters dp, double rSource, double zSource, double rImage, double zImage, Complex k)
        {
            return DiffusionGreensFunctions.TemporalFrequencyPointSourceGreensFunctionZFlux(dp, rSource, zSource, k) -
                DiffusionGreensFunctions.TemporalFrequencyPointSourceGreensFunctionZFlux(dp, rImage, zImage, k);
        }

        #endregion
    }
}
