using System;
using Vts.Common;

namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// RayDataPoint is the subset of the Ray-tracing tool ray data output that
    /// is necessary to MCCL.
    /// </summary>
    public class RayDataPoint
    {
        private Position _position;
        private Direction _direction;
        private double _weight;
        
        /// <summary>
        /// Defines Ray Data class
        /// </summary>
        public RayDataPoint( 
            Position position,
            Direction direction,
            double weight)
        {
            _position = position;
            _direction = direction;
            _weight = weight;
        }
        /// <summary>
        /// position of photon source data point
        /// </summary>
        public Position Position { get { return _position; } set { _position = value; } }
        /// <summary>
        /// direction of photon source data point
        /// </summary>
        public Direction Direction { get { return _direction; } set { _direction = value; } }
        /// <summary>
        /// photon initial weight
        /// </summary>
        public double Weight { get { return _weight; } set { _weight = value; } }
    }
}
