using System;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Collimated line source centered at "center" 
    /// extending in "direction" with "extent" and "width".
    /// </summary>
    public class LineSource : ISource
    {
        private Position _center;
        private Direction _lineSourceDirection;
        private double _width;
        private DoubleRange _extent;

        public LineSource(
            Position center,
            Direction lineSourceDirection,
            double width,
            DoubleRange extent)
        {
            _center = center;
            _lineSourceDirection = lineSourceDirection;
            _width = width;
            _extent = extent;
;
        }
        public LineSource()
            : this(
                new Position(0, 0, 0),
                new Direction(0, 1, 0), // along y-axis
                0.0, // zero width line
                new DoubleRange(-40.0, 40.0, 2)) { }

        public Photon Photon { get; set; }

        /// <summary>
        /// This generates a line source along y-axis.  It is maintained because it is the source
        /// laura used to run her tests
        /// </summary>
        /// <returns>Photon</returns>
        public Photon GetNextPhoton(ITissue tissue, Random rng)
        {
            var Photon = new Photon();
            Photon.DP = new PhotonDataPoint(
                    new Position(),
                    new Direction(0, 0, 1),
                    // the handling of specular needs work
                    1.0 - Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N),
                    PhotonStateType.NotSet,
                    Enumerable.Range(0, tissue.Regions.Count).Select(i => new SubRegionCollisionInfo(0.0, 0)).ToArray());
            // flat long rectangular beam ONLY ALONG Y-AXIS FOR NOW 
            Photon.DP.Position.X = _center.X + _width * (rng.NextDouble() - 0.5); //x=[-beam_radius/2+beam_center_x:beam_radius/2+beam_center_x]
            //Photon.DP.Position.Y = RN1 * 8 - 4; //y=[-4:4 cm]  Laura's code
            Photon.DP.Position.Y = rng.NextDouble() * _extent.Delta;
            Photon.DP.Position.Z = 0;
            return Photon;
        }
    }
}

