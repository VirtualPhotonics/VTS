using System;
using Vts.IO;
using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Implements CustomBinaryStreamWriter(Of PhotonDataPoint). Handles writing photon
    /// data to database.
    /// </summary>
    public class PhotonDatabaseWriter : DatabaseWriter<PhotonDatabase, PhotonDataPoint>
    {
        public PhotonDatabaseWriter(VirtualBoundaryType virtualBoundaryType, string filename)
            : base(filename, new PhotonDatabase(), new PhotonDataPointSerializer())
        {
            VirtualBoundaryType = virtualBoundaryType;
        }
        public VirtualBoundaryType VirtualBoundaryType { get; set; }
    }
}
