namespace Vts
{
    /// <summary>
    /// Reflectance solution domain types
    /// </summary>
    public enum SolutionDomainType
    {
        /// <summary>
        /// reflectance as a function of source-detector separation (rho)
        /// </summary>
        ROfRho,
        /// <summary>
        /// reflectance as a function of spatial-frequency (fx)
        /// </summary>
        ROfFx,
        /// <summary>
        /// reflectance as a function of source-detector separation (rho) and time (t)
        /// </summary>
        ROfRhoAndTime,
        /// <summary>
        /// reflectance as a function of spatial-frequency (fx) and time (t)
        /// </summary>
        ROfFxAndTime,
        /// <summary>
        /// reflectance as a function source-detector separation (rho) and temporal-frequency (ft)
        /// </summary>
        ROfRhoAndFt,
        /// <summary>
        /// reflectance as a function of spatial-frequency (fx) and temporal-frequency (ft)
        /// </summary>
        ROfFxAndFt
    }
}