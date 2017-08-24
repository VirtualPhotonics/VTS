using System.Runtime.Serialization;

namespace Vts.MonteCarlo
{
    public class PolarLookupTablePhaseFunctionData : ILookupTablePhaseFunctionData
    {
        public PolarLookupTablePhaseFunctionData()
        {
            Name = "PolarLookupTablePhaseFunctionData";
        }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// LutAngles are the theta angles
        /// </summary>
        [IgnoreDataMember] public double[] LutAngles { get; set; }
        /// <summary>
        /// LutPdf is the scattering angles probability distribution function (PDF) for each theta angle
        /// </summary>
        [IgnoreDataMember] public double[] LutPdf { get; set; }
        /// <summary>
        /// lutCdf is the cumulative distribution function (CDF) associated with the PDF
        /// </summary>
        [IgnoreDataMember] public double[] LutCdf { get; set; }
    }
}