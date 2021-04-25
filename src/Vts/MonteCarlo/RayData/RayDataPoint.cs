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
        private Position _Position;
        private Direction _Direction;
        private double _Weight;
        
        /// <summary>
        /// Defines Ray Data class
        public RayDataPoint( 
            Position position,
            Direction direction,
            double weight)
        {
            _Position = position;
            _Direction = direction;
            _Weight = weight;
        }
        /// <summary>
        /// position of photon source data point
        /// </summary>
        public Position Position { get { return _Position; } set { _Position = value; } }
        /// <summary>
        /// direction of photon source data point
        /// </summary>
        public Direction Direction { get { return _Direction; } set { _Direction = value; } }
        /// <summary>
        /// photon initial weight
        /// </summary>
        public double Weight { get { return _Weight; } set { _Weight = value; } }
    }
}
