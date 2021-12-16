namespace Vts.MonteCarlo
{
    /// <summary>
    /// Phase function types
    /// </summary>
    public static class PhaseFunctionType
    {
        public static readonly string[] BuiltInTypes =
        {
            /// <summary>
            /// Henyey-Greenstein scattering phase functiion
            /// </summary>
            "HenyeyGreenstein",
            /// <summary>
            /// bidirectional scattering phase function
            /// </summary>
            "Bidirectional",
            /// <summary>
            /// Options for discretized p(theta) scattering phase function
            /// </summary>
            "LookupTable",
        };
    }
}