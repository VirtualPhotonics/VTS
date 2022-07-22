using Vts.Common;

namespace Vts.MonteCarlo.PhotonData
{
    /// <summary>
    /// Captures data describing current state of photon.
    /// </summary>
    public class PhotonDataPoint
    {

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
            Position = position;
            Direction = direction;
            Weight = weight;
            TotalTime = totalTime;
            StateFlag = stateFlag;
        }

        /// <summary>
        /// position of photon data point
        /// </summary>
        public Position Position { get; set; }
        /// <summary>
        /// direction of photon data point
        /// </summary>
        public Direction Direction { get; set; }
        /// <summary>
        /// weight of photon
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// total time to date that photon has lived
        /// </summary>
        public double TotalTime { get; set; }
        /// <summary>
        /// state flag indicating state of photon
        /// </summary>
        public PhotonStateType StateFlag { get; set; }

        /// <summary>
        /// method to clone photon data point
        /// </summary>
        /// <returns>PhotonDataPoint class clone</returns>
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
