using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(fx)
    /// </summary>
    public class ROfFxDetectorInput : IDetectorInput
    {
        /// <summary>
        /// Returns an instance of ROfFxDetectorInput
        /// </summary>
        /// <param name="fx">spatial frequency sampling points (not binned)</param>
        /// <param name="name"></param>
        public ROfFxDetectorInput(DoubleRange fx, String name)
        {
            TallyType = "ROfFx";
            Name = name;
            Fx = fx;
        }
        /// <summary>
        /// constructor uses TallyType as name
        /// </summary>
        /// <param name="fx">spatial frequency sampling points (not binned)</param>
        public ROfFxDetectorInput(DoubleRange fx)
            : this(fx, "ROfFx") { }

        /// <summary>
        /// Default constructor uses default rho bins
        /// </summary>
        public ROfFxDetectorInput()
            : this(new DoubleRange(0.0, 0.2, 21), "ROfFx") { }

        /// <summary>
        ///  Detector tally type
        /// </summary>
        public string TallyType { get; set; }

        /// <summary>
        /// Detector name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Spatial frequency sampling points, fx (not binned)
        /// </summary>
        public DoubleRange Fx { get; set; }

    }
}
