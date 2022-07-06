using Vts.Common;

namespace Vts
{
    /// <summary>
    /// layer optical property region
    /// </summary>
    public interface ILayerOpticalPropertyRegion : IOpticalPropertyRegion
    {
        /// <summary>
        /// extent of z layer
        /// </summary>
        DoubleRange ZRange { get; set; }
    }
}
