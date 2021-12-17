using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Describes database for storing and returning source Zemax ZRD ray data points 
    /// (position, direction, weight).
    /// The base class, Database(OfT), exposes the IEnumerable(OfT) DataPoints 
    /// list of RayDataPoint items
    /// </summary>
    public class ZRDRayDatabase  : Database<ZRDRayDataPoint> 
    {
        /// <summary>
        /// Returns an instance of SourceDatabase
        /// </summary>
        public ZRDRayDatabase()
        {
        }

        /// <summary>
        /// method to read ZRDRayDatabase from file
        /// </summary>
        /// <param name="fileName">filename of ZRDRayDatabase</param>
        /// <returns></returns>
        public static ZRDRayDatabase FromFile(string fileName)
        {
            var dbReader = new DatabaseReader<ZRDRayDatabase, ZRDRayDataPoint>(
                db => new ZRDRayDataPointSerializer());

            return dbReader.FromFile(fileName);
        }

    }
}
