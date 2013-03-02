using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Vts.MonteCarlo.PhaseFunctions
{
    public class StokesVector
    {
        private double _s0 = 1;
        public double _s1 { get; set; }
        public double _s2 { get; set; }
        public double _s3 { get; set; }

        public StokesVector(double s1, double s2, double s3)
        {
            _s1 = s1;
            _s2 = s2;
            _s3 = s3;
        }

        /// <summary>
        /// Default constructor that creates an unpolarized Stokes Vector.
        /// </summary>
        /// <param name="incomingDirectionToModify">The input direction</param>
        public StokesVector()
        {
            _s1 = 0;
            _s2 = 0;
            _s3 = 0;
        }

        // The input Stokes vector is  (S0, S1, S2, S3)
        // Scattering reference frame defined such that (1,1,0,0) is horizontal and (1,-1,0,0) is vertical
        public void rotate(double theta, double phi, MuellerMatrix m)
        {
            double cos2Psi, sin2Psi, S0s, S1s, S2s, S3s, S0sn, S1sn, S2sn, S3sn;

            cos2Psi = Math.Pow(Math.Cos(phi), 2) - Math.Pow(Math.Sin(phi), 2);
            sin2Psi = 2.0 * Math.Sin(phi) * Math.Cos(phi);
            S0s = _s0; // Rotate the Stokes vector into the scattering plane
            S1s = _s1 * cos2Psi + _s2 * sin2Psi;
            S2s = -1 * _s1 * sin2Psi + _s2 * cos2Psi;
            S3s = _s3;

            int index = int(theta*4.0*180.0/Math.PI+0.5);
            //Mueller Matrix multiplication
            /* If Mie is used; i.e. spherical scatterers */
            S0sn = m.st11[index] * S0s + m.s12[index] * S1s;
            S1sn = m.s12[index] * S0s + m.st11[index] * S1s + m.s22[index] * S1s;
            S2sn = m.s33[index] * S2s + m.s34[index] * S3s;
            S3sn = -1 * m.s34[index] * S2s + m.s33[index] * S3s + m.s44[index] * S3s;

            _s1 = S1sn / S0sn;
            _s2 = S2sn / S0sn;
            _s3 = S3sn / S0sn;
        }
    }
}
