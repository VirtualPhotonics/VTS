using System;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// A directional line source of a given length, centered at the specified position, 
    /// oriented along one specified direction with light directed along another specified direction
    /// </summary>
    public class CollimatedLineSource : LineSourceBase
    {
        /// <summary>
        /// Creates a diretional line source of a given length, centered at the specified position, 
        /// and oriented along the specified direction.
        /// </summary>
        /// <param name="center">The center position of the line source</param>
        /// <param name="lineAxis">The axis of the line source. (Must be normalized!)</param>
        /// <param name="photonDirection">Direction of collimated light generated</param>
        /// <param name="length">the length of the line source.</param>
        public CollimatedLineSource(Position center, Direction lineAxis, Direction photonDirection, double length)
            : base(center, lineAxis, length)
        {
            PhotonDirection = photonDirection;
        }

        /// <summary>
        /// Creates a default 80mm collimated line source along the y-axis
        /// </summary>
        public CollimatedLineSource()
            : this(new Position(0, 0, 0), new Direction(0, 1, 0), new Direction(0, 0, 1), 80) { }

        // todo: need helper CollimatedLineSourceInput constructor

        /// <summary>
        /// Direction of collimated light output
        /// </summary>
        public Direction PhotonDirection { get; private set; }

        /// <summary>
        /// This generates a photon, randomly oriented radially, along the line
        /// </summary>
        /// <returns>Photon</returns>
        public override Photon GetNextPhoton(ITissue tissue)
        {
            var photonPosition = SourceToolbox.GetRandomLinePosition(Position, Orientation, Length, Rng);

            var dataPoint = new PhotonDataPoint(
                    photonPosition,
                    PhotonDirection,
                    // the handling of specular needs work
                    1.0 - Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N),
                    PhotonStateType.NotSet,
                    Enumerable.Range(0, tissue.Regions.Count).Select(i => new SubRegionCollisionInfo(0.0, 0)).ToArray());

            var photon = new Photon { DP = dataPoint };

            return photon;
        }
    }
}

