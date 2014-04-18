using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Extensions
{
    /// <summary>
    /// Methods used to write to surface or volume virtual boundary databases
    /// </summary>
    public static class PhotonDataExtensionMethods
    {

        /// <summary>
        /// Method to write to pMC surface VB databases, calls singular method below for a list.
        /// </summary>
        /// <param name="collisionInfoDatabaseWriters">list of CollisionInfoDatabaseWriters</param>
        /// <param name="dp">PhotonDataPoint</param>
        /// <param name="collisionInfo">CollisionInfo</param>
        public static void WriteToPMCSurfaceVirtualBoundaryDatabases(
            this IList<CollisionInfoDatabaseWriter> collisionInfoDatabaseWriters, PhotonDataPoint dp, CollisionInfo collisionInfo)
        {
            foreach (var writer in collisionInfoDatabaseWriters)
            {
                WriteToPMCSurfaceVirtualBoundaryDatabase(writer, dp, collisionInfo);
            };
        }
        /// <summary>
        /// Method to write to pMC surface VB database
        /// </summary>
        /// <param name="collisionInfoDatabaseWriter">CollisionInfoDatabaseWriter</param>
        /// <param name="dp">PhotonDataPoint</param>
        /// <param name="collisionInfo">CollisionInfo</param>
        public static void WriteToPMCSurfaceVirtualBoundaryDatabase(
            this CollisionInfoDatabaseWriter collisionInfoDatabaseWriter, PhotonDataPoint dp, CollisionInfo collisionInfo)
        {
            if (dp.BelongsToSurfaceVirtualBoundary(collisionInfoDatabaseWriter))
            {
                collisionInfoDatabaseWriter.Write(collisionInfo);
            }
        }
        /// <summary>
        /// method to determine whether photon database belongs to surface virtual boundary
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <param name="collisionInfoDatabaseWriter">collision info database writer</param>
        /// <returns>boolean</returns>
        public static bool BelongsToSurfaceVirtualBoundary(this PhotonDataPoint dp,
            CollisionInfoDatabaseWriter collisionInfoDatabaseWriter)
        {
            if ((dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) &&
                 collisionInfoDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.DiffuseReflectance) ||
                (dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary) &&
                 collisionInfoDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.DiffuseTransmittance) ||
                (dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) && // pMC uses regular PST
                 collisionInfoDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.pMCDiffuseReflectance))
            {
                return true;
            }
            return false;
        }
        


    }
}
