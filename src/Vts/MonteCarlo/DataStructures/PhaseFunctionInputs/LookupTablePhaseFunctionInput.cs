using System;

namespace Vts.MonteCarlo
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
            new PolarLookupTablePhaseFunctionData()
                {
                    LutAngles = new[] { 0, Math.PI/6, Math.PI/3, Math.PI/2, 2*Math.PI/3, Math.PI*5/6, Math.PI },
                    LutPdf = new[] { 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5 },
                    LutCdf = new[] { 0, 0.5 * (1 - Math.Sqrt(3) / 2), 0.25 , 0.5, 0.75 , 0.5 * (1 + Math.Sqrt(3) / 2), 1},
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
        public PhaseFunctionType PhaseFunctionType { get; set; }
    }
}