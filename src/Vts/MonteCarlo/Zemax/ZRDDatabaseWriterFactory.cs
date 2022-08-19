using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vts.MonteCarlo.Zemax
{
    /// <summary>
    /// This is code that would be executed to convert ZRD DB to/from MCCL compatible DB
    /// Factory methods to provide the PhotonDatabaseWriter (or list of PhotonDatabaseWriters)
    /// or CollisionInfoDatabaseWriter (or list).
    /// </summary>
    public class ZRDDatabaseWriterFactory
    {
        /// <summary>
        /// Static method to provide list of ZRDRayDatabaseWriters.  It calls the method
        /// to instantiate one CollisionInfoDatabaseWriter, GetCollisionInfoDatabaseWriter,
        /// for all elements in the list of virtual boundary DatabaseType. 
        /// </summary>
        /// <param name="databaseTypes">list of database types</param>
        /// <param name="filePath">path string of database output</param>
        /// <param name="outputName">filename string of output file</param>
        /// <returns>a list of CollisionInfoDatabaseWriter</returns>
        public static IList<ZRDRayDatabaseWriter> GetZRDSurfaceVirtualBoundaryDatabaseWriters(
            IList<DatabaseType> databaseTypes, string filePath, string outputName)
        {
            return databaseTypes.Select(v => GetZRDSurfaceVirtualBoundaryDatabaseWriter(v,
                filePath, outputName)).ToList();

        }
        /// <summary>
        /// Static method to instantiate correct ZRDRayDatabaseWriter given a 
        /// virtual boundary DatabaseType, path to where to output database and database filename.
        /// </summary>
        /// <param name="databaseType">database type enum</param>
        /// <param name="filePath">path string of database output</param>
        /// <param name="outputName">filename string of database file</param>
        /// <returns>a ZRDRayDatabaseWriter</returns>
        public static ZRDRayDatabaseWriter GetZRDSurfaceVirtualBoundaryDatabaseWriter(
            DatabaseType databaseType, string filePath, string outputName)
        {
            switch (databaseType)
            {
                default:
                case DatabaseType.ZRDDiffuseReflectance:
                    return new ZRDRayDatabaseWriter(VirtualBoundaryType.DiffuseReflectance,
                        Path.Combine(filePath, outputName, "ZRDDiffuseReflectanceDatabase"));
            }
        }

    }
} 
