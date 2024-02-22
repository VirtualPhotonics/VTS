using System;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueRegion.  Defines cylindrical region infinite along y-axis with center at (xc,zc).
    /// </summary>
    public class InfiniteCylinderTissueRegion : ITissueRegion
    {
        private bool _onBoundary;

        /// <summary>
        /// CylinderTissueRegion assumes cylinder axis is parallel with z-axis
        /// </summary>
        /// <param name="center">center position</param>
        /// <param name="radius">radius in x-y plane</param>
        /// <param name="op">optical properties of cylinder</param>
        public InfiniteCylinderTissueRegion(Position center, double radius, OpticalProperties op) 
        {
            TissueRegionType = "InfiniteCylinder";
            Center = center;
            Radius = radius;
            RegionOP = op;
        }

        /// <summary>
        /// default constructor
        /// </summary>
        public InfiniteCylinderTissueRegion() : this(new Position(0, 0, 5), 1, 
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
        /// optical properties of cylinder
        /// </summary>
        public OpticalProperties RegionOP { get; set; }
        
        /// <summary>
        /// Method to determine if photon position within or on cylinder.  The loss of precision in floating
        /// point operations necessitates the checks of if "inside" is close but not exact
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>Boolean</returns>
        public bool ContainsPosition(Position position)
        {
            // wrote following to give "buffer" of error
            var deltaR = Math.Sqrt((position.X - Center.X) * (position.X - Center.X) +
                                   (position.Z - Center.Z) * (position.Z - Center.Z)) - Radius;

            switch (deltaR)
            {
                // the epsilon needs to match MultiConcentricInfiniteCylinder
                // GetDistanceToBoundary or code goes through cycles at cylinder boundary            
                case < -1e-9:
                    return true;
                case > 1e-9:
                    return false;
                default:
                    _onBoundary = true;
                    return true;  // ckh 2/28/19 this has to return true or unit tests fail
            }
        }

        /// <summary>
        /// Method to determine if photon on boundary of infinite cylinder.
        /// Currently OnBoundary of an inclusion region isn't called by any code ckh 3/5/19.
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>Boolean</returns>
        public bool OnBoundary(Position position)
        {
            return !ContainsPosition(position) && _onBoundary; // match with EllipsoidTissueRegion
        }

        /// <summary>
        /// method to determine normal to surface at given position. Note this returns outward facing normal.
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>Direction normal to surface at position</returns>
        public Direction SurfaceNormal(Position position)
        {
            var dx = position.X - Center.X;
            var dz = position.Z - Center.Z;
            var norm = Math.Sqrt(dx * dx + dz * dz);
            return new Direction(dx / norm, 0, dz / norm);
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
        /// and modified to assume cylinder infinite along y-axis
        /// </summary>
        /// <param name="photon">photon position, direction, etc.</param>
        /// <param name="distanceToBoundary">return: distance to boundary</param>
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

            return (CylinderTissueRegionToolbox.RayIntersectInfiniteCylinder(p1, p2, oneIn,
                CylinderTissueRegionAxisType.Y, Center, Radius,
                out distanceToBoundary));
        }
    }
}
