using Vts.Common;

namespace Vts.MonteCarlo
{
    public class PolarLookupTablePhaseFunctionData : ILookupTablePhaseFunctionData
    {
        public PolarLookupTablePhaseFunctionData()
        {
            Name = "PolarLookupTablePhaseFunctionData";
            //Type = LookupTablePhaseFunctionDataType.PolarLookupTablePhaseFunctionData;

        }
        //public LookupTablePhaseFunctionDataType Type { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// LutAngles are the theta angles
        /// </summary>
        public double[] LutAngles { get; set; }
        /// <summary>
        /// LutPdf is the scattering angles probability distribution function (PDF) for each theta angle
        /// </summary>
        public double[] LutPdf { get; set; }
        /// <summary>
        /// lutCdf is the cumulative distribution function (CDF) associated with the PDF
        /// </summary>
        public double[] LutCdf { get; set; }
    }
}