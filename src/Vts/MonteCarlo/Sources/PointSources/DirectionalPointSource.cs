using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class DirectionalPointSource : PointSourceBase
    {
        /// <summary>
        /// Returns an instance of Directional Point Source with a given emission direction at a given location
        /// </summary>
        /// <param name="pointLocation">New position</param>
        /// <param name="direction">Photon emitting direction</param>
        public DirectionalPointSource(
            Position pointLocation,
            Direction direction)
            : base(
                new DoubleRange (0.0, 0.0),
                new DoubleRange(0.0, 0.0),
                pointLocation,
                SourceToolbox.GetPolarAndAzimuthalAnglesFromDirection(direction))
        {
        }

        /// <summary>
        /// Returns an instance of Directional Point Source at a given location
        /// </summary>
        /// <param name="pointLocation">New position</param>
        public DirectionalPointSource(Position pointLocation)
            : this(pointLocation, new Direction(0.0, 0.0, 1.0))
        {
        }

        /// <summary>
        /// Returns an instance of Directional Point Source with a given emission direction
        /// </summary>
        /// <param name="direction">Photon emitting direction</param>
        public DirectionalPointSource(
            Direction direction)
            : this( new Position(0.0, 0.0, 0.0), direction)
        {
        }

        /// <summary>
        ///  Returns an instance of Directional Point Source emitting along the z-axis at the origin.
        /// </summary>
        public DirectionalPointSource()
            : this(new Position(0.0, 0.0, 0.0), new Direction(0.0, 0.0, 1.0))
        {
        }
    }
}
