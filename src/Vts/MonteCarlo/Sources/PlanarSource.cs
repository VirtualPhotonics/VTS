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
    public class PlanarSource : ISource
    {
        private Position _center;
        private double _radius;
        private Direction _direction;

        public PlanarSource(
            Position center, 
            Direction direction, 
            double radius) 
        {
            _center = center;
            _direction = direction;
            _radius = radius;
        }
        public PlanarSource()
            : this(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                10) { }

        public Photon Photon { get; set; }

        public Photon GetNextPhoton(ITissue tissue, Random rng)
        {
            var Photon = new Photon();
            Photon.DP = new PhotonDataPoint(
                    new Position(),
                    _direction,
                    // the handling of specular needs work
                    1.0 - Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N),
                    PhotonStateType.NotSet,
                    Enumerable.Range(0, tissue.Regions.Count).Select(i => new SubRegionCollisionInfo(0.0, 0)).ToArray());
            Photon.DP.Position = SourceToolbox.GetFlatCircularPosition(_center, _radius, rng);
            return Photon;
        }
    }
}
