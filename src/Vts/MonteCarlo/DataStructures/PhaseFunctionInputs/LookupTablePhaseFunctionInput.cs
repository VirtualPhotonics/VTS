using System;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Lookup table phase function input
    /// </summary>
    public class LookupTablePhaseFunctionInput : IPhaseFunctionInput
    {
        /// <summary>
        /// Constructs an instance of LookupTablePhaseFunctionInput
        /// </summary>
        /// <param name="phaseFunctionData">A variable that stores the tabulated phase function values evaluated at particular polar angles.</param>
        public LookupTablePhaseFunctionInput(ILookupTablePhaseFunctionData phaseFunctionData)
        {
            RegionPhaseFunctionData = phaseFunctionData;
            PhaseFunctionType = "LookupTable";
        }

        /// <summary>
        /// Default constructor for serialization purposes only
        /// </summary>
        public LookupTablePhaseFunctionInput() : this(
            new PolarLookupTablePhaseFunctionData()           
            )
        {}

        /// <summary>
        /// Optional data object to store phase-function specific code
        /// </summary>
        public ILookupTablePhaseFunctionData RegionPhaseFunctionData { get; set; }
        
        /// <summary>
        /// Phase function type
        /// </summary>
        public string PhaseFunctionType { get; set; }

    }
}