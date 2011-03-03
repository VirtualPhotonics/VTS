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
        public PhotonTerminationDatabase(long numPhotons)
        {
            NumberOfPhotons = numPhotons;
        }

        public PhotonTerminationDatabase() : this(1000000) { } 

        public long NumberOfPhotons { get; set; }

        [IgnoreDataMember]
        public IEnumerable<PhotonDataPoint> DataPoints { get; set; }

        public static PhotonTerminationDatabase FromFile(string fileName)
        {
            var photonExitHistory = FileIO.ReadFromXML<PhotonTerminationDatabase>(fileName + ".xml");

            var serializer = new PhotonDataPointCustomBinarySerializer();
            
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

            var serializer = new PhotonDataPointCustomBinarySerializer();

            photonExitHistory.DataPoints = FileIO.ReadFromBinaryInResourcesCustom<PhotonDataPoint>(
                fileName, 
                projectName, 
                serializer.ReadFromBinary);

            return photonExitHistory;
        }
    }
}
