using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctions
{
    public class MieScattererMuellerMatrix:MuellerMatrix
    {
        public double[] Theta { get; set; }

        //default constructor sets mueller matrix as linear vertical polarizer.
        public MieScattererMuellerMatrix(double [] st11, double [] s12, double [] s22, double [] s33, double [] s34, double [] s44)
        {
            St11 = st11;
            S12 = s12;
            S22 = s22;
            S33 = s33;
            S34 = s34;
            MuellerMatrixType = MuellerMatrixType.Mie;
        }

        public void MultiplyByVector(StokesVector v, double theta)
        {
            int index = ReturnIndexAtThetaValue(theta);

            double S0sn, S1sn, S2sn, S3sn;
            S0sn = St11[index] * v.S0 + S12[index] * v.S1;
            S1sn = S12[index] * v.S0 + St11[index] * v.S1;
            S2sn = S33[index] * v.S2 + S34[index] * v.S3;
            S3sn = -1 * S34[index] * v.S2 + S33[index] * v.S3;
            v.S0 = S0sn;
            v.S1 = S1sn;
            v.S2 = S2sn;
            v.S3 = S3sn;
        }

        /// <summary>
        /// Mueller Matrix type
        /// </summary>
        [IgnoreDataMember]
        public MuellerMatrixType MuellerMatrixType{ get; set; }
    }
}
