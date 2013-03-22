namespace Vts.MonteCarlo.PhaseFunctionInputs
{
    public class PolarAndAzimuthalLookupTablePhaseFunctionData : ILookupTablePhaseFunctionData
    {
        public PolarAndAzimuthalLookupTablePhaseFunctionData()
        {
            Name = "PolarAndAzimuthalLookupTablePhaseFunctionData";
        }

        public string Name { get; set; }
        public double[] LutAngles { get; set; }
        public double[] LutPdf { get; set; }
        public double[] LutCdf { get; set; }
    }
}