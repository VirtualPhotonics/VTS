using System;
using Vts.Common;

namespace Vts.MonteCarlo.Sources
{   
    /// <summary>
    /// Point source, collimated, isotropic or eminating from a solid angle
    /// Notes: possible update: its up to caller to determine acceptable 
    /// theta/phi range to pass specular rather than in GetNextPhoton.
    /// </summary>
    public class CustomPointSource : SourceBase
    {
        /// <summary>
        /// Creates a PointSource with user-specified details
        /// </summary>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        /// <param name="thetaRange"></param>
        /// <param name="phiRange"></param>
        public CustomPointSource( 
            Position position,
            Direction orientation,
            DoubleRange thetaRange, 
            DoubleRange phiRange,
            int startingRegionIndex)
            : base(position, orientation)
        {
            ThetaRange = thetaRange;
            PhiRange = phiRange;
            StartingRegionIndex = startingRegionIndex;
        }

        /// <summary>
        /// Creates a default CustomPointSource with unit normal pointing in the positive z direction
        /// </summary>
        public CustomPointSource()
            : this(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                new DoubleRange(0.0, 0, 1),
                new DoubleRange(0.0, 0, 1),
                0) { }

        /// <summary>
        /// Creates a PointSource, based on a PointSourceInput data transfer object)
        /// </summary>
        /// <param name="psi">The point source input DTO</param>
        /// <remarks>This really should be logic in a factory class</remarks>
        public CustomPointSource(CustomPointSourceInput cpsi) : this(
            cpsi.PointLocation,
            cpsi.SolidAngleAxis,
            cpsi.ThetaRange,
            cpsi.PhiRange,
            cpsi.StartingRegionIndex)
        {
        }

        public DoubleRange ThetaRange { get; protected set; }
        public DoubleRange PhiRange { get; protected set; }
        public int StartingRegionIndex { get; protected set; } // should this be in SourceBase?

        public override Photon GetNextPhoton(ITissue tissue)
        {
            var p = Position.Clone();
            var d = Orientation.Clone();
            // if Position is on boundary of tissue region and if state type is OnBoundary then Photon 
            // determines tissue region that photon starts in by Orientation and where its entering
            // var _photon = new Photon(p, d, tissue, PhotonState.Add(PhotonStateType.OnBoundary, Rng);
            
            // if Position is on boundary of tissue region and if state type is not OnBoundary then
            // Photon determines tissue region that photon starts in by determining region 
            // "behind" it (see Photon)
            var _photon = new Photon(p, d, tissue, StartingRegionIndex, Rng);

            // the following is not general enough
            //if ((tissue.OnDomainBoundary(_photon)) &&
            //    (tissue.Regions[0].RegionOP.N != tissue.Regions[1].RegionOP.N))
            //    _photon.DP.Weight = 1.0 - Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N);

            //don't call RNG if true point source (this aligns sequence with linux for debug)
            if (ThetaRange.Delta != 0.0)
            {
                _photon.DP.Direction = SourceToolbox.GetRandomAngleDistributedDirection(Orientation, ThetaRange, PhiRange, Rng);
            }

            return _photon;
        }
    }
}
