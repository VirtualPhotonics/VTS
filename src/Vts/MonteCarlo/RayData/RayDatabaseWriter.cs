using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Implements CustomBinaryStreamWriter(Of RayDataPoint). Handles writing ray
    /// data to database.
    /// </summary>
    public class RayDatabaseWriter : DatabaseWriter<RayDatabase, RayDataPoint>
    {
        /// <summary>
        /// constructor for photon database writer
        /// </summary>
        /// <param name="virtualBoundaryType">virtual boundary type</param>
        /// <param name="filename">database filename</param>
        public RayDatabaseWriter(VirtualBoundaryType virtualBoundaryType, string filename)
            : base(filename, new RayDatabase(), new RayDataPointSerializer())
        {
            VirtualBoundaryType = virtualBoundaryType;
        }
        /// <summary>
        /// virtual boundary type
        /// </summary>
        public VirtualBoundaryType VirtualBoundaryType { get; set; }
    }
}
