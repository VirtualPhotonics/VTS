using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Captures data describing current state of photon.
    /// </summary>
    public class pMCDataPoint
    {
        public pMCDataPoint(PhotonDataPoint photonDataPoint, CollisionInfo collisionInfo)
        {
            PhotonDataPoint = photonDataPoint;
            CollisionInfo = collisionInfo;
        }

        public PhotonDataPoint PhotonDataPoint { get; set; }
        public CollisionInfo CollisionInfo { get; set; }

        public pMCDataPoint Clone()
        {
            return new pMCDataPoint(PhotonDataPoint.Clone(), CollisionInfo.Clone());
        }
    }
}
