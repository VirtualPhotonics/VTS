using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Describes database for storing and returning MCCL DB source ray data points 
    /// (position, direction, weight).
    /// The base class, Database(OfT), exposes the IEnumerable(OfT) DataPoints 
    /// list of RayDataPoint items
    /// </summary>
    public class RayDatabase  : Database<RayDataPoint> 
    {
        /// <summary>
        /// Returns an instance of SourceDatabase
        /// </summary>
        public RayDatabase()
        {
        }

        /// <summary>
        /// method to read ZRDRayDatabase from file
        /// </summary>
        /// <param name="fileName">filename of RayDatabase</param>
        /// <returns></returns>
        public static RayDatabase FromFile(string fileName)
        {
            var dbReader = new DatabaseReader<RayDatabase, RayDataPoint>(
                db => new RayDataPointSerializer());

            return dbReader.FromFile(fileName);
        }

    }
}
