using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctions
{
    /// <summary>
    /// A class that stores the values of a Mueller Matrix.  Elements of the Mueller Matrix may depend on the polar angle.
    /// </summary>
    /// <param name="theta">  An array storing all the polar angles st11, s12, s22, s33, and s34 were evaluated at.</param>
    /// <param name="st11">  The element in the first row and column of this Mueller Matrix.  </param>
    /// <param name="s12">  The element in the 1st row and 2nd column of this Mueller Matrix.  </param>
    /// <param name="s13">  The element in the 1st row and 3rd column of this Mueller Matrix.  </param>
    /// <param name="s14">  The element in the 1st row and 4th column of this Mueller Matrix.  </param>
    /// <param name="s21">  The element in the 2nd row and 1st column of this Mueller Matrix.  </param>
    /// <param name="s22">  The element in the 2nd row and 2nd column of this Mueller Matrix.  </param>
    /// <param name="s23">  The element in the 2nd row and 3rd column of this Mueller Matrix.  </param>
    /// <param name="s24">  The element in the 2nd row and 4th column of this Mueller Matrix.  </param>
    /// <param name="s31">  The element in the 3rd row and 1st column of this Mueller Matrix.  </param>
    /// <param name="s32">  The element in the 3rd row and 2nd column of this Mueller Matrix.  </param>
    /// <param name="s33">  The element in the 3rd row and 3rd column of this Mueller Matrix.  </param>
    /// <param name="s34">  The element in the 3rd row and 4th column of this Mueller Matrix.  </param>
    /// <param name="s41">  The element in the 4th row and 1st column of this Mueller Matrix.  </param>
    /// <param name="s42">  The element in the 4th row and 2nd column of this Mueller Matrix.  </param>
    /// <param name="s43">  The element in the 4th row and 3rd column of this Mueller Matrix.  </param>
    /// <param name="s44">  The element in the 4th row and 4th column of this Mueller Matrix.  </param>
    public abstract class MuellerMatrix
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

        /// <summary>
        /// Default constructor sets mueller matrix as linear vertical polarizer.
        /// </summary>
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

        /// <summary>
        /// An abstract function for subclasses to implement.  This function multiplies the Stokes vector, vectorToBeModified by this 
        /// Mueller Matrix evaluated at polar angle theta.  The function then saves the product of the two to vectorToBeModified.
        /// </summary>
        /// <param name="vectorToBeModified">  Describes the current polarization of the photon.  At the end of this function, this 
        /// vector will store the product of the original Stokes vector and the Mueller matrix.</param>
        /// <param name="theta">  A polar angle that this Mueller matrix will be evaluated at.</param>
        public abstract void MultiplyByVector(StokesVector vectorToBeModified, double theta);

        /// <summary>
        /// Does a binary search for thetaValue in the array Theta.  Returns the index of the array element closest to thetaValue.  Assumes that Theta is sorted.
        /// </summary>
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
