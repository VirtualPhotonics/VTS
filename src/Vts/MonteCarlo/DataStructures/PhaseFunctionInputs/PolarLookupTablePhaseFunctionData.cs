using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.PhaseFunctions
{
    public class PolarLookupTablePhaseFunctionData : ILookupTablePhaseFunctionData
    {
        public PolarLookupTablePhaseFunctionData()
        {
            //LookupTableDataType = "PolarLookupTablePhaseFunctionData";
        }

        /// <summary>
        /// Type of data
        /// </summary>
        //public string LookupTableDataType { get { return "PolarLookupTablePhaseFunctionData"; } }
        public Vts.LookupTablePhaseFunctionDataType LookupTablePhaseFunctionDataType
        {
            get { return Vts.LookupTablePhaseFunctionDataType.PolarLookupTablePhaseFunctionData; }
        }
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