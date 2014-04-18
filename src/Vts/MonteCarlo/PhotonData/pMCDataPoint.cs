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
        /// <param name="photonDataPoint">photon data point</param>
        /// <param name="collisionInfo">collision information</param>
        public pMCDataPoint(PhotonDataPoint photonDataPoint, CollisionInfo collisionInfo)
        {
            PhotonDataPoint = photonDataPoint;
            CollisionInfo = collisionInfo;
        }

        /// <summary>
        /// photon data point
        /// </summary>
        public PhotonDataPoint PhotonDataPoint { get; set; }
        /// <summary>
        /// collision information
        /// </summary>
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
