using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Describes database for terminating photons.
    /// </summary>
    public class PhotonTerminationDatabase
    {
        //public PhotonTerminationDatabase(long numPhotons, long numSubRegions, bool tallyMomentumTransfer)
        public PhotonTerminationDatabase(long numPhotons, long numSubRegions)
        {
            NumberOfPhotons = numPhotons;
            NumberOfSubRegions = numSubRegions;
            //TallyMomentumTransfer = tallyMomentumTransfer;
        }

        public PhotonTerminationDatabase() : this(1000000, 3) { } 

        public long NumberOfPhotons { get; set; }
        public long NumberOfSubRegions { get; set; }
        //public bool TallyMomentumTransfer { get; set; }

        [IgnoreDataMember]
        public IEnumerable<PhotonDataPoint> DataPoints { get; set; }

        public static PhotonTerminationDatabase FromFile(string fileName)
        {
            var photonExitHistory = FileIO.ReadFromXML<PhotonTerminationDatabase>(fileName + ".xml");
            //var serializer = new PhotonTerminationDataPointCustomBinarySerializer(
            //    photonExitHistory.NumberOfSubRegions, 
            //    photonExitHistory.TallyMomentumTransfer);
            var serializer = new PhotonTerminationDataPointCustomBinarySerializer(
                photonExitHistory.NumberOfSubRegions);
            
            photonExitHistory.DataPoints = FileIO.ReadFromBinaryCustom<PhotonDataPoint>(
                fileName, 
                serializer.ReadFromBinary);
            
            return photonExitHistory;
        }

        public static PhotonTerminationDatabase FromFileInResources(string fileName, string projectName)
        {
            var photonExitHistory = FileIO.ReadFromXMLInResources<PhotonTerminationDatabase>(
                fileName + ".xml", 
                projectName);

            //var serializer = new PhotonTerminationDataPointCustomBinarySerializer(
            //    photonExitHistory.NumberOfSubRegions,
            //    photonExitHistory.TallyMomentumTransfer);
            var serializer = new PhotonTerminationDataPointCustomBinarySerializer(
                photonExitHistory.NumberOfSubRegions);

            photonExitHistory.DataPoints = FileIO.ReadFromBinaryInResourcesCustom<PhotonDataPoint>(
                fileName, 
                projectName, 
                serializer.ReadFromBinary);

            return photonExitHistory;
        }
    }
}
