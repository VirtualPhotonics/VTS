namespace Vts.Common
{
    /// <summary>
    /// class for layer region optical properties
    /// </summary>
    public class LayerOpticalPropertyRegion : ILayerOpticalPropertyRegion
    {
        /// <summary>
        /// Create an instance of LayerOpticalPropertyRegion
        /// </summary>
        /// <param name="regionOP">optical properties of the layer region</param>
        /// <param name="zRange">z range of the layer region</param>
        public LayerOpticalPropertyRegion(DoubleRange zRange, OpticalProperties regionOP)
        {
            ZRange = zRange;
            RegionOP = regionOP;
        }

        /// <summary>
        /// Optical properties of tissue region.
        /// </summary>
        public OpticalProperties RegionOP { get; set; }

        /// <summary>
        /// extent of z layer
        /// </summary>
        public DoubleRange ZRange { get; set; }
    }
}
