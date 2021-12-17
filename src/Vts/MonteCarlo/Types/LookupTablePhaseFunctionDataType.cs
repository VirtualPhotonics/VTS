namespace Vts.MonteCarlo
{
    /// <summary>
    /// Phase function types
    /// </summary>
    public static class LookupTablePhaseFunctionDataType
    {
        /// <summary>
        /// Types for lookup table phase function
        /// </summary>
        public static readonly string[] BuiltInTypes =
        {
            // Polar only
            "Polar",
            // Polar And Azimuthal
            "PolarAndAzimuthal",
        };
    }
}