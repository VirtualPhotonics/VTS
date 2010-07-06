namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// A struct to hold pathlength and collision count for each subregion of a PhotonDataPoint.
    /// </summary>
    public class SubRegionCollisionInfo
    {
        private double _PathLength;
        private long _NumberOfCollisions;

        private bool _TallyMomentumTransfer;
        private double _MomentumTransfer;

        public SubRegionCollisionInfo(double pathLength, long numberOfCollisions, bool tallyMomentumTransfer, double momentumTransfer)
        {
            _PathLength = pathLength;
            _NumberOfCollisions = numberOfCollisions;

            _TallyMomentumTransfer = tallyMomentumTransfer;
            _MomentumTransfer = momentumTransfer;
        }

        public SubRegionCollisionInfo(double pathLength, long numberOfCollisions)
             : this(pathLength, numberOfCollisions, false, 0D)
        {
        }

        public double PathLength { get { return _PathLength; } set { _PathLength = value; } }
        public long NumberOfCollisions { get { return _NumberOfCollisions; } set { _NumberOfCollisions = value; } }

        public bool TallyMomentumTransfer { get { return _TallyMomentumTransfer; } set { _TallyMomentumTransfer = value; } }
        public double MomentumTransfer { get { return _MomentumTransfer; } set { _MomentumTransfer = value; } }
    }
}
