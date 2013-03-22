using System;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctionInputs
{
    public class LookupTablePhaseFunctionInput : IPhaseFunctionInput
    {
        /// <summary>
        /// Constructs an instance of LookupTablePhaseFunctionInput
        /// </summary>
        /// <param name="phaseFunctionData">A variable that stores the tabulated phase function values evaluated at particular polar angles.</param>
        public LookupTablePhaseFunctionInput(ILookupTablePhaseFunctionData phaseFunctionData)
        {
            RegionPhaseFunctionData = phaseFunctionData;
            PhaseFunctionType = PhaseFunctionType.LookupTable;
        }

        /// <summary>
        /// Default constructor for serialization purposes only
        /// </summary>
        public LookupTablePhaseFunctionInput() : this(
            new PolarLookupTablePhaseFunctionData
                {
                    LutAngles = new[] { 0, Math.PI },
                    LutPdf = new [] { 1.0, 0.0 }, // todo: need bins!
                    LutCdf = new [] { 1.0, 1.0 },
                }
            )
        {
            PhaseFunctionType = PhaseFunctionType.LookupTable;
        }

        /// <summary>
        /// Optional data object to store phase-function specific code
        /// </summary>
        public ILookupTablePhaseFunctionData RegionPhaseFunctionData { get; set; }
        
        /// <summary>
        /// Phase function type
        /// </summary>
        [IgnoreDataMember]
        public PhaseFunctionType PhaseFunctionType { get; set; }
    }
}