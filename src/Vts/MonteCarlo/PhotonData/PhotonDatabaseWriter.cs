using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Implements CustomBinaryStreamWriter(Of PhotonDataPoint). Handles writing photon
    /// data to database.
    /// </summary>
    public class PhotonDatabaseWriter : DatabaseWriter<PhotonDatabase, PhotonDataPoint>
    {
        /// <summary>
        /// constructor for photon database writer
        /// </summary>
        /// <param name="virtualBoundaryType">virtual boundary type</param>
        /// <param name="filename">database filename</param>
        public PhotonDatabaseWriter(VirtualBoundaryType virtualBoundaryType, string filename)
            : base(filename, new PhotonDatabase(), new PhotonDataPointSerializer())
        {
            VirtualBoundaryType = virtualBoundaryType;
        }
        /// <summary>
        /// virtual boundary type
        /// </summary>
        public VirtualBoundaryType VirtualBoundaryType { get; set; }
    }
}
