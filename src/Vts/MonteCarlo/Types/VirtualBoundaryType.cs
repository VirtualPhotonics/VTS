namespace Vts.MonteCarlo
{
    /// <summary>
    /// Virtual boundaries are entities upon which detectors are attached.
    /// Each detector is associated with one and only one of the following types.
    /// The VBs have a spatial location (surface or volume) and sometimes have
    /// a direction.
    /// </summary>
    public enum VirtualBoundaryType
    {
        /// <summary>
        /// All diffuse reflectance detectors attach to this virtual boundary type
        /// </summary>
        DiffuseReflectance,
        /// <summary>
        /// All diffuse transmittance detectors attach to this virtual boundary type
        /// </summary>
        DiffuseTransmittance,
        /// <summary>
        /// Specular reflection detectors attach to this virtual boundary type
        /// </summary>
        SpecularReflectance,
        /// <summary>
        /// Internal volume detectors attach to this virtual boundary type
        /// </summary>
        GenericVolumeBoundary,
        /// <summary>
        /// Internal surface detectors attach to this virtual boundary type
        /// </summary>
        InternalSurface,
        /// <summary>
        /// Virtual boundary used for pMC diffuse reflectance detectors
        /// </summary>
        pMCDiffuseReflectance,
        /// <summary>
        /// Virtual boundary used for pMC diffuse transmittance detectors
        /// </summary>
        pMCDiffuseTransmittance,
        /// <summary>
        /// Virtual boundary used to capture photons if leave this lateral boundary
        /// </summary>
        BoundingVolume,
    }
}