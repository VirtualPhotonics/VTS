using System.Linq;
using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Describes database for storing and returning photon data points (position, direction, weight &amp; total time).
    /// The base class, Database(OfT), exposes the IEnumerable(OfT) DataPoints list of PhotonDataPoint items
    /// </summary>
    public class pMCDatabase : Database<pMCDataPoint>
    {
        /// <summary>
        /// Returns an instance of PhotonDatabase
        /// </summary>
        public pMCDatabase(PhotonDatabase photonDatabase, CollisionInfoDatabase collisionInfoDatabase)
        {
            PhotonDatabase = photonDatabase;
            CollisionInfoDatabase = collisionInfoDatabase;
        }

        /// <summary>
        /// Do not use this overload, it is only for serialization purposes
        /// </summary>
        public pMCDatabase()
            : this(new PhotonDatabase(), new CollisionInfoDatabase(0))
        {
        }

        // these public members are available to be accessed by the user if necessary
        // (e.g. PhotonDatabase.SimulationInput to get RegionOP, ranges, etc)

        /// <summary>
        /// The underlying PhotonDatabase
        /// </summary>
        public PhotonDatabase PhotonDatabase { get; set; }

        /// <summary>
        /// The underlying CollisionInfoDatabase
        /// </summary>
        public CollisionInfoDatabase CollisionInfoDatabase { get; set; }

        /// <summary>
        /// Static helper method to simplify reading from file
        /// </summary>
        /// <param name="photonDatabaseFileName">The filename for the PhotonDataPoint database (no ".xml")</param>
        /// <param name="collisionInfoDatabaseFileName">The filename for the CollisionInfo database (no ".xml")</param>
        /// <returns>A new instance of PhotonDatabase</returns>
        public static pMCDatabase FromFile(string photonDatabaseFileName, string collisionInfoDatabaseFileName)
        {
            var photonDatabase = PhotonDatabase.FromFile(photonDatabaseFileName);

            var collisionInfoDatabase = CollisionInfoDatabase.FromFile(collisionInfoDatabaseFileName);

            var zippedDataPoints = Enumerable.Zip(
                photonDatabase.DataPoints,
                collisionInfoDatabase.DataPoints,
                (photonDataPoint, collisionInfo) => new pMCDataPoint(photonDataPoint, collisionInfo));
            
            var pMCDatabase = new pMCDatabase(photonDatabase, collisionInfoDatabase);

            pMCDatabase.SetDataPoints(zippedDataPoints);

            return pMCDatabase;
        }
        /// <summary>
        /// Method to read database from file in resources.
        /// </summary>
        /// <param name="photonDatabaseFileName">PhotonDataPoint database name</param>
        /// <param name="collisionInfoDatabaseFileName">CollisionInfo database name</param>
        /// <param name="projectName">project name where the databases reside</param>
        /// <returns>pMCDatabase</returns>
        public static pMCDatabase FromFileInResources(
            string photonDatabaseFileName,
            string collisionInfoDatabaseFileName, 
            string projectName)
        {
            var photonDatabase = PhotonDatabase
                .FromFileInResources(photonDatabaseFileName, projectName);

            var collisionInfoDatabase = CollisionInfoDatabase
                .FromFileInResources(collisionInfoDatabaseFileName, projectName);

            var pMCDatabase = new pMCDatabase(photonDatabase, collisionInfoDatabase);

            // sew the two things together in a lightweight pMCDataPoint object
            // (only exists in memory, the two database records are stored separately)
            var zippedDataPoints = Enumerable.Zip(
                photonDatabase.DataPoints,
                collisionInfoDatabase.DataPoints,
                (photonDataPoint, collisionInfo) => new pMCDataPoint(photonDataPoint, collisionInfo));

            pMCDatabase.SetDataPoints(zippedDataPoints);

            return pMCDatabase;
        }
    }
}
