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
        public PhotonTerminationDatabaseWriter(string filename, int numberOfSubRegions, bool tallyMomentumTransfer)
            : base(filename, new PhotonTerminationDataPointCustomBinarySerializer(numberOfSubRegions, tallyMomentumTransfer))
        {
            // this specifies any action to take at the end of the file writing
            PostWriteAction = delegate
            {   
                // note: Count will be calculated at the end, not captured at instantiation
                Func<long> currentCount = () => Count;
                //new PhotonTerminationDatabase // why not call perameterized constructurer? what's the "bug"?
                //    {
                //        NumberOfPhotons = currentCount(),
                //        NumberOfSubRegions = numberOfSubRegions,
                //        TallyMomentumTransfer = tallyMomentumTransfer,
                //    }.WriteToXML(filename + ".xml");
                new PhotonTerminationDatabase( currentCount(), numberOfSubRegions, tallyMomentumTransfer)
                        .WriteToXML(filename + ".xml");
            };
        }
    }
}
