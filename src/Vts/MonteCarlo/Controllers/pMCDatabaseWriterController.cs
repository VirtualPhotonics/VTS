﻿using System.Collections.Generic;
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

        /// <summary>
        /// constructor for pMC database writer controller
        /// </summary>
        /// <param name="photonDatabaseWriters">list of photon database writers</param>
        /// <param name="collisionInfoDatabaseWriters">list of collision info database writers</param>
        public pMCDatabaseWriterController(
            IList<PhotonDatabaseWriter> photonDatabaseWriters,
            IList<CollisionInfoDatabaseWriter> collisionInfoDatabaseWriters)
        {
            PhotonDatabaseWriters = photonDatabaseWriters;
            CollisionInfoDatabaseWriters = collisionInfoDatabaseWriters;
        }
        /// <summary>
        /// list of photon database writers
        /// </summary>
        public IList<PhotonDatabaseWriter> PhotonDatabaseWriters { get; set; }
        /// <summary>
        /// list of collision info database writers
        /// </summary>
        public IList<CollisionInfoDatabaseWriter> CollisionInfoDatabaseWriters { get; set; }

        /// <summary>
        /// Method to write to all surface VB databases
        /// </summary>
        /// <param name="dp">photon data point</param>
        /// <param name="collisionInfo">collision information</param>
        public void WriteToSurfaceVirtualBoundaryDatabases(PhotonDataPoint dp, CollisionInfo collisionInfo)
        {
            var writeData = false;
            foreach (var writer in PhotonDatabaseWriters)
            {
                if (DPBelongsToSurfaceVirtualBoundary(dp, writer))
                {
                    writer.Write(dp);
                }
            }; 
            // not best design but may work for now
            foreach (var writer in CollisionInfoDatabaseWriters)
            {
                if (DPBelongsToSurfaceVirtualBoundary(dp, writer))
                {
                    writer.Write(collisionInfo);
                }
            }
        }

        /// <summary>
        /// Method to determine if photon data point should be tallied or not
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="photonDatabaseWriter"></param>
        /// <returns></returns>
        public bool DPBelongsToSurfaceVirtualBoundary(PhotonDataPoint dp,
            PhotonDatabaseWriter photonDatabaseWriter)
        {
            if (((dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) && // pMC uses regular PST
                 photonDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.pMCDiffuseReflectance)) ||
                ((dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary) && // 
                  photonDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.pMCDiffuseTransmittance)))
            {
                return true;
            }
            return false;
        }        
        /// <summary>
        /// Method to determine if photon datapoint should be tallied or not to
        /// the collision info database
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="collisionInfoDatabaseWriter"></param>
        /// <returns></returns>
        public bool DPBelongsToSurfaceVirtualBoundary(PhotonDataPoint dp,
            CollisionInfoDatabaseWriter collisionInfoDatabaseWriter)
        {
            if (((dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary) && // pMC uses regular PST
                  collisionInfoDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.pMCDiffuseReflectance)) ||
                ((dp.StateFlag.HasFlag(PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary) && // 
                  collisionInfoDatabaseWriter.VirtualBoundaryType == VirtualBoundaryType.pMCDiffuseTransmittance)))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// method to dispose of photon database writers
        /// </summary>
        public void Dispose()
        {
            foreach (var writer in PhotonDatabaseWriters)
            {
                writer.Dispose();
            }
            foreach (var writer in CollisionInfoDatabaseWriters)
            {
                writer.Dispose();
            }
        }
      
    }
} 
