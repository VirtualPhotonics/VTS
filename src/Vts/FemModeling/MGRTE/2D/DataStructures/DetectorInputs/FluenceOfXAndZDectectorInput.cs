using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// DetectorInput for Flu(x,z)
    /// </summary>
    public class FluenceOfXAndZDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for fluence as a function of x and z detector input
        /// </summary>
        /// <param name="name">detector name</param>
        public FluenceOfXAndZDetectorInput(String name)
        {
            TallyType = TallyType.FluenceOfXAndZ;
            Name = name;
        }

        /// <summary>
        /// Default constructor uses default x and z bins
        /// </summary>
        public FluenceOfXAndZDetectorInput()
            : this(TallyType.FluenceOfXAndZ.ToString()) { }

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }

        /// <summary>
        /// detector name, defaults to TallyType.ToString() but can be user specified
        /// </summary>
        public String Name { get; set; }
    }
}
