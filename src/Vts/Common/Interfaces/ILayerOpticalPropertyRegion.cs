using Vts.Common;

namespace Vts
{
    public interface ILayerOpticalPropertyRegion : IOpticalPropertyRegion
    {
        /// <summary>
        /// extent of z layer
        /// </summary>
        DoubleRange ZRange { get; set; }
    }
}
