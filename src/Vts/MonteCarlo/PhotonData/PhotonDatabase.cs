using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Describes database for photon data points.
    /// </summary>
    public class PhotonDatabase
    {
        public PhotonDatabase(long numPhotons)
        {
            NumberOfPhotons = numPhotons;
        }

        public PhotonDatabase() : this(1000000) { } 

        public long NumberOfPhotons { get; set; }

        [IgnoreDataMember]
        public IEnumerable<PhotonDataPoint> DataPoints { get; set; }

        public static PhotonDatabase FromFile(string fileName)
        {
            var photonExitHistory = FileIO.ReadFromXML<PhotonDatabase>(fileName + ".xml");

            var serializer = new PhotonDataPointCustomBinarySerializer();
            
            photonExitHistory.DataPoints = FileIO.ReadFromBinaryCustom<PhotonDataPoint>(
                fileName, 
                serializer.ReadFromBinary);
            
            return photonExitHistory;
        }

        public static PhotonDatabase FromFileInResources(string fileName, string projectName)
        {
            var photonExitHistory = FileIO.ReadFromXMLInResources<PhotonDatabase>(
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
