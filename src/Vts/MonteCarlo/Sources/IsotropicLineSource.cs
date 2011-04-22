using System;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// A radially-isotropic line source of a given length, centered at the specified position, 
    /// and oriented along the specified direction.
    /// </summary>
    public class IsotropicLineSource : LineSourceBase
    {
        /// <summary>
        /// Creates a radially-isotropic line source of a given length, centered at the specified position, 
        /// and oriented along the specified direction.
        /// </summary>
        /// <param name="center">The center position of the line source</param>
        /// <param name="lineAxis">The axis of the line source. (Must be normalized!)</param>
        /// <param name="length">the length of the line source.</param>
        public IsotropicLineSource(Position center, Direction lineAxis, double length)
            : base(center, lineAxis, length)
        {
        }

        /// <summary>
        /// Creates a default 80mm line source along the y-axis
        /// </summary>
        public IsotropicLineSource() 
            : this(new Position(0, 0, 0), new Direction(0, 1, 0), 80) { }

        // todo: need helper IsotropicLineSourceInput constructor

        /// <summary>
        /// This generates a photon, randomly oriented radially, along the line
        /// </summary>
        /// <returns>Photon</returns>
        public override Photon GetNextPhoton(ITissue tissue)
        {
            var photonPosition = SourceToolbox.GetRandomLinePosition(Position, Orientation, Length, Rng);
            
            // create a random radial direction along axis orthogonal to line direction
            var photonDirection = SourceToolbox.GetRandomRadialDirection(Orientation, Rng);

            var dataPoint = new PhotonDataPoint(
                    photonPosition,
                    photonDirection,
                    // the handling of specular needs work
                    1.0 - Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N),
                    0.0,
                    PhotonStateType.Alive);

            var photon = new Photon { DP = dataPoint };

            return photon;
        }
    }
}

