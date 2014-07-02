using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Describes database for storing and returning collision info (pathlength &amp; number of collisions).
    /// The base class, Database(OfT), exposes the IEnumerable(OfT) DataPoints list of CollisionInfo items
    /// </summary>
    public class CollisionInfoDatabase : Database<CollisionInfo> 
    {
        /// <summary>
        /// Creates an instance of a CollisionInfoDatabase
        /// </summary>
        /// <param name="numSubRegions">The number of "sub-regions" within the tissue</param>
        public CollisionInfoDatabase(int numSubRegions)
        {
            NumberOfSubRegions = numSubRegions;
        }

        /// <summary>
        /// Do not use this overload, it is only for serialization purposes
        /// </summary>
        public CollisionInfoDatabase() : this(0) { } // only needed for serialization

        /// <summary>
        /// The number of "sub-regions" within the tissue
        /// </summary>
        public int NumberOfSubRegions { get; set; }

        /// <summary>
        /// Static helper method to simplify reading from file
        /// </summary>
        /// <param name="fileName">The base filename for the database (no ".txt")</param>
        /// <returns>A new instance of CollisionInfoDatabase</returns>
        public static CollisionInfoDatabase FromFile(string fileName)
        {
            var dbReader = new DatabaseReader<CollisionInfoDatabase, CollisionInfo>(
                db => new CollisionInfoSerializer(db.NumberOfSubRegions));

            return dbReader.FromFile(fileName);
        }

        /// <summary>
        /// Static helper method to simplify reading from file
        /// </summary>
        /// <param name="fileName">The base filename for the database (no ".txt")</param>
        /// <param name="projectName">The project name containing the resource</param>
        /// <returns>A new instance of CollisionInfoDatabase</returns>
        public static CollisionInfoDatabase FromFileInResources(string fileName, string projectName)
        {
            var dbReader = new DatabaseReader<CollisionInfoDatabase, CollisionInfo>(
                db => new CollisionInfoSerializer(db.NumberOfSubRegions));

            return dbReader.FromFileInResources(fileName, projectName);
        }
    }
}
