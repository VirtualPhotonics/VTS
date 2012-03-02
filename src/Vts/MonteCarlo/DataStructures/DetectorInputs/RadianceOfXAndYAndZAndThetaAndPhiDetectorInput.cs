using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for volume detector Radiance(x, y, z, theta, phi)
    /// </summary>
    public class RadianceOfXAndYAndZAndThetaAndPhiDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for radiance that passes x, y, z, theta, phi, name
        /// </summary>
        /// <param name="x">x binning</param>
        /// <param name="y">y binning</param>
        /// <param name="z">z binning</param>
        /// <param name="theta">theta binning (polar angle [0,pi])</param>
        /// <param name="phi">phi binning (azimuthal angle [0,2pi])</param>
        /// <param name="name">detector name</param>
        public RadianceOfXAndYAndZAndThetaAndPhiDetectorInput(DoubleRange x, DoubleRange y, DoubleRange z, DoubleRange theta, DoubleRange phi, String name)
        {
            TallyType = TallyType.RadianceOfXAndYAndZAndThetaAndPhi;
            Name = name;
            X = x;
            Y = y;
            Z = z;
            Theta = theta;
            Phi = phi;
        }
        /// <summary>
        /// Constructor for radiance(x, y, z, theta, phi) that doesn't pass name and uses TallyType.ToString() for name
        /// </summary>
        /// <param name="x">x binning</param>
        /// <param name="y">y binning</param>
        /// <param name="z">z binning</param>
        /// <param name="theta">theta binning</param>
        /// <param name="phi">phi binning</param>
        public RadianceOfXAndYAndZAndThetaAndPhiDetectorInput(DoubleRange x, DoubleRange y, DoubleRange z, DoubleRange theta, DoubleRange phi) 
            : this (x, y, z, theta, phi, TallyType.RadianceOfXAndYAndZAndThetaAndPhi.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho bins
        /// </summary>
        public RadianceOfXAndYAndZAndThetaAndPhiDetectorInput() 
            : this(new DoubleRange(-10, 10, 101), 
                new DoubleRange(-10, 10, 101),
                new DoubleRange(0, 10, 101),
                new DoubleRange(0.0, Math.PI, 2),  // theta goes from 0 to pi
                new DoubleRange(0.0, 2 * Math.PI, 2), // phi goes from 0 to 2pi
                TallyType.RadianceOfXAndYAndZAndThetaAndPhi.ToString()) {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, defaults to TallyType.ToString() but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// detector x binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// detector y binning
        /// </summary>
        public DoubleRange Y { get; set; }
        /// <summary>
        /// detector z binning
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// detector theta (polar angle [0,pi]) binning
        /// </summary>
        public DoubleRange Theta { get; set; }
        /// <summary>
        /// detector phi (azimuthal angle [0,2pi]) binning
        /// </summary>
        public DoubleRange Phi { get; set; }
    }
}
