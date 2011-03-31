using System;
using Vts.Common;

namespace Vts.MonteCarlo.Sources
{   
    /// <summary>
    /// Point source, collimated, isotropic or eminating from a solid angle
    /// Notes: possible update: its up to caller to determine acceptable 
    /// theta/phi range to pass specular rather than in GetNextPhoton.
    /// </summary>
    public class CustomPointSourceOld : SourceBaseOld
    {
        /// <summary>
        /// Creates a PointSource with user-specified details
        /// </summary>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        /// <param name="thetaRange"></param>
        /// <param name="phiRange"></param>
        public CustomPointSourceOld( 
            Position position,
            Direction orientation,
            DoubleRange thetaRange, 
            DoubleRange phiRange)
            : base(position, orientation)
        {
            ThetaRange = thetaRange;
            PhiRange = phiRange;
        }

        /// <summary>
        /// Creates a default CustomPointSource with unit normal pointing in the positive z direction
        /// </summary>
        public CustomPointSourceOld()
            : this(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                new DoubleRange(0.0, 0, 1),
                new DoubleRange(0.0, 0, 1)) { }

        /// <summary>
        /// Creates a PointSource, based on a PointSourceInput data transfer object)
        /// </summary>
        /// <param name="psi">The point source input DTO</param>
        /// <remarks>This really should be logic in a factory class</remarks>
        public CustomPointSourceOld(CustomPointSourceInputOld cpsi) : this(
            cpsi.PointLocation,
            cpsi.SolidAngleAxis,
            cpsi.ThetaRange,
            cpsi.PhiRange)
        {
        }

        public DoubleRange ThetaRange { get; protected set; }
        public DoubleRange PhiRange { get; protected set; }

        public override Photon GetNextPhoton(ITissue tissue)
        {
            //var p = new Position(0, 0, 0);
            //var d = new Direction(0, 0, 1);
            var p = Position.Clone();
            var d = Orientation.Clone();

            var _photon = new Photon(p, d, tissue, Rng);

            // the following is not general enough
            if ((tissue.OnDomainBoundary(_photon)) &&
                (tissue.Regions[0].RegionOP.N != tissue.Regions[1].RegionOP.N))
                _photon.DP.Weight = 1.0 - Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N);

           

            return _photon;
        }
    }
}
