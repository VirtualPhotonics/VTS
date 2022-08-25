namespace Vts.Extensions
{
    /// <summary>
    /// Extension methods for optical properties class
    /// </summary>
    public static class OpticalPropertiesExtensions
    {
        /// <summary>
        /// Extension method to determine scattering length given the absorption weighting type 
        /// </summary>
        /// <param name="op">The optical properties</param>
        /// <param name="awt">The absorption weighting type</param>
        /// <returns>A scatter length</returns>
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
