using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Factory methods to provide the PhotonDatabaseWriter (or list of PhotonDatabaseWriters)
    /// or CollisionInfoDatabaseWriter (or list).
    /// </summary>
    public class DatabaseWriterFactory
    {
        public static IList<PhotonDatabaseWriter> GetSurfaceVirtualBoundaryDatabaseWriters(
            IList<VirtualBoundaryType> virtualBoundaryTypes, string filePath, string outputName)
        {
            return virtualBoundaryTypes.Select(v => GetSurfaceVirtualBoundaryDatabaseWriter(v,
                filePath, outputName)).ToList();
        
        }
        public static PhotonDatabaseWriter GetSurfaceVirtualBoundaryDatabaseWriter(
            VirtualBoundaryType virtualBoundaryType, string filePath, string outputName)
        {
            switch (virtualBoundaryType)
            {
                default:
                case VirtualBoundaryType.DiffuseReflectance:
                    return new PhotonDatabaseWriter(VirtualBoundaryType.DiffuseReflectance,
                        Path.Combine(filePath, outputName, "DiffuseReflectanceDatabase"));
                case VirtualBoundaryType.DiffuseTransmittance:
                    return new PhotonDatabaseWriter(VirtualBoundaryType.DiffuseTransmittance,
                        Path.Combine(filePath, outputName, "DiffuseTransmittanceDatabase"));
                case VirtualBoundaryType.SpecularReflectance:
                    return new PhotonDatabaseWriter(VirtualBoundaryType.SpecularReflectance,
                        Path.Combine(filePath, outputName, "SpecularReflectanceDatabase"));
                case VirtualBoundaryType.pMCDiffuseReflectance:
                    return new PhotonDatabaseWriter(VirtualBoundaryType.pMCDiffuseReflectance,
                        Path.Combine(filePath, outputName, "DiffuseReflectanceDatabase"));
            }
        }
        public static IList<CollisionInfoDatabaseWriter> GetCollisionInfoDatabaseWriters(
            IList<VirtualBoundaryType> virtualBoundaryTypes, ITissue tissue, string filePath, string outputName)
        {
            return virtualBoundaryTypes.Select(v => GetCollisionInfoDatabaseWriter(v, tissue,
                filePath, outputName)).ToList();

        }
        public static CollisionInfoDatabaseWriter GetCollisionInfoDatabaseWriter(
            VirtualBoundaryType virtualBoundaryType, ITissue tissue, string filePath, string outputName)
        {
            switch (virtualBoundaryType)
            {
                default:
                case VirtualBoundaryType.pMCDiffuseReflectance:
                    return new CollisionInfoDatabaseWriter(VirtualBoundaryType.pMCDiffuseReflectance,
                        Path.Combine(filePath, outputName, "CollisionInfoDatabase"), 
                        tissue.Regions.Count());
            }
        }

    }
} 
