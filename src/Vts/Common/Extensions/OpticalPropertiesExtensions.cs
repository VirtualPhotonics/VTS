namespace Vts.Extensions
{
    public static class OpticalPropertiesExtensions
    {
        /// <summary>
        /// Method to determine scattering length given the absorption weighting type 
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="awt">absorption weighting type</param>
        /// <returns>scatter length</returns>
        public static double GetScatterLength(this OpticalProperties op, AbsorptionWeightingType awt)
        {
            switch (awt)
            {
                default:
                case AbsorptionWeightingType.Discrete:
                case AbsorptionWeightingType.Analog:
                    return (op.Mua + op.Mus);
                case AbsorptionWeightingType.Continuous:
                    return op.Mus;
            }
        }
    }
}
