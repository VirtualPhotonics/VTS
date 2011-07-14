using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.VirtualBoundaries;

namespace Vts.MonteCarlo.Extensions
{
    /// <summary>
    /// Methods used to write to surface or volume virtual boundary databases
    /// </summary>
    public static class PhotonDataExtensionMethods
    {
        public static void WriteToSurfaceVirtualBoundaryDatabases(
            this IList<PhotonDatabaseWriter> photonDatabaseWriters, PhotonDataPoint dp)
        {
            foreach (var writer in photonDatabaseWriters)
            {
                WriteToSurfaceVirtualBoundaryDatabase(writer, dp);
            };
        }
        public static void WriteToSurfaceVirtualBoundaryDatabase(
            this PhotonDatabaseWriter photonDatabaseWriter, PhotonDataPoint dp)
        {
            if (dp.BelongsToSurfaceVirtualBoundary(photonDatabaseWriter))
            {
                photonDatabaseWriter.Write(dp);
            }
        }
        public static bool BelongsToSurfaceVirtualBoundary(this PhotonDataPoint dp, 
            PhotonDatabaseWriter photonDatabaseWriter)
        {
            if ((dp.StateFlag.Has(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) &&
                 photonDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.DiffuseReflectance) ||
                (dp.StateFlag.Has(PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary) &&
                 photonDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.DiffuseTransmittance) ||
                (dp.StateFlag.Has(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) && // pMC uses regular PST
                 photonDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.pMCDiffuseReflectance))
            {    
                return true;
            }
            return false;
        }
        public static void WriteToCollisionInfoDatabases(
            this IList<CollisionInfoDatabaseWriter> collisionInfoDatabaseWriters, PhotonDataPoint dp, CollisionInfo collisionInfo)
        {
            foreach (var writer in collisionInfoDatabaseWriters)
            {
                WriteToCollisionInfoDatabase(writer, dp, collisionInfo);
            };
        }
        public static void WriteToCollisionInfoDatabase(
            this CollisionInfoDatabaseWriter collisionInfoDatabaseWriter, PhotonDataPoint dp, CollisionInfo collisionInfo)
        {
            if (dp.BelongsToSurfaceVirtualBoundary(collisionInfoDatabaseWriter))
            {
                collisionInfoDatabaseWriter.Write(collisionInfo);
            }
        }
        public static bool BelongsToSurfaceVirtualBoundary(this PhotonDataPoint dp,
            CollisionInfoDatabaseWriter collisionInfoDatabaseWriter)
        {
            if ((dp.StateFlag.Has(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) &&
                 collisionInfoDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.DiffuseReflectance) ||
                (dp.StateFlag.Has(PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary) &&
                 collisionInfoDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.DiffuseTransmittance) ||
                (dp.StateFlag.Has(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) && // pMC uses regular PST
                 collisionInfoDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.pMCDiffuseReflectance))
            {
                return true;
            }
            return false;
        }
        
        public static void Dispose(
            this IList<PhotonDatabaseWriter> photonDatabaseWriters)
        {
            foreach (var writer in photonDatabaseWriters)
            {
                writer.Dispose();
            }
        }
        public static void Dispose(
            this IList<CollisionInfoDatabaseWriter> collisionInfoDatabaseWriters)
        {
            foreach (var writer in collisionInfoDatabaseWriters)
            {
                writer.Dispose();
            }
        }
    }
}
