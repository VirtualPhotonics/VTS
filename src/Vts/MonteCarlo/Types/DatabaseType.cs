namespace Vts.MonteCarlo
{
    /// <summary>
    /// This should match VirtualBoundaryType one for one.  Commented out ones have not made
    /// it to the white list yet.
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// All diffuse reflectance detectors 
        /// </summary>
        DiffuseReflectance,
        /// <summary>
        /// All diffuse transmittance detectors 
        /// </summary>
        DiffuseTransmittance,
        /// <summary>
        /// Specular reflection detectors 
        /// </summary>
        SpecularReflectance,
        ///// <summary>
        ///// Internal volume detectors 
        ///// </summary>
        //GenericVolumeBoundary,
        ///// <summary>
        ///// Internal surface detectors 
        ///// </summary>
        //SurfaceRadiance,
        /// <summary>
        /// pMC diffuse reflectance
        /// </summary>
        pMCDiffuseReflectance,
        /// <summary>
        /// Zemax uncompressed database format
        /// </summary>
        ZRDDiffuseReflectance,
    }
}