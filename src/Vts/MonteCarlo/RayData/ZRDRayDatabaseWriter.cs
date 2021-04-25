using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Implements CustomBinaryStreamWriter(Of ZRDRayDataPoint). Handles writing ray
    /// data to database.
    /// </summary>
    public class ZRDRayDatabaseWriter : DatabaseWriter<ZRDRayDatabase, ZRDRayDataPoint>
    {
        /// <summary>
        /// constructor for photon database writer
        /// </summary>
        /// <param name="virtualBoundaryType">virtual boundary type</param>
        /// <param name="filename">database filename</param>
        public ZRDRayDatabaseWriter(VirtualBoundaryType virtualBoundaryType, string filename)
            : base(filename, new ZRDRayDatabase(), new ZRDRayDataPointSerializer())
        {
            VirtualBoundaryType = virtualBoundaryType;
        }
        /// <summary>
        /// virtual boundary type
        /// </summary>
        public VirtualBoundaryType VirtualBoundaryType { get; set; }
    }
}
