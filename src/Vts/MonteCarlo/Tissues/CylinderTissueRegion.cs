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
        private double _topCapPlane;
        private double _bottomCapPlane;
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
            _topCapPlane = Center.Z + Height / 2;
            _bottomCapPlane = Center.Z - Height / 2;
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
            if (((Math.Sqrt(position.X * position.X + position.Y * position.Y) <= Radius)) &&
                (position.Z <= Center.Z + Height / 2) &&
                (position.Z >= Center.Z - Height / 2))
                return true;
            else
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
            //return ((position.Z == Center.Z + Height) || (position.Z == Center.Z - Height)) &&
            //    Math.Sqrt(position.X * position.X + position.Y * position.Y) == Radius;
            return !ContainsPosition(position) && _onBoundary; // match with EllipsoidTissueRegion
        }

        /// <summary>
        /// Method to determine if photon ray (or track) will intersect boundary of cylinder
        /// equations to determine intersection are derived by parameterizing ray from p1 to p2
        /// as p2=p1+[dx dy dz]t t in [0,1] where dx=p2.x-p1.x dy=p2.y-p1.y dz=p2.z-p2.z
        /// and substituting into ellipsoid equations and solving quadratic in t, i.e. t1, t2
        /// t1,t2<0 or t1,t2>1 => no intersection
        /// 0<t1<1 => one intersection
        /// 0<t2<1 => one intersections, if above line true too => two intersections
        /// Equations obtained from pdf at https://mrl.nyu.edu/~dzorin/rendering/lectures/lecture3/lecture3-6pp.pdf
        /// and modified to assume cylinder finite along z-axis with caps in x-y planes 
        /// </summary>
        /// <param name="photon">photon position, direction, etc.</param>
        /// <param name="distanceToBoundary">distance to boundary</param>
        /// <returns>boolean</returns>
        public bool RayIntersectBoundary(Photon photon, out double distanceToBoundary)
        {
            // first check if ray intersect infinite cylinder, then check if between planes of caps
            distanceToBoundary = double.PositiveInfinity;
            double distanceToCap1 = double.PositiveInfinity;
            double distanceToCap2 = double.PositiveInfinity;
            double root1, root2, xto, yto, zto;
            double root = 0;
            var dp = photon.DP;
            var p1 = dp.Position;
            var d1 = dp.Direction;

            // determine location of end of ray
            var p2 = new Position(p1.X + d1.Ux * photon.S,
                                  p1.Y + d1.Uy * photon.S,
                                  p1.Z + d1.Uz * photon.S);
            if (Height == 0) // check if cylinder is infinitely thin disk at surface
            {
                // check if intersection with one cap (they should be equal)
                if (RayIntersectCap(_topCapPlane, p1, p2, d1, out distanceToBoundary))
                {
                    _onBoundary = false;
                    return true;
                }
            }

            bool one_in = this.ContainsPosition(p1);
            bool two_in = this.ContainsPosition(p2);

            // check if ray within cylinder
            if ((one_in || _onBoundary) && two_in)
            {
                return false;
            }
            _onBoundary = false; // reset flag

            double dx = (p2.X - p1.X);
            double dy = (p2.Y - p1.Y);
            double dz = (p2.Z - p1.Z);

            double A = dx * dx + dy * dy;

            double B =
                2 * (p1.X - Center.X) * dx +
                2 * (p1.Y - Center.Y) * dy;

            double C =
                (p1.X - Center.X) * (p1.X - Center.X) +
                (p1.Y - Center.Y) * (p1.Y - Center.Y) - Radius * Radius;

            double rootTerm = B * B - 4 * A * C;

            if (rootTerm > 0)  // roots are real 
            {
                double rootTermSqrt = Math.Sqrt(rootTerm);
                root1 = (-B - rootTermSqrt) / (2 * A);
                root2 = (-B + rootTermSqrt) / (2 * A);

                int numint = 0; //number of intersections

                if ((root1 < 1) && (root1 > 0))
                {
                    numint += 1;
                    root = root1;
                }

                if ((root2 < 1) && (root2 > 0))
                {
                    numint += 1;
                    root = root2;
                }

                switch (numint)
                {
                    case 0: /* roots real but no intersection */
                        // check if intersection with caps
                        if (RayIntersectCap(_topCapPlane, p1, p2, d1, out distanceToCap1) ||
                            RayIntersectCap(_bottomCapPlane, p1, p2, d1, out distanceToCap2))
                        {
                            distanceToBoundary = (distanceToCap1 < distanceToCap2) ? distanceToCap1 : distanceToCap2;
                            return true;
                        }
                        return false;
                    case 1:
                        if ((!one_in) && (Math.Abs(root) < 1e-7))
                        {
                            return false;
                        }
                        /*entering or exiting cylinder. It's the same*/
                        xto = p1.X + root * dx;
                        yto = p1.Y + root * dy;
                        zto = p1.Z + root * dz;

                        // check if zto is between caps
                        if ((zto > Center.Z - Height / 2) && (zto < Center.Z + Height / 2))
                        {
                            /*distance to the boundary*/
                            distanceToBoundary = Math.Sqrt((xto - p1.X) * (xto - p1.X) +
                                                           (yto - p1.Y) * (yto - p1.Y) +
                                                           (zto - p1.Z) * (zto - p1.Z));

                            //// ckh fix 8/25/11: check if on boundary of cylinder
                            if (distanceToBoundary < 1e-11)
                            {
                                return false;
                            }

                            return true;
                        }
                        else // zto is not between caps
                        {
                            // check if intersection with caps
                            if (RayIntersectCap(_topCapPlane, p1, p2, d1, out distanceToCap1) ||
                                RayIntersectCap(_bottomCapPlane, p1, p2, d1, out distanceToCap2))
                            {
                                distanceToBoundary = (distanceToCap1 < distanceToCap2) ? distanceToCap1 : distanceToCap2;
                                return true;
                            }
                            return false;
                        }

                    case 2:  /* went through cylinder: must stop at nearest intersection */
                        ///*which is nearest?*/
                        if (one_in)
                        {
                            if (root1 > root2)
                                root = root1;
                            else root = root2;
                        }
                        else
                        {
                            if (root1 < root2)
                                root = root1;
                            else root = root2;
                        }
                        xto = p1.X + root * dx;
                        yto = p1.Y + root * dy;
                        zto = p1.Z + root * dz;

                        /*distance to the nearest boundary*/
                        distanceToBoundary = Math.Sqrt((xto - p1.X) * (xto - p1.X) +
                                                       (yto - p1.Y) * (yto - p1.Y) +
                                                       (zto - p1.Z) * (zto - p1.Z));

                        return true;

                } /* end switch */

            } /* BB-4AC>0 */

            /* roots imaginary -> no intersection with sides */
            // check if intersection with caps
            if (RayIntersectCap(_topCapPlane, p1, p2, d1, out distanceToCap1) ||
                RayIntersectCap(_bottomCapPlane, p1, p2, d1, out distanceToCap2))
            {
                distanceToBoundary = (distanceToCap1 < distanceToCap2) ? distanceToCap1 : distanceToCap2;
                return true;
            }
            return false;
        }

        public bool RayIntersectCap(double capZ, Position p1, Position p2, Direction d1, out double distanceToBoundary)
        {
            if (((p1.Z >= capZ) && (p2.Z <= capZ)) || ((p1.Z <= capZ) && (p2.Z >= capZ))) // pointed up or down
            {
                distanceToBoundary = (capZ - p1.Z) / d1.Uz; // denom and numer are either both pos or both neg
                double xTo = p1.X + distanceToBoundary * d1.Ux;
                double yTo = p1.Y + distanceToBoundary * d1.Uy;
                return (xTo * xTo + yTo * yTo < Radius);
            }
            else
            {
                distanceToBoundary = double.PositiveInfinity;
                return false;
            }
        }

        //public bool RayIntersectBoundary(Photon photptr)
        //{
        //    throw new NotImplementedException();
        //}

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
