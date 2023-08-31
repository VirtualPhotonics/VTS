namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// details for each detector that identifies type of detector
    /// </summary>
    public class TallyDetails
    {
        /// <summary>
        /// Boolean identifying all reflectance tallies
        /// </summary>
        public bool IsReflectanceTally { get; set; }
        /// <summary>
        /// Boolean identifying all transmittance tallies
        /// </summary>
        public bool IsTransmittanceTally { get; set; }
        /// <summary>
        /// Boolean identifying all specular reflectance tallies
        /// </summary>
        public bool IsSpecularReflectanceTally { get; set; }
        /// <summary>
        /// Boolean identifying all internal surface tallies
        /// </summary>
        public bool IsInternalSurfaceTally { get; set; }
        /// <summary>
        /// Boolean identifying all perturbation MC reflectance tallies
        /// </summary>
        public bool IspMCReflectanceTally { get; set; }
        /// <summary>
        /// Boolean identifying all perturbation MC transmittance tallies
        /// </summary>
        public bool IspMCTransmittanceTally { get; set; }
        /// <summary>
        /// Boolean identifying all bounding volume tallies
        /// </summary>
        public bool IsLateralBoundingVolumeTally { get; set; }
        /// <summary>
        /// Boolean identifying all volume tallies
        /// </summary>
        public bool IsVolumeTally { get; set; }
        /// <summary>
        /// Boolean identifying whether detector is in cylindrical coordinates
        /// </summary>
        public bool IsCylindricalTally { get; set; }
        /// <summary>
        /// Boolean identifying whether detector is not implemented yet for DAW
        /// </summary>
        public bool IsNotImplementedForDAW { get; set; }
        /// <summary>
        /// Boolean identifying whether detector is not implemented yet for CAW
        /// </summary>
        public bool IsNotImplementedForCAW { get; set; }
        /// <summary>
        /// Boolean identifying whether detector is not implemented yet
        /// </summary>
        public bool IsNotImplementedYet { get; set; }
    }
}
