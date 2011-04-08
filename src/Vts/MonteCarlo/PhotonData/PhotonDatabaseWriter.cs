using System;
using Vts.IO;
using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Implements CustomBinaryStreamWriter(Of PhotonDataPoint). Handles writing photon
    /// terminating data to database.
    /// </summary>
    public class PhotonDatabaseWriter : DatabaseWriter<PhotonDatabase, PhotonDataPoint>
    {
        public PhotonDatabaseWriter(string filename)
            //: base(filename, new PhotonDatabase(), new PhotonDataPointSerializer())
            : base(filename, new PhotonDatabase(new SimulationInput { N = 1000 }), new PhotonDataPointSerializer())
        {
        }
    }
}
