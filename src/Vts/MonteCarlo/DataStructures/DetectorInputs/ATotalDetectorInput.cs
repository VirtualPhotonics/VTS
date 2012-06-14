using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for total absorption
    /// </summary>
    public class ATotalDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for total absorption detector input
        /// </summary>
        /// <param name="name">detector name</param>
        public ATotalDetectorInput(String name)
        {
            TallyType = "ATotal";
            Name = name;
        }
        /// <summary>
        /// default constructor uses TallyType for name
        /// </summary>
        public ATotalDetectorInput() : this("ATotal") { }

        /// <summary>
        /// detector tally identifier
        /// </summary>
        public string TallyType { get; set; }
        /// <summary>
        /// detector name, defaults to TallyType.ToString() but can be user specified
        /// </summary>
        public string Name { get; set; }
    }
}
