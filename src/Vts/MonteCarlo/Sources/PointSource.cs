using System;
using Vts.Common;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Sources
{   
    /// <summary>
    /// Point source, collimated, isotropic or eminating from a solid angle
    /// Notes: possible update: its up to caller to determine acceptable 
    /// theta/phi range to pass specular rather than in GetNextPhoton.
    /// </summary>
    public class PointSource : ISource
    {
        private bool _tallyMomentumTransfer;

        public PointSource( 
            Position pointLocation,
            Direction solidAngleAxis,
            DoubleRange thetaRange, 
            DoubleRange phiRange,
            bool tallyMomentumTransfer) 
        {
            PointLocation = pointLocation;
            SolidAngleAxis = solidAngleAxis;
            ThetaRange = thetaRange;
            PhiRange = phiRange;
            _tallyMomentumTransfer = tallyMomentumTransfer;
        }

        public PointSource()
            : this(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                new DoubleRange(0.0, 0, 1),
                new DoubleRange(0.0, 0, 1),
                false) { }

        public PointSource(PointSourceInput psi, bool tallyMomentumTransfer) : this(
            psi.PointLocation,
            psi.SolidAngleAxis,
            psi.ThetaRange,
            psi.PhiRange,
            tallyMomentumTransfer)
        {
        }

        public Photon Photon { get; set; }
        Position PointLocation { get; set; }
        Direction SolidAngleAxis { get; set; }
        DoubleRange ThetaRange { get; set; }
        DoubleRange PhiRange { get; set; }

        public Photon GetNextPhoton(ITissue tissue, Random rng)
        {
            var p = new Position(0, 0, 0);
            var d = new Direction(0, 0, 1);

            var _photon = new Photon(p, d, tissue, MonteCarloSimulation.ABSORPTION_WEIGHTING, rng, _tallyMomentumTransfer);

            // the following is not general enough
            if ((tissue.OnDomainBoundary(_photon))&&
                (tissue.Regions[0].RegionOP.N != tissue.Regions[1].RegionOP.N))
                _photon.DP.Weight = 1.0 - 
                    Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N);
             //don't call RNG if true point source (this aligns sequence with linux for debug)
            if (ThetaRange.Delta != 0.0)
            {
                Photon.DP.Direction = SourceToolbox.GetAngleDistributedDirection(SolidAngleAxis, ThetaRange, PhiRange, rng);
            }
            return _photon;
        }
    }
}
