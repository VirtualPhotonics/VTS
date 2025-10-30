using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Describes database for storing and returning photon emission data points (position, direction, weight, totalTime).
    /// The base class, Database(OfT), exposes the IEnumerable(OfT) DataPoints list of RayDataPoint items
    /// </summary>
    public class PhotonEmissionDatabase : Database<PhotonEmissionDataPoint>
    {
        /// <summary>
        /// Returns an instance of PhotonDatabase
        /// </summary>
         public PhotonEmissionDatabase()
        {
        }

        /// <summary>
        /// Static helper method to simplify reading from file
        /// </summary>
        /// <param name="fileName">The base filename for the database (no ".txt")</param>
        /// <returns>A new instance of PhotonEmissionDatabase</returns>
        public static PhotonEmissionDatabase FromFile(string fileName)
        {
            var dbReader = new DatabaseReader<PhotonEmissionDatabase, PhotonEmissionDataPoint>(
                db => new PhotonEmissionDataPointSerializer());

            return dbReader.FromFile(fileName);
        }

        /// <summary>
        /// Static helper method to simplify reading from file
        /// </summary>
        /// <param name="fileName">The base filename for the database (no ".txt")</param>
        /// <param name="projectName">The project name containing the resource</param>
        /// <returns>A new instance of PhotonDatabase</returns>
        public static RayDatabase FromFileInResources(string fileName, string projectName)
        {
            var dbReader = new DatabaseReader<RayDatabase, RayDataPoint>(
                db => new RayDataPointSerializer());

            return dbReader.FromFileInResources(fileName, projectName);
        }
    }
}
