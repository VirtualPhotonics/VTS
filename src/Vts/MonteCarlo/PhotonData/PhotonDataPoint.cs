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
        /// <summary>
        /// Photon information updated during its trajectory through tissue
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="direction">Direction</param>
        /// <param name="weight">weight</param>
        /// <param name="totalTime">total time of flight</param>
        /// <param name="stateFlag">PhotonStateType</param>
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

        /// <summary>
        /// position of photon data point
        /// </summary>
        public Position Position { get { return _Position; } set { _Position = value; } }
        /// <summary>
        /// direction of photon data point
        /// </summary>
        public Direction Direction { get { return _Direction; } set { _Direction = value; } }
        /// <summary>
        /// weight of photon
        /// </summary>
        public double Weight { get { return _Weight; } set { _Weight = value; } }
        /// <summary>
        /// total time to date that photon has lived
        /// </summary>
        public double TotalTime { get { return _TotalTime; } set { _TotalTime = value; } }
        /// <summary>
        /// state flag indicating state of photon
        /// </summary>
        public PhotonStateType StateFlag { get { return _StateFlag; } set { _StateFlag = value; } }

        /// <summary>
        /// method to clone photon data point
        /// </summary>
        /// <returns></returns>
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
