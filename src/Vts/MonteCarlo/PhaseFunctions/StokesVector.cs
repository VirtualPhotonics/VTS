using System;
using Vts.Common;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctions
{
    public class StokesVector
    {
        public double S0 { get; set; }
        public double S1 { get; set; }
        public double S2 { get; set; }
        public double S3 { get; set; }

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
        /// <param name="incomingDirectionToModify">The input direction</param>
        public StokesVector()
        {
            S0 = 1;
            S1 = 0;
            S2 = 0;
            S3 = 0;
        }

        // The input Stokes vector is  (S0, S1, S2, S3)
        // Scattering reference frame defined such that (1,1,0,0) is horizontal and (1,-1,0,0) is vertical
        public void rotate(double theta, double phi, MuellerMatrix m)
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
