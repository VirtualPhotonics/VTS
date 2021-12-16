namespace Vts.MonteCarlo
{
    /// <summary>
    /// Flag indicating whether the photon hit a actual tissue boundary or a virtual boundary
    /// </summary>
    public enum BoundaryHitType
    {
        /// <summary>
        /// No boundary hit
        /// </summary>
        None,
        /// <summary>
        /// Virtual boundary hit by photon
        /// </summary>
        Virtual,
        /// <summary>
        /// Actual (tissue) boundary hit by photon
        /// </summary>
        Tissue
    }
}