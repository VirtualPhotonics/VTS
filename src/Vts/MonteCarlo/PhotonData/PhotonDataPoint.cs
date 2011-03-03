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
        private double _TotalTime;
        private PhotonStateType _StateFlag;

        public PhotonDataPoint(
            Position position,
            Direction direction,
            double weight,
            double totalTime,
            PhotonStateType stateFlag)
        {
            _Position = position;
            _Direction = direction;
            _Weight = weight;
            _TotalTime = totalTime;
            _StateFlag = stateFlag;
        }

        public Position Position { get { return _Position; } set { _Position = value; } }
        public Direction Direction { get { return _Direction; } set { _Direction = value; } }

        public double Weight { get { return _Weight; } set { _Weight = value; } }
        public double TotalTime { get { return _TotalTime; } set { _TotalTime = value; } }
        public PhotonStateType StateFlag { get { return _StateFlag; } set { _StateFlag = value; } }

        public PhotonDataPoint Clone()
        {
            return new PhotonDataPoint(
                new Position(Position.X, Position.Y, Position.Z),
                new Direction(Direction.Ux, Direction.Uy, Direction.Uz),
                Weight,
                TotalTime,
                StateFlag);
        }

    }
}
