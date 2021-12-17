namespace Vts.MonteCarlo
{
    /// <summary>
    /// Phase function types
    /// </summary>
    public static class PhaseFunctionType
    {
        /// <summary>
        /// Phase function types
        /// </summary>
        public static readonly string[] BuiltInTypes =
        {
            // Henyey-Greenstein scattering phase function
            "HenyeyGreenstein",
            // bidirectional scattering phase function
            "Bidirectional",
            // Options for discretized p(theta) scattering phase function
            "LookupTable",
        };
    }
}