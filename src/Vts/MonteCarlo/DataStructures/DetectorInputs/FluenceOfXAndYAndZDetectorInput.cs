using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for Flu(x,y,z)
    /// </summary>
    public class FluenceOfXAndYAndZDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for fluence as a function of x, y and z detector input
        /// </summary>
        /// <param name="x">x binning</param>
        /// <param name="y">y binning</param>
        /// <param name="z">z binning</param>
        /// <param name="name">detector name</param>
        public FluenceOfXAndYAndZDetectorInput(
            DoubleRange x, DoubleRange y, DoubleRange z, String name)
        {
            TallyType = TallyType.FluenceOfXAndYAndZ;
            Name = name;
            X = x;
            Y = y;
            Z = z;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="x">x binning</param>
        /// <param name="y">y binning</param>
        /// <param name="z">z binning</param>
        public FluenceOfXAndYAndZDetectorInput(
            DoubleRange x, DoubleRange y, DoubleRange z) 
            : this (x, y, z, TallyType.FluenceOfXAndYAndZ.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho and z bins
        /// </summary>
        public FluenceOfXAndYAndZDetectorInput() 
            : this(
                new DoubleRange(-10.0, 10.0, 101), // x
                new DoubleRange(-10.0, 10.0, 101), // y
                new DoubleRange(0.0, 10.0, 101), // z
                TallyType.FluenceOfXAndYAndZ.ToString()) {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// x binning
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// y binning
        /// </summary>
        public DoubleRange Y { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }
    }
}
