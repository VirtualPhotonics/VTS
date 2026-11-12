using System;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueRegion.  Defines cylindrical region finite along z-axis with Center at (xc,yc,zc)
    /// Radius and Height where cylinder caps on planes (zc-Height/2) and (zc+Height/2).
    /// </summary>
    public class CylinderTissueRegion : ITissueRegion
    {
        private const double SidesTolerance = 1e-8;
        private const double CapTolerance = 1e-13;
        private bool _onBoundary;

        /// <summary>
        /// CylinderTissueRegion assumes cylinder axis is parallel with z-axis
        /// </summary>
        /// <param name="center">center position</param>
        /// <param name="radius">radius in x-y plane</param>
        /// <param name="height">height along z axis</param>
        /// <param name="op">optical properties of cylinder</param>
        public CylinderTissueRegion(Position center, double radius, double height, OpticalProperties op)
        {
            TissueRegionType = "Cylinder";
            Center = center;
            Radius = radius;
            Height = height;
            RegionOP = op;
        }
        /// <summary>
        /// default constructor
        /// </summary>
        public CylinderTissueRegion() : this(new Position(0, 0, 5), 1, 5, 
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
        /// Method to determine if photon position within or on cylinder.  This works if height=0
        /// as long as Center.Z=0.
        /// True is returned if photon is within or on boundary of cylinder, false if outside.
        /// _onBoundary is set to true if photon is on boundary, false otherwise.
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>Boolean</returns>
        public bool ContainsPosition(Position position)
        {
            var deltaR = Math.Sqrt(position.X * position.X + position.Y * position.Y) - Radius;
            if (Math.Abs(position.Z - (Center.Z - Height / 2)) < CapTolerance || 
                Math.Abs(position.Z - (Center.Z + Height / 2)) < CapTolerance)
            {
                _onBoundary = true;
                return true;
            }
            // use <= and >= to include points on caps of cylinder, but not sides (sidesTolerance used for sides)
            var betweenCaps = position.Z >= Center.Z - Height / 2 && position.Z <= Center.Z + Height / 2;
            if (!betweenCaps)
            {
                _onBoundary = false;
                return false;
            }
            switch (deltaR)
            {
                case < -SidesTolerance:
                    return true;
                case > SidesTolerance:
                    return false;
                default:
                    _onBoundary = true;
                    return true;
            }
        }

        /// <summary>
        /// Method to determine if photon on boundary of cylinder.
        /// ContainsPosition will return true if on boundary and set _onBoundary to true, so this
        /// will return false if strictly on boundary.  This is consistent with other tissue regions.
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>Boolean</returns>
        public bool OnBoundary(Position position)
        {
            return !ContainsPosition(position) && _onBoundary;
        }

        /// <summary>
        /// Method to determine if photon ray (or track) will intersect boundary of cylinder
        /// equations to determine intersection are derived by parameterizing ray from p1 to p2
        /// as p2=p1+[dx dy dz]t t in [0,1] where dx=p2.x-p1.x dy=p2.y-p1.y dz=p2.z-p2.z
        /// and substituting into circle equations and solving quadratic in t, i.e. t1, t2
        /// t1,t2 less than 0 or t1,t2 greater than 1 => no intersection
        /// 0 less than t1 less than 1 => one intersection
        /// 0 less than t2 less than 1 => one intersections, if above line true too => two intersections
        /// Equations obtained from pdf at https://mrl.nyu.edu/~dzorin/rendering/lectures/lecture3/lecture3-6pp.pdf
        /// and modified to assume cylinder finite along z-axis with caps in x-y planes.
        /// Note: can't vouch for this code yet, especially if photon intersects sides AND cap
        /// </summary>
        /// <param name="photon">photon position, direction, etc.</param>
        /// <param name="distanceToBoundary">distance to boundary</param>
        /// <returns>Boolean indicating intersection or not</returns>
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

            var oneIn = ContainsPosition(p1);
            var twoIn = ContainsPosition(p2);

            // check if ray within cylinder
            if ((oneIn || _onBoundary) && twoIn)
            {
                return false;
            }
            _onBoundary = false; // reset flag

            var distanceToSides = double.PositiveInfinity;
            // first check if intersect with *infinite cylinder* with axis along z-axis
            var intersectSides = CylinderTissueRegionToolbox.RayIntersectInfiniteCylinder(p1, p2, oneIn,
                CylinderTissueRegionAxisType.Z, Center, Radius,
                out var distanceToInfiniteCylinder);
            if (intersectSides)
            {
                // determine where on infinite cylinder intersection is and check if
                // *not* within finite cylinder (between caps)
                var zIntercept = p1.Z + distanceToInfiniteCylinder * d1.Uz;
                if (zIntercept > Center.Z - Height / 2 && zIntercept < Center.Z + Height / 2)
                {
                    distanceToSides = distanceToInfiniteCylinder;
                }
            }

            var distanceToTopCap = double.PositiveInfinity;
            // determine intersection of plane of top cap with p1 -> p2 or p2 -> p1
            var topCapZPlane = Center.Z - Height / 2;
            if (p1.Z < topCapZPlane && p2.Z > topCapZPlane)
            {
                var t = (topCapZPlane - p1.Z) / d1.Uz;
                var xIntercept = p1.X + t * d1.Ux;
                var yIntercept = p1.Y + t * d1.Uy;
                if (Math.Sqrt((xIntercept - Center.X) * (xIntercept - Center.X) +
                              (yIntercept - Center.Y) * (yIntercept - Center.Y)) < Radius)
                {
                    distanceToTopCap = Math.Abs(t);
                }
            }
            if (p1.Z > topCapZPlane && p2.Z < topCapZPlane)
            {
                var t = (p1.Z - topCapZPlane) / d1.Uz;
                var xIntercept = p1.X + t * d1.Ux;
                var yIntercept = p1.Y + t * d1.Uy;
                if (Math.Sqrt((xIntercept - Center.X) * (xIntercept - Center.X) +
                              (yIntercept - Center.Y) * (yIntercept - Center.Y)) < Radius)
                {
                    distanceToTopCap = Math.Abs(t); // t could be negative but distance always positive
                }
            }
            // determine intersection of plane of bottom cap with p1 -> p2 or p2 -> p1
            var distanceToBottomCap = double.PositiveInfinity;
            var bottomCapZPlane = Center.Z + Height / 2;
            if (p1.Z < bottomCapZPlane && p2.Z > bottomCapZPlane)
            {
                var t = (p1.Z - bottomCapZPlane) / d1.Uz;
                var xIntercept = p1.X + t * d1.Ux;
                var yIntercept = p1.Y + t * d1.Uy;
                if (Math.Sqrt((xIntercept - Center.X) * (xIntercept - Center.X) +
                              (yIntercept - Center.Y) * (yIntercept - Center.Y)) < Radius)
                {
                    distanceToBottomCap = Math.Abs(t); // t could be negative but distance always positive
                }
            }

            if (p1.Z > bottomCapZPlane && p2.Z < bottomCapZPlane)
            {
                var t = (p1.Z - bottomCapZPlane) / d1.Uz;
                var xIntercept = p1.X + t * d1.Ux;
                var yIntercept = p1.Y + t * d1.Uy;
                if (!(Math.Sqrt((xIntercept - Center.X) * (xIntercept - Center.X) +
                                (yIntercept - Center.Y) * (yIntercept - Center.Y)) < Radius)) return false;
                distanceToBottomCap = Math.Abs(t); // t could be negative but distance always positive
            }
            distanceToBoundary = Math.Min(distanceToSides, Math.Min(distanceToTopCap, distanceToBottomCap));
            return !double.IsPositiveInfinity(distanceToBoundary);
        }

        /// <summary>
        /// Method to determine normal to surface at given position.  This method only gets called
        /// if photon sitting on the boundary of the cylinder so needs to return some Direction
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>Direction normal to surface at position</returns>
        public Direction SurfaceNormal(Position position)
        {
            var onCylinder = Math.Abs(
                Math.Sqrt((position.X - Center.X) * (position.X - Center.X) + 
                          (position.Y - Center.Y) * (position.Y - Center.Y)) - Radius) < SidesTolerance;
            var betweenCaps = position.Z > Center.Z - Height / 2 && position.Z < Center.Z + Height / 2;
            if (onCylinder && betweenCaps)
            {
                // Return the normal vector for the cylindrical surface
                var norm = Math.Sqrt(4 * (position.X - Center.X) * (position.X - Center.X) +
                                     4 * (position.Y - Center.Y) * (position.Y - Center.Y));
                return new Direction(
                    2 * (position.X - Center.X) / norm,
                    2 * (position.Y - Center.Y) / norm,
                    0.0);
            }
            var inRadius = Math.Sqrt((position.X - Center.X) * (position.X - Center.X) + 
                                          (position.Y - Center.Y) * (position.Y - Center.Y)) < Radius;
            var onTopCap = Math.Abs(position.Z - (Center.Z - Height / 2)) < CapTolerance;
            var onBottomCap = Math.Abs(position.Z - (Center.Z + Height / 2)) < CapTolerance;
            if (onTopCap && inRadius) return new Direction(0, 0, -1); // top cap normal points up
            // must be on bottom cap
            return new Direction(0, 0, 1);
        }
    }
}
