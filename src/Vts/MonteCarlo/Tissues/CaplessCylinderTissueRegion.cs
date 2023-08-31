using System;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueRegion.  Defines cylindrical region with axis along
    /// z-axis and dimensions
    /// Center, Radius and Height with NO CAPS (used for BoundingCylinderTissue).
    /// </summary>
    public class CaplessCylinderTissueRegion : ITissueRegion
    {
        private bool _onBoundary = false;
        /// <summary>
        /// CaplessCylinderTissueRegion assumes cylinder axis is parallel with z-axis
        /// </summary>
        /// <param name="center">center position</param>
        /// <param name="radius">radius in x-y plane</param>
        /// <param name="height">height along z axis</param>
        /// <param name="op">optical properties of cylinder</param>
        public CaplessCylinderTissueRegion(Position center, double radius, double height, OpticalProperties op) 
        {
            TissueRegionType = "CaplessCylinder";
            Center = center;
            Radius = radius;
            Height = height;
            RegionOP = op;
        }
        /// <summary>
        /// default constructor
        /// </summary>
        public CaplessCylinderTissueRegion() : this(new Position(0, 0, 5), 1, 5, 
            new OpticalProperties(0.01, 1.0, 0.8, 1.4)) {}

        /// <summary>
        /// tissue region identifier
        /// </summary>
        public string TissueRegionType { get; set; }

        /// <summary>
        /// center of cylinder
        /// </summary>
        public Position Center { get; set; }
        /// <summary>
        /// radius of cylinder
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// height of cylinder
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// optical properties of cylinder
        /// </summary>
        public OpticalProperties RegionOP { get; set; }
        
        /// <summary>
        /// method to determine if photon position within or on cylinder
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>Boolean</returns>
        public bool ContainsPosition(Position position)
        {
            var inside = Math.Sqrt(position.X * position.X + position.Y * position.Y);
            // check if within radius
            if (inside < 0.9999999999 * Radius)  return true;
            if (inside > 1.00000000001 * Radius)  return false; 
            // on boundary means cylinder contains position
            _onBoundary = true; 
            return true;
        }
        /// <summary>
        /// Method to determine if photon on boundary of cylinder.
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>Boolean</returns>
        public bool OnBoundary(Position position)
        {
            // need to call ContainsPosition because it sets _onBoundary
            // note this method different than other TissueRegion OnBoundary due
            // to used to bound tissue and photon dead when outside this region
            return ContainsPosition(position) && _onBoundary;
        }

        /// <summary>
        /// method to determine if photon ray (or track) will intersect boundary of cylinder
        /// without caps
        /// </summary>
        /// <param name="photon">photon position, direction, etc.</param>
        /// <param name="distanceToBoundary">distance to boundary</param>
        /// <returns>Boolean</returns>
        public bool RayIntersectBoundary(Photon photon, out double distanceToBoundary)
        {
            distanceToBoundary = double.PositiveInfinity;
            _onBoundary = false; // reset _onBoundary
            var dp = photon.DP;
            var p1 = dp.Position;
            var d1 = dp.Direction;

            // determine location of end of ray
            var p2 = new Position(p1.X + d1.Ux * photon.S,
                p1.Y + d1.Uy * photon.S,
                p1.Z + d1.Uz * photon.S);

            var oneIn = this.ContainsPosition(p1);
            var twoIn = this.ContainsPosition(p2);

            // check if ray within cylinder
            if ((oneIn || _onBoundary) && twoIn)
            {
                return false;
            }
            _onBoundary = false; // reset flag

            // distanceToSides is initialized to double.PositiveInfinity at start of RayIntersect
            // first check if intersect with infinite cylinder
            var intersectSides = (CylinderTissueRegionToolbox.RayIntersectInfiniteCylinder(
                p1, p2, oneIn, CylinderTissueRegionAxisType.Z, Center, Radius, out var distanceToSides));

            if (!intersectSides) return false;

            distanceToBoundary = distanceToSides;
            return true;

        }
        /// <summary>
        /// method to determine normal to surface at given position. Cylinder has axis along
        /// z-axis so normal has z component = 0.  Note this returns outward facing normal.
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>Direction normal to surface at position</returns>
        public Direction SurfaceNormal(Position position)
        {
            var norm = Math.Sqrt(4 * (position.X - Center.X) * (position.X - Center.X) +
                                 4 * (position.Y - Center.Y) * (position.Y - Center.Y));
            return new Direction(
                2 * (position.X - Center.X) / norm,
                2 * (position.Y - Center.Y) / norm,
                0.0);
        }
    }
}
