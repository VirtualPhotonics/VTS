using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Implements CustomBinaryStreamWriter(Of PhotonEmissionDataPoint). Handles writing ray
    /// data to database.
    /// </summary>
    public class PhotonEmissionDatabaseWriter : DatabaseWriter<PhotonEmissionDatabase, PhotonEmissionDataPoint>
    {
        /// <summary>
        /// constructor for ray database writer
        /// </summary>
        /// <param name="virtualBoundaryType">virtual boundary type</param>
        /// <param name="filename">database filename</param>
        public PhotonEmissionDatabaseWriter(VirtualBoundaryType virtualBoundaryType, string filename)
            : base(filename, new PhotonEmissionDatabase(), new PhotonEmissionDataPointSerializer())
        {
            VirtualBoundaryType = virtualBoundaryType;
        }
        /// <summary>
        /// virtual boundary type
        /// </summary>
        public VirtualBoundaryType VirtualBoundaryType { get; set; }
    }
}
