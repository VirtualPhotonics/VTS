namespace Vts
{
    /// <summary>
    /// Absorption weighting type used within Monte Carlo code
    /// </summary>
    public enum AbsorptionWeightingType
    {
        /// <summary>
        /// analog absorption weighting
        /// </summary>
        Analog,
        /// <summary>
        /// discrete absorption weighting
        /// </summary>
        Discrete,
        /// <summary>
        /// continuous absorption weighting
        /// </summary>
        Continuous,
    }
}