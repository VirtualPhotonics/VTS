namespace Vts
{
    /// <summary>
    /// fluence solution domain types 
    /// </summary>
    public enum FluenceSolutionDomainType
    {
        /// <summary>
        /// fluence as a function or source-detector separation (rho) and tissue depth (z)
        /// </summary>
        FluenceOfRhoAndZ,
        /// <summary>
        /// fluence as a function of spatial-frequency (fx) and tissue depth (z)
        /// </summary>
        FluenceOfFxAndZ,
        /// <summary>
        /// fluence as a function or source-detector separation (rho), tissue depth (z) and time (t)
        /// </summary>
        FluenceOfRhoAndZAndTime,
        /// <summary>
        /// fluence as a function of spatial-frequency (fx), tissue depth (z) and time (t)
        /// </summary>
        FluenceOfFxAndZAndTime,
        /// <summary>
        /// fluence as a function of source-detector separation (rho), tissue depth (z) and temporal-frequency (ft)
        /// </summary>
        FluenceOfRhoAndZAndFt,
        /// <summary>
        /// fluence as a function of spatial-frequency (fx), tissue depth (z) and temporal-frequency (ft)
        /// </summary>
        FluenceOfFxAndZAndFt
    }
}