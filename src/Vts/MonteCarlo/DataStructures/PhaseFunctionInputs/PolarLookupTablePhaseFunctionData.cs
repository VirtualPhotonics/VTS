namespace Vts.MonteCarlo.PhaseFunctionInputs
{
    public class PolarLookupTablePhaseFunctionData : ILookupTablePhaseFunctionData
    {
        public PolarLookupTablePhaseFunctionData()
        {
            Name = "PolarLookupTablePhaseFunctionData";
        }

        public string Name { get; set; }
        public double[] LutAngles { get; set; }
        public double[] LutPdf { get; set; }
        public double[] LutCdf { get; set; }
    }
}