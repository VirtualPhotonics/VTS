namespace Vts.MonteCarlo
{
    public class UnmanagedSimulationOptions : ISimulationOptions
    {
        public int Seed { get; set; }
        public AbsorptionWeightingType AbsorptionWeightingType { get; set; }

        public UnmanagedSimulationOptions(int seed) : this(0, AbsorptionWeightingType.Discrete) { }

        public UnmanagedSimulationOptions(int seed, AbsorptionWeightingType absWtType)
        {
            Seed = seed;
            AbsorptionWeightingType = absWtType;
        }
        public UnmanagedSimulationOptions() : this(0, AbsorptionWeightingType.Discrete) { }
    }
}
