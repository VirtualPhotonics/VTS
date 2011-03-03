using System;
using Vts.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Implements CustomBinaryStreamWriter<PhotonDataPoint>.   Handles writing photon
    /// terminating data to database.
    /// </summary>
    public class PhotonTerminationDatabaseWriter : CustomBinaryStreamWriter<PhotonDataPoint>
    {
        public PhotonTerminationDatabaseWriter(string filename)
            : base(filename, new PhotonDataPointCustomBinarySerializer())
        {
            // this specifies any action to take at the end of the file writing
            PostWriteAction = delegate
            {   
                // note: Count will be calculated at the end, not captured at instantiation
                Func<long> currentCount = () => Count;
                new PhotonTerminationDatabase( currentCount()).WriteToXML(filename + ".xml");
            };
        }
    }
}
