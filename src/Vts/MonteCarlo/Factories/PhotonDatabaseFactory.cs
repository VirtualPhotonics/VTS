using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Factories
{
    public class PhotonDatabaseFactory
    {
        public static PhotonDatabase GetPhotonDatabase(
            VirtualBoundaryType virtualBoundaryType, string filePath, string fileName)
        {
            switch (virtualBoundaryType)
            {
                case VirtualBoundaryType.DiffuseReflectance:
                    return PhotonDatabase.FromFile(Path.Combine(filePath, fileName));
                case VirtualBoundaryType.DiffuseTransmittance:
                    return PhotonDatabase.FromFile(Path.Combine(filePath, fileName));
                case VirtualBoundaryType.SpecularReflectance:
                    return PhotonDatabase.FromFile(Path.Combine(filePath, fileName));
                case VirtualBoundaryType.pMCDiffuseReflectance:
                    return PhotonDatabase.FromFile(Path.Combine(filePath, fileName));
                default:
                    return null;
            }
        }

        public static pMCDatabase GetpMCDatabase(
            VirtualBoundaryType virtualBoundaryType, ITissue tissue, string filePath, 
            string photonDatabaseFilename, string collisionInfoFilename)
        {
            switch (virtualBoundaryType)
            {
                case VirtualBoundaryType.pMCDiffuseReflectance:
                    return pMCDatabase.FromFile(Path.Combine(filePath, photonDatabaseFilename),
                        Path.Combine(filePath, collisionInfoFilename));
                default:
                    return null;
            }
        }

    }
} 
