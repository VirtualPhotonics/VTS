using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Captures data describing current state of photon.
    /// </summary>
    public class pMCDataPoint
    {
        /// <summary>
        /// class to house data needed for pMC database.
        /// </summary>
        /// <param name="photonDataPoint">PhotonDataPoint</param>
        /// <param name="collisionInfo">CollisionInfo</param>
        public pMCDataPoint(PhotonDataPoint photonDataPoint, CollisionInfo collisionInfo)
        {
            PhotonDataPoint = photonDataPoint;
            CollisionInfo = collisionInfo;
        }

        public PhotonDataPoint PhotonDataPoint { get; set; }
        public CollisionInfo CollisionInfo { get; set; }

        /// <summary>
        /// Method to clone class
        /// </summary>
        /// <returns>instantiated clone of class</returns>
        public pMCDataPoint Clone()
        {
            return new pMCDataPoint(PhotonDataPoint.Clone(), CollisionInfo.Clone());
        }
    }
}
