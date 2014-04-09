using System;

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
            TallyType = TallyType.ATotal;
            Name = name;
        }
        /// <summary>
        /// default constructor uses TallyType for name
        /// </summary>
        public ATotalDetectorInput() : this(TallyType.ATotal.ToString()) { }

        /// <summary>
        /// detector tally identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, defaults to TallyType.ToString() but can be user specified
        /// </summary>
        public String Name { get; set; }
    }
}
