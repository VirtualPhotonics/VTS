using System;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Planar, collimated circular source defined by center and radius 
    /// in a fixed direction.
    /// </summary>
    public class CollimatedRectangularSource : SourceBase
    {
        /// <summary>
        /// Creates a planar, collimated rectangular source defined by lengthX and lengthY, oriented
        /// in a single direction.
        /// </summary>
        /// <param name="position">The center location of the rectangular source</param>
        /// <param name="orientation">The normal vector of the rectangular source (must be normalized)</param>
        /// <param name="lengthX">The x length</param>
        /// <param name="lengthY">The y length</param>
        public CollimatedRectangularSource(Position position, Direction orientation, double lengthX, double lengthY)
            : base(position, orientation)
        {
            LengthX = lengthX;
            LengthY = lengthY;
        }

        public CollimatedRectangularSource()
            : this(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                10,
                10
            ) { }

        public double LengthX { get; protected set; }

        public double LengthY { get; protected set; }
        
        public override Photon GetNextPhoton(ITissue tissue)
        {
            var photon = new Photon();

            photon.DP = new PhotonDataPoint(
                    Position,
                    Orientation,
                    // the handling of specular needs work
                    1.0 - Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N),
                    0.0,
                    PhotonStateType.NotSet);

            photon.DP.Position = SourceToolbox.GetRandomFlatRectangularPosition(Position, LengthX, LengthY, Rng);

            return photon;
        }
    }
}
