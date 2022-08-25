using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.Zemax
{
    /// <summary>
    /// This is code that would be executed to convert ZRD DB to/from MCCL compatible DB
    /// Describes database for storing and returning source Zemax ZRD ray data points 
    /// (position, direction, weight).
    /// The base class, Database(OfT), exposes the IEnumerable(OfT) DataPoints 
    /// list of RayDataPoint items
    /// </summary>
    public class ZrdRayDatabase  : Database<ZrdRayDataPoint> 
    {
        /// <summary>
        /// Returns an instance of SourceDatabase
        /// </summary>
        public ZrdRayDatabase()
        {
        }

        /// <summary>
        /// method to read ZrdRayDatabase from file
        /// </summary>
        /// <param name="fileName">filename of ZrdRayDatabase</param>
        /// <returns>database of zemax rays</returns>
        public static ZrdRayDatabase FromFile(string fileName)
        {
            var dbReader = new DatabaseReader<ZrdRayDatabase, ZrdRayDataPoint>(
                db => new ZrdRayDataPointSerializer());

            return dbReader.FromFile(fileName);
        }

    }
}
