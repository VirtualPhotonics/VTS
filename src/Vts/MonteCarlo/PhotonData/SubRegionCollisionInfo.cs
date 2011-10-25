namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// A struct to hold pathlength and collision count for each subregion of a PhotonDataPoint.
    /// </summary>
    public class SubRegionCollisionInfo
    {
        private double _PathLength;
        private long _NumberOfCollisions;

        /// <summary>
        /// SubRegionCollisionInfo collects the number of collision and total path length
        /// at the current PhotonDataPoint position.
        /// </summary>
        /// <param name="pathLength">total path length in subregion</param>
        /// <param name="numberOfCollisions">total number of collisions in subregion</param>
        public SubRegionCollisionInfo(double pathLength, long numberOfCollisions)
        {
            _PathLength = pathLength;
            _NumberOfCollisions = numberOfCollisions;
        }

        public double PathLength { get { return _PathLength; } set { _PathLength = value; } }
        public long NumberOfCollisions { get { return _NumberOfCollisions; } set { _NumberOfCollisions = value; } }

        /// <summary>
        /// Method to clone class
        /// </summary>
        /// <returns>instantiated clone of class</returns>
        public SubRegionCollisionInfo Clone()
        {
            return new SubRegionCollisionInfo(_PathLength, _NumberOfCollisions);
        }
    }
}
