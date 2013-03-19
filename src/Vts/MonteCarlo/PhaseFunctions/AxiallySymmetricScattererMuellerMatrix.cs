using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctions
{
    /// <summary>
    /// A Mueller matrix class for scatterers that are axially symmetric.  (Mainly for Mueller matrices generated from the T-Matrix method.)
    /// </summary>
    public class AxiallySymmetricScattererMuellerMatrix:MuellerMatrix
    {
        /// <summary>
        /// A constructor that initalizes the non-zero elements of the Mueller matrix for axially symmetric particles.
        /// Also initializes the Theta and MuellerMatrixType.
        /// </summary>
        public AxiallySymmetricScattererMuellerMatrix(double [] theta, double [] st11, double [] s12, double [] s22, double [] s33, double [] s34, double [] s44)
        {
            St11 = st11;
            S12 = s12;
            S22 = s22;
            S33 = s33;
            S34 = s34;
            S44 = s44;
            Theta = theta;
            MuellerMatrixType = MuellerMatrixType.TMatrix;
        }

        /// <summary>
        /// Multiplies this MuellerMatrix instance (evaluated at polar angle theta) by the StokesVector v and saves the product to v.
        /// </summary>
        /// <param name="v">Current Stokes vector of the photon.</param>
        /// <param name="theta">The polar angle.</param>
        public override void MultiplyByVector(StokesVector v, double theta)
        {
            int index = ReturnIndexAtThetaValue(theta);

            double S0sn, S1sn, S2sn, S3sn;
            S0sn = St11[index] * v.S0 + S12[index] * v.S1;
            S1sn = S12[index] * v.S0 + S22[index] * v.S1;
            S2sn = S33[index] * v.S2 + S34[index] * v.S3;
            S3sn = -1 * S34[index] * v.S2 + S44[index] * v.S3;

            v.S0 = S0sn;
            v.S1 = S1sn;
            v.S2 = S2sn;
            v.S3 = S3sn;
        }
    }
}
