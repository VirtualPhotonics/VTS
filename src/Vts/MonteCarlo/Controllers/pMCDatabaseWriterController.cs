using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Controllers
{
    /// <summary>
    /// A controller of DatabaseWriter(s) for perturbation Monte Carlo (pMC).  It handles 
    /// determining whether data should be written,
    /// and if so, writing the data, and finally disposing of the database.
    /// </summary>
    public class pMCDatabaseWriterController
    {
        IList<PhotonDatabaseWriter> _photonDatabaseWriters;
        IList<CollisionInfoDatabaseWriter> _collisionInfoDatabaseWriters;

        /// <summary>
        /// constructor for pMC database writer controller
        /// </summary>
        /// <param name="photonDatabaseWriters">list of photon database writers</param>
        /// <param name="collisionInfoDatabaseWriters">list of collision info database writers</param>
        public pMCDatabaseWriterController(
            IList<PhotonDatabaseWriter> photonDatabaseWriters,
            IList<CollisionInfoDatabaseWriter> collisionInfoDatabaseWriters)
        {
            _photonDatabaseWriters = photonDatabaseWriters;
            _collisionInfoDatabaseWriters = collisionInfoDatabaseWriters;
        }
        /// <summary>
        /// list of photon database writers
        /// </summary>
        public IList<PhotonDatabaseWriter> PhotonDatabaseWriters { get { return _photonDatabaseWriters; } set { _photonDatabaseWriters = value; } }
        /// <summary>
        /// list of collision info database writers
        /// </summary>
        public IList<CollisionInfoDatabaseWriter> CollisionInfoDatabaseWriters { get { return _collisionInfoDatabaseWriters; } set { _collisionInfoDatabaseWriters = value; } }

        /// <summary>
        /// Method to write to all surface VB databases
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <param name="collisionInfo">collision information</param>
        public void WriteToSurfaceVirtualBoundaryDatabases(PhotonDataPoint dp, CollisionInfo collisionInfo)
        {
            var writeData = false;
            foreach (var writer in _photonDatabaseWriters)
            {
                if (DPBelongsToSurfaceVirtualBoundary(dp, writer))
                {
                    writer.Write(dp);
                    writeData = true;
                }
            }; 
            // not best design but may work for now
            foreach (var writer in _collisionInfoDatabaseWriters)
            {
                if (writeData)
                {
                    writer.Write(collisionInfo);
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
            if ((dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) && // pMC uses regular PST
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
            foreach (var writer in _collisionInfoDatabaseWriters)
            {
                writer.Dispose();
            }
        }
      
    }
} 
