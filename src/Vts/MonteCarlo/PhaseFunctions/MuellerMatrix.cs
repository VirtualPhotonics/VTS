using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctions
{
    public class MuellerMatrix
    {
        public double[] St11 { get; set; }
        public double[] S12 { get; set; }
        public double[] S13 { get; set; }
        public double[] S14 { get; set; }
        public double[] S21 { get; set; }
        public double[] S22 { get; set; }
        public double[] S23 { get; set; }
        public double[] S24 { get; set; }
        public double[] S31 { get; set; }
        public double[] S32 { get; set; }
        public double[] S33 { get; set; }
        public double[] S34 { get; set; }
        public double[] S41 { get; set; }
        public double[] S42 { get; set; }
        public double[] S43 { get; set; }
        public double[] S44 { get; set; }
        public double[] Theta { get; set; }
        //default constructor sets mueller matrix as linear vertical polarizer.
        public MuellerMatrix()
        {
            St11 = new double[1];
            S12 = new double[1];
            S13 = new double[1];
            S14 = new double[1];
            S21 = new double[1];
            S22 = new double[1];
            S23 = new double[1];
            S24 = new double[1];
            S31 = new double[1];
            S32 = new double[1];
            S33 = new double[1];
            S34 = new double[1];
            S41 = new double[1];
            S42 = new double[1];
            S43 = new double[1];
            S44 = new double[1];
            Theta = new double[1];
            St11[0] = 0.5;
            S12[0] = -0.5;
            S13[0] = 0;
            S14[0] = 0;
            S21[0] = -0.5;
            S22[0] = 0.5;
            S23[0] = 0;
            S24[0] = 0;
            S31[0] = 0;
            S32[0] = 0;
            S33[0] = 0;
            S34[0] = 0;
            S41[0] = 0;
            S42[0] = 0;
            S43[0] = 0;
            S44[0] = 0;
            Theta[0] = 0;
            MuellerMatrixType = MuellerMatrixType.LinearVerticalPolarizer;
        }

        public void MultiplyByVector(StokesVector v, double theta)
        { 
            //TODO
        }


        //does a binary search for thetavalue in the array theta.
        public int ReturnIndexAtThetaValue(double thetaValue)
        {
            int imin = 0;
            int imax = Theta.Length-1;
            int imid = 0;
            while (imax > imin)
            {
                /* calculate the midpoint for roughly equal partition */
                imid = (int)(imin+ imax)/2;

                // determine which subarray to search
                if (Theta[imid] < thetaValue)
                    // change min index to search upper subarray
                    imin = imid + 1;
                else if (Theta[imid] > thetaValue)
                    // change max index to search lower subarray
                    imax = imid - 1;
                else
                    // key found at index imid
                    return imid;
            }
            return 0;
        }
        /// <summary>
        /// Mueller Matrix type
        /// </summary>
        [IgnoreDataMember]
        public MuellerMatrixType MuellerMatrixType{ get; set; }
    }
}
