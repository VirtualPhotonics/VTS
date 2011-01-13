namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// A struct to hold pathlength and collision count for each subregion of a PhotonDataPoint.
    /// </summary>
    public class SubRegionCollisionInfo
    {
        private double _PathLength;
        private long _NumberOfCollisions;

        public SubRegionCollisionInfo(double pathLength, long numberOfCollisions)
        {
            _PathLength = pathLength;
            _NumberOfCollisions = numberOfCollisions;
        }

        public double PathLength { get { return _PathLength; } set { _PathLength = value; } }
        public long NumberOfCollisions { get { return _NumberOfCollisions; } set { _NumberOfCollisions = value; } }
    }
}
