using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctions
{
    /// <summary>
    /// A class that stores the values of a Mueller Matrix.  Elements of the Mueller Matrix may depend on the polar angle.
    /// </summary>
    /// <param name="Theta">  An array storing all the polar angles st11, s12, s22, s33, and s34 were evaluated at.</param>
    /// <param name="St11">  The element in the first row and column of this Mueller Matrix.  </param>
    /// <param name="S12">  The element in the 1st row and 2nd column of this Mueller Matrix.  </param>
    /// <param name="S13">  The element in the 1st row and 3rd column of this Mueller Matrix.  </param>
    /// <param name="S14">  The element in the 1st row and 4th column of this Mueller Matrix.  </param>
    /// <param name="S21">  The element in the 2nd row and 1st column of this Mueller Matrix.  </param>
    /// <param name="S22">  The element in the 2nd row and 2nd column of this Mueller Matrix.  </param>
    /// <param name="S23">  The element in the 2nd row and 3rd column of this Mueller Matrix.  </param>
    /// <param name="S24">  The element in the 2nd row and 4th column of this Mueller Matrix.  </param>
    /// <param name="S31">  The element in the 3rd row and 1st column of this Mueller Matrix.  </param>
    /// <param name="S32">  The element in the 3rd row and 2nd column of this Mueller Matrix.  </param>
    /// <param name="S33">  The element in the 3rd row and 3rd column of this Mueller Matrix.  </param>
    /// <param name="S34">  The element in the 3rd row and 4th column of this Mueller Matrix.  </param>
    /// <param name="S41">  The element in the 4th row and 1st column of this Mueller Matrix.  </param>
    /// <param name="S42">  The element in the 4th row and 2nd column of this Mueller Matrix.  </param>
    /// <param name="S43">  The element in the 4th row and 3rd column of this Mueller Matrix.  </param>
    /// <param name="S44">  The element in the 4th row and 4th column of this Mueller Matrix.  </param>
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
        public List<double>Theta { get; set; }

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
            Theta = new List<double>();
            Theta.Add(0.0);
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
            MuellerMatrixType = MuellerMatrixType.LinearVerticalPolarizer;
        }

        /// <summary>
        /// Constructor for mueller matrix.
        /// </summary>
        public MuellerMatrix(List<double> theta, double[] st11, double[] s12, double[] s13, double[] s14, double[] s21, double[] s22, double[] s23, double[] s24, double[] s31, double[] s32, double[] s33, double[] s34, double[] s41, double[] s42, double[] s43, double[] s44)
        {
            Theta = theta;
            St11 = st11;
            S12 = s12;
            S13 = s13;
            S14 = s14;
            S21 = s21;
            S22 = s22;
            S23 = s23;
            S24 = s24;
            S31 = s31;
            S32 = s32;
            S33 = s33;
            S34 = s34;
            S41 = s41;
            S42 = s42;
            S43 = s43;
            S44 = s44;
            MuellerMatrixType = MuellerMatrixType.General;
        }

        /// <summary>
        /// An abstract function for subclasses to implement.  This function multiplies the Stokes vector, vectorToBeModified by this 
        /// Mueller Matrix evaluated at polar angle theta.  The function then saves the product of the two to vectorToBeModified.
        /// </summary>
        /// <param name="vectorToBeModified">  Describes the current polarization of the photon.  At the end of this function, this 
        /// vector will store the product of the original Stokes vector and the Mueller matrix.</param>
        /// <param name="theta">  A polar angle that this Mueller matrix will be evaluated at.</param>
        public virtual void MultiplyByVector(StokesVector vectorToBeModified, double theta)
        {
            double S0, S1, S2, S3;
            int thetaIndex = ReturnIndexAtThetaValue(theta);
            S0 = vectorToBeModified.S0 * St11[thetaIndex] + vectorToBeModified.S1 * S12[thetaIndex] + vectorToBeModified.S2 * S13[thetaIndex] + vectorToBeModified.S3 * S14[thetaIndex];
            S1 = vectorToBeModified.S0 * S21[thetaIndex] + vectorToBeModified.S1 * S22[thetaIndex] + vectorToBeModified.S2 * S23[thetaIndex] + vectorToBeModified.S3 * S24[thetaIndex];
            S2 = vectorToBeModified.S0 * S31[thetaIndex] + vectorToBeModified.S1 * S32[thetaIndex] + vectorToBeModified.S2 * S33[thetaIndex] + vectorToBeModified.S3 * S34[thetaIndex];
            S3 = vectorToBeModified.S0 * S41[thetaIndex] + vectorToBeModified.S1 * S42[thetaIndex] + vectorToBeModified.S2 * S43[thetaIndex] + vectorToBeModified.S3 * S44[thetaIndex];
            vectorToBeModified.S0 = S0;
            vectorToBeModified.S1 = S1;
            vectorToBeModified.S2 = S2;
            vectorToBeModified.S3 = S3;
        }

        /// <summary>
        /// Does a binary search for thetaValue in the array Theta.  Returns the index of the array element closest to thetaValue.  Assumes that Theta is sorted.
        /// </summary>
        public int ReturnIndexAtThetaValue(double thetaValue)
        {
            if (thetaValue > Theta[Theta.Count - 1])
                return Theta.Count - 1;
            if (thetaValue < Theta[0])
                return 0;
            //edge detection.  probably should throw an exception here.
            int index = Theta.BinarySearch(thetaValue);
            if (index >= 0)
                return index;
            else
            {
                //compare insertion point and neighbor.
                if (Math.Abs(thetaValue - Theta[~index]) < Math.Abs(thetaValue - Theta[~index - 1]))
                    return ~index;
                return ~index - 1;
            }
        }
        /// <summary>
        /// Mueller Matrix type
        /// </summary>
        [IgnoreDataMember]
        public MuellerMatrixType MuellerMatrixType{ get; set; }
    }
}