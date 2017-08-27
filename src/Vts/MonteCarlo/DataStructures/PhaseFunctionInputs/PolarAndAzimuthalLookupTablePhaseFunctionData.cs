using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.PhaseFunctions
{
    public class PolarAndAzimuthalLookupTablePhaseFunctionData : ILookupTablePhaseFunctionData
    {
        public PolarAndAzimuthalLookupTablePhaseFunctionData()
        {
            //LookupTableDataType = "PolarAndAzimuthalLookupTablePhaseFunctionData";
        }
        /// <summary>
        /// Type of data
        /// </summary>
        //public string LookupTableDataType { get { return "PolarAndAziumuthalLookupTablePhaseFunctionData";} }
        public Vts.LookupTablePhaseFunctionDataType LookupTablePhaseFunctionDataType { get
        {
            return Vts.LookupTablePhaseFunctionDataType.PolarAndAziumuthalLookupTablePhaseFunctionData;
        } }
        /// <summary>
        /// theta, phi angles
        /// </summary>
        public double[,] LutAngles { get; set; }
        /// <summary>
        /// lookup pdf given theta and phi
        /// </summary>
        public double[] LutPdf { get; set; }
        /// <summary>
        /// lookup cdf given theta and phi
        /// </summary>
        public double[] LutCdf { get; set; }
    }
}