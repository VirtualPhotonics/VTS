using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Describes database for storing and returning photon data points (position, direction, weight &amp; total time).
    /// The base class, Database(OfT), exposes the IEnumerable(OfT) DataPoints list of PhotonDataPoint items
    /// </summary>
    public class PhotonDatabase : Database<PhotonDataPoint>
    {
        /// <summary>
        /// Returns an instance of PhotonDatabase
        /// </summary>
         public PhotonDatabase()
        {
        }

        ///// <summary>
        ///// Do not use this overload, it is only for serialization purposes
        ///// </summary>
        //public PhotonDatabase()
        //    : this(new SimulationInput())
        //{
        //}

        ///// <summary>
        ///// The details of the simulation that generated this result
        ///// </summary>
        //public SimulationInput SimulationInput { get; set; }

        /// <summary>
        /// Static helper method to simplify reading from file
        /// </summary>
        /// <param name="fileName">The base filename for the database (no ".txt")</param>
        /// <returns>A new instance of PhotonDatabase</returns>
        public static PhotonDatabase FromFile(string fileName)
        {
            var dbReader = new DatabaseReader<PhotonDatabase, PhotonDataPoint>(
                db => new PhotonDataPointSerializer());

            return dbReader.FromFile(fileName);
        }

        /// <summary>
        /// Static helper method to simplify reading from file
        /// </summary>
        /// <param name="fileName">The base filename for the database (no ".txt")</param>
        /// <param name="projectName">The project name containing the resource</param>
        /// <returns>A new instance of PhotonDatabase</returns>
        public static PhotonDatabase FromFileInResources(string fileName, string projectName)
        {
            var dbReader = new DatabaseReader<PhotonDatabase, PhotonDataPoint>(
                db => new PhotonDataPointSerializer());

            return dbReader.FromFileInResources(fileName, projectName);
        }
    }
}
