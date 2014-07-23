namespace Vts.Common
{
    public class LayerOpticalPropertyRegion : ILayerOpticalPropertyRegion
    {
        /// <summary>
        /// Create an instance of LayerOpticalPropertyRegion
        /// </summary>
        /// <param name="regionOP"></param>
        /// <param name="zRange"></param>
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
