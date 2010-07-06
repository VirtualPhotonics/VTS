using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Captures data describing current state of photon.
    /// </summary>
    public class PhotonDataPoint
    {
        private Position _Position;
        private Direction _Direction;
        private double _Weight;
        private PhotonStateType _StateFlag;
        private IList<SubRegionCollisionInfo> _SubRegionInfoList;

        public PhotonDataPoint(
            Position position,
            Direction direction,
            double weight,
            PhotonStateType stateFlag,
            IList<SubRegionCollisionInfo> subRegionInfoList)
        {
            _Position = position;
            _Direction = direction;
            _Weight = weight;
            _StateFlag = stateFlag;
            _SubRegionInfoList = subRegionInfoList;
        }

        public Position Position { get { return _Position; } set { _Position = value; } }
        public Direction Direction { get { return _Direction; } set { _Direction = value; } }

        public double Weight { get { return _Weight; } set { _Weight = value; } }
        public PhotonStateType StateFlag { get { return _StateFlag; } set { _StateFlag = value; } }
        public IList<SubRegionCollisionInfo> SubRegionInfoList { get { return _SubRegionInfoList; } set { _SubRegionInfoList = value; } }

    }
}
