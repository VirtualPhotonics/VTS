using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Controllers
{
    public class DatabaseWriterController
    {
        IList<PhotonDatabaseWriter> _photonDatabaseWriters;

        public DatabaseWriterController(IList<PhotonDatabaseWriter> photonDatabaseWriters)
        {
            _photonDatabaseWriters = photonDatabaseWriters;
        }

        public IList<PhotonDatabaseWriter> PhotonDatabaseWriters { get { return _photonDatabaseWriters; } set { _photonDatabaseWriters = value; } }

        /// <summary>
        /// Method to write to all surface VB databases
        /// </summary>
        /// <param name="photonDatabaseWriters"></param>
        /// <param name="dp"></param>
        public void WriteToSurfaceVirtualBoundaryDatabases(PhotonDataPoint dp)
        {
            foreach (var writer in _photonDatabaseWriters)
            {
                if (DPBelongsToSurfaceVirtualBoundary(dp, writer))
                {
                    writer.Write(dp);
                }
            };
        }

        /// <summary>
        /// Method to determine if photon datapoint should be tallied or not
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="photonDatabaseWriter"></param>
        /// <returns></returns>
        public bool DPBelongsToSurfaceVirtualBoundary(PhotonDataPoint dp,
            PhotonDatabaseWriter photonDatabaseWriter)
        {
            if ((dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) &&
                 photonDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.DiffuseReflectance) ||
                (dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary) &&
                 photonDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.DiffuseTransmittance) ||
                (dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) && // pMC uses regular PST
                 photonDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.pMCDiffuseReflectance))
            {
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            foreach (var writer in _photonDatabaseWriters)
            {
                writer.Dispose();
            }
        }

    }
} 
