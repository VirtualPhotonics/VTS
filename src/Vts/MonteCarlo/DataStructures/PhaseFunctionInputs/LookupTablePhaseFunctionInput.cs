using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctionInputs
{
    public class LookupTablePhaseFunctionInput : IPhaseFunctionInput
    {
        /// <summary>
        /// Constructs an instance of LookupTablePhaseFunctionInput
        /// </summary>
        /// <param name="phaseFunctionData"></param>
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
                    LutAngles = new[] { 0.5 },
                    LutPdf = new [] { 1.0, 0.0 }, // todo: need bins!
                    LutCdf = new [] { 1.0, 0.0 },
                }
            )
        {
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