using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vts.MonteCarlo;

namespace Vts.Zemax
{
    /// <summary>
    /// This is code that would be executed to convert ZRD DB to/from MCCL compatible DB
    /// Factory methods to provide the PhotonDatabaseWriter (or list of PhotonDatabaseWriters)
    /// or CollisionInfoDatabaseWriter (or list).
    /// </summary>
    public class ZrdDatabaseWriterFactory
    {
        /// <summary>
        /// Static method to provide list of ZrdRayDatabaseWriters.  It calls the method
        /// to instantiate one CollisionInfoDatabaseWriter, GetCollisionInfoDatabaseWriter,
        /// for all elements in the list of virtual boundary DatabaseType. 
        /// </summary>
        /// <param name="databaseTypes">list of database types</param>
        /// <param name="filePath">path string of database output</param>
        /// <param name="outputName">filename string of output file</param>
        /// <returns>a list of CollisionInfoDatabaseWriter</returns>
        public static IList<ZrdRayDatabaseWriter> GetZrdSurfaceVirtualBoundaryDatabaseWriters(
            IList<ZemaxDatabaseType> databaseTypes, string filePath, string outputName)
        {
            return databaseTypes.Select(v => GetZrdSurfaceVirtualBoundaryDatabaseWriter(v,
                filePath, outputName)).ToList();

        }
        /// <summary>
        /// Static method to instantiate correct ZrdRayDatabaseWriter given a 
        /// virtual boundary DatabaseType, path to where to output database and database filename.
        /// </summary>
        /// <param name="databaseType">database type enum</param>
        /// <param name="filePath">path string of database output</param>
        /// <param name="outputName">filename string of database file</param>
        /// <returns>a ZrdRayDatabaseWriter</returns>
        public static ZrdRayDatabaseWriter GetZrdSurfaceVirtualBoundaryDatabaseWriter(
            ZemaxDatabaseType databaseType, string filePath, string outputName)
        {
            switch (databaseType)
            {
                default:
                case ZemaxDatabaseType.ZrdDiffuseReflectance:
                    return new ZrdRayDatabaseWriter(VirtualBoundaryType.DiffuseReflectance,
                        Path.Combine(filePath, outputName, "ZrdDiffuseReflectanceDatabase"));
            }
        }

    }
} 
