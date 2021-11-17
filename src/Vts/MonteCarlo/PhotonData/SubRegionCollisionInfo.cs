namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// A struct to hold pathlength and collision count for each subregion of a PhotonDataPoint.
    /// </summary>
    public class SubRegionCollisionInfo
    {

        /// <summary>
        /// SubRegionCollisionInfo collects the number of collision and total path length
        /// at the current PhotonDataPoint position.
        /// </summary>
        /// <param name="pathLength">total path length in subregion</param>
        /// <param name="numberOfCollisions">total number of collisions in subregion</param>
        public SubRegionCollisionInfo(double pathLength, long numberOfCollisions)
        {
            PathLength = pathLength;
            NumberOfCollisions = numberOfCollisions;
        }

        /// <summary>
        /// path length of photon in sub-region
        /// </summary>
        public double PathLength { get; set; }
        /// <summary>
        /// number of collisions made by photon in sub-region
        /// </summary>
        public long NumberOfCollisions { get; set; }

        /// <summary>
        /// Method to clone class
        /// </summary>
        /// <returns>instantiated clone of class</returns>
        public SubRegionCollisionInfo Clone()
        {
            return new SubRegionCollisionInfo(PathLength, NumberOfCollisions);
        }
    }
}
