namespace Vts.MonteCarlo.PhaseFunctionInputs
{
    public class PolarAndAzimuthalLookupTablePhaseFunctionData : ILookupTablePhaseFunctionData
    {
        public PolarAndAzimuthalLookupTablePhaseFunctionData()
        {
            Name = "PolarAndAzimuthalLookupTablePhaseFunctionData";
        }
        /// <summary>
        /// Name of data
        /// </summary>
        public string Name { get; set; }
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