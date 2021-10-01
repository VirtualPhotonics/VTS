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
        private bool _onBoundary = false;

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
        /// as long as Center.Z=0;
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>boolean</returns>
        public bool ContainsPosition(Position position)
        {
            double radialPosition = Math.Sqrt(position.X * position.X + position.Y * position.Y);
            // check if within caps vertically and within radius
            if ((position.Z <= Center.Z + Height / 2) && (position.Z >= Center.Z - Height / 2) &&
                (radialPosition < Radius)) 
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Method to determine if photon on boundary of cylinder.
        /// Currently OnBoundary of an inclusion region isn't called by any code ckh 3/5/19.
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>boolean</returns>
        public bool OnBoundary(Position position)
        {
            bool onCylinder = Math.Abs(
                                  Math.Sqrt(position.X * position.X + position.Y * position.Y) - Radius) < 1e-10;
            bool inRadius = Math.Sqrt(position.X * position.X + position.Y * position.Y) < Radius;
            bool onOneCap = (Math.Abs(position.Z - (Center.Z - Height / 2)) < 1e-10 || 
                    Math.Abs(position.Z - (Center.Z + Height / 2)) < 1e-10) && inRadius;
            return onCylinder || onOneCap;
        }

        /// <summary>
        /// Method to determine if photon ray (or track) will intersect boundary of cylinder
        /// equations to determine intersection are derived by parameterizing ray from p1 to p2
        /// as p2=p1+[dx dy dz]t t in [0,1] where dx=p2.x-p1.x dy=p2.y-p1.y dz=p2.z-p2.z
        /// and substituting into ellipsoid equations and solving quadratic in t, i.e. t1, t2
        /// t1,t2 less than 0 or t1,t2 greater than 1 => no intersection
        /// 0 less than t1 less than 1 => one intersection
        /// 0 less than t2 less than 1 => one intersections, if above line true too => two intersections
        /// Equations obtained from pdf at https://mrl.nyu.edu/~dzorin/rendering/lectures/lecture3/lecture3-6pp.pdf
        /// and modified to assume cylinder finite along z-axis with caps in x-y planes.
        /// Note: can't vouch for this code yet, especially if photon intersects sides AND cap
        /// </summary>
        /// <param name="photon">photon position, direction, etc.</param>
        /// <param name="distanceToBoundary">distance to boundary</param>
        /// <returns>boolean indicating intersection or not</returns>
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

            bool oneIn = this.ContainsPosition(p1);
            bool twoIn = this.ContainsPosition(p2);

            // check if ray within cylinder
            if ((oneIn || _onBoundary) && twoIn)
            {
                return false;
            }
            _onBoundary = false; // reset flag

            // distanceToSides is initialized to double.PositiveInfinity at start of RayIntersect
            // first check if intersect with infinite cylinder
            var intersectSides = (CylinderTissueRegionToolbox.RayIntersectInfiniteCylinder(p1, p2, oneIn,
                CylinderTissueRegionAxisType.Z, Center, Radius,
                out var distanceToSides));
            // then check if intersect caps, create three tissue layers 1) above cylinder, 2) cylinder, 3) below

            var topLayer = new LayerTissueRegion(
                new DoubleRange(0, Center.Z - (Height / 2)),
                new OpticalProperties()); // doesn't matter what OPs are
            var intersectTopLayer = topLayer.RayIntersectBoundary(photon, out var distanceToTopLayer);

            var enclosingLayer =
                new LayerTissueRegion(
                    new DoubleRange(Center.Z - (Height / 2), Center.Z + (Height / 2)),
                    new OpticalProperties());
            var intersectCapLayer = enclosingLayer.RayIntersectBoundary(photon, out var distanceToCapLayer);

            var bottomLayer = new LayerTissueRegion(
                new DoubleRange(Center.Z + (Height / 2), double.PositiveInfinity),
                new OpticalProperties()); // doesn't matter what OPs are
            var intersectBottomLayer = bottomLayer.RayIntersectBoundary(photon, out var distanceToBottomLayer);
            var hitCaps = false;
            double distanceToCap = double.PositiveInfinity;
            if (intersectTopLayer || intersectCapLayer || intersectBottomLayer)
            {
                distanceToCap = Math.Min(distanceToTopLayer, Math.Min(distanceToCapLayer, distanceToBottomLayer));
                double xto = p1.X + distanceToCap * d1.Ux;
                double yto = p1.Y + distanceToCap * d1.Uy;
                double zto = p1.Z + distanceToCap * d1.Uz;
                if ((Math.Abs(zto - (Center.Z + (Height / 2))) < 1e-10 ||
                     Math.Abs(zto - (Center.Z - (Height / 2))) < 1e-10) &&
                    Math.Sqrt(xto * xto + yto * yto) < Radius)
                {
                    hitCaps = true;
                }
            }
            if (hitCaps && distanceToCap < distanceToSides)
            {
                distanceToBoundary = distanceToCap;
                return true;
            }

            if (intersectSides && distanceToSides < distanceToCapLayer)
            {
                distanceToBoundary = distanceToSides;
                return true;
            }

            return false;
        }

        /// <summary>
        /// method to determine normal to surface at given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns>Direction</returns>
        public Direction SurfaceNormal(Position position)
        {
            throw new NotImplementedException();
        }
    }
}
