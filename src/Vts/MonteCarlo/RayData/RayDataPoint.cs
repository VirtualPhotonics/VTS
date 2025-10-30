using Vts.Common;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Captures data describing current state of incoming ray and outgoing photons
    /// to become rays
    /// </summary>
    public class RayDataPoint
    {
        /// <summary>
        /// Ray information for outgoing photons to become rays
        /// </summary>
        /// <param name="positionX">X coordinate of position</param>
        /// <param name="positionY">Y coordinate of position</param>
        /// <param name="positionZ">Z coordinate of position</param>
        /// <param name="directionUx">Ux coordinate of direction</param>
        /// <param name="directionUy">Uy coordinate of direction</param>
        /// <param name="directionUz">Uz coordinate of direction</param>
        /// <param name="weight">weight</param>
        /// <param name="totalTime">total time in tissue</param>
        public RayDataPoint(
            double positionX,
            double positionY,
            double positionZ,
            double directionUx,
            double directionUy,
            double directionUz,
            double weight,
            double totalTime)
        {
            Position = new Position(positionX, positionY, positionZ);
            Direction = new Direction(directionUx, directionUy, directionUz);
            Weight = weight;
            TotalTime = totalTime;
        }

        /// Ray information for incoming rays 
        /// <summary>
        /// <param name="positionX">X coordinate of position</param>
        /// <param name="positionY">Y coordinate of position</param>
        /// <param name="positionZ">Z coordinate of position</param>
        /// <param name="directionUx">Ux coordinate of direction</param>
        /// <param name="directionUy">Uy coordinate of direction</param>
        /// <param name="directionUz">Uz coordinate of direction</param>
        /// <param name="weight">weight</param>
        /// </summary>
        public RayDataPoint(
            double positionX,
            double positionY,
            double positionZ,
            double directionUx,
            double directionUy,
            double directionUz,
            double weight)
        {
            Position = new Position(positionX, positionY, positionZ);
            Direction = new Direction(directionUx, directionUy, directionUz);
            Weight = weight;
        }

        /// <summary>
        /// position of ray data point
        /// </summary>
        public Position Position { get; set; }
        /// <summary>
        /// direction of ray data point
        /// </summary>
        public Direction Direction { get; set; }
        /// <summary>
        /// weight of ray
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// total time of ray in tissue
        /// </summary>
        public double TotalTime { get; set; }

        /// <summary>
        /// method to clone ray data point
        /// </summary>
        /// <returns>RayDataPoint class clone</returns>
        public RayDataPoint Clone()
        {
            return new RayDataPoint(
                Position.X, Position.Y, Position.Z,
                Direction.Ux, Direction.Uy, Direction.Uz,
                Weight, TotalTime);
        }

    }
}
