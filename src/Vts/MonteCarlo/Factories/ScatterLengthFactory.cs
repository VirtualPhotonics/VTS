namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Determines correct scatter length of Monte Carlo simulation given
    /// the AbsorptionWeightingType and optical properties.
    /// </summary>
    public static class ScatterLengthFactory
    {
        public static double GetScatterLength(this OpticalProperties op, AbsorptionWeightingType type)
        {
            switch (type)
            {
                case AbsorptionWeightingType.Analog:
                case AbsorptionWeightingType.Discrete:
                default:
                    return op.Mus + op.Mua;
                case AbsorptionWeightingType.Continuous:
                    return op.Mus;
            }
        }
    }
}
