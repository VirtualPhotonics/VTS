using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Describes database for storing and returning ray data points (position, direction, weight).
    /// The base class, Database(OfT), exposes the IEnumerable(OfT) DataPoints list of RayDataPoint items
    /// </summary>
    public class RayDatabase : Database<RayDataPoint>
    {
        /// <summary>
        /// Returns an instance of PhotonDatabase
        /// </summary>
         public RayDatabase()
        {
        }

        /// <summary>
        /// Static helper method to simplify reading from file
        /// </summary>
        /// <param name="fileName">The base filename for the database (no ".txt")</param>
        /// <returns>A new instance of PhotonDatabase</returns>
        public static RayDatabase FromFile(string fileName)
        {
            var dbReader = new DatabaseReader<RayDatabase, RayDataPoint>(
                db => new RayDataPointSerializer());

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
