using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctions
{
    /// <summary>
    /// A Stokes Vector class that describes photon polarization.  See Van De Hulst's "Light Scattering by Small Particles" for more info.
    /// </summary>
    public class StokesVector
    {
        /// <summary>
        /// Constructor that initializes S0, S1, S2, S3 elements.
        /// </summary>
        /// <param name="s0">Input for S0.</param>
        /// <param name="s1">Input for S1.</param>
        /// <param name="s2">Input for S2.</param>
        /// <param name="s3">Input for S3.</param>
        public StokesVector(double s0, double s1, double s2, double s3)
        {
            S0 = s0;
            S1 = s1;
            S2 = s2;
            S3 = s3;
        }

        /// <summary>
        /// Default constructor that creates an unpolarized Stokes Vector.
        /// </summary>
        public StokesVector()
        {
            S0 = 1;
            S1 = 0;
            S2 = 0;
            S3 = 0;
        }
        /// <summary>
        /// 1st component of Stokes Vector
        /// </summary>
        public double S0 { get; set; }
        /// <summary>
        /// 2nd component of Stokes Vector
        /// </summary>
        public double S1 { get; set; }
        /// <summary>
        /// 3rd component of Stokes Vector
        /// </summary>
        public double S2 { get; set; }
        /// <summary>
        /// 4th component of Stokes Vector
        /// </summary>
        public double S3 { get; set; }

        /// <summary>
        /// Using Mueller Matrix m, rotate this Stokes vector by polar angle theta and azimuthal angle phi.  
        /// Scattering reference frame defined such that (1,1,0,0) is horizontal and (1,-1,0,0) is vertical
        /// </summary>
        /// <param name="theta">polar angle.</param>
        /// <param name="phi">azimuthal angle.</param>
        /// <param name="m">mueller matrix of the current region.</param>
        public void Rotate(double theta, double phi, MuellerMatrix m)
        {
            double cos2Psi, sin2Psi;

            cos2Psi = Math.Pow(Math.Cos(phi), 2) - Math.Pow(Math.Sin(phi), 2);
            sin2Psi = 2.0 * Math.Sin(phi) * Math.Cos(phi);
            StokesVector temp = new StokesVector(S0, S1 * cos2Psi + S2 * sin2Psi, -1 * S1 * sin2Psi + S2 * cos2Psi, S3);
            m.MultiplyByVector(temp, theta);
            //Mueller Matrix multiplication
            /* If Mie is used; i.e. spherical scatterers */
            S0 = 1;
            S1 = temp.S1 / temp.S0;
            S2 = temp.S2 / temp.S0;
            S3 = temp.S3 / temp.S0;
        }
    }
}
