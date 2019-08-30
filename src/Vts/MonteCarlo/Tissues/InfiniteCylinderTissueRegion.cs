using System;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueRegion.  Defines cylindrical region infinite along y-axis with center at (xc,zc).
    /// </summary>
    public class InfiniteCylinderTissueRegion : ITissueRegion
    {
        private bool _onBoundary = false;
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
        /// method to determine if photon position within or on cylinder
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>boolean</returns>
        public bool ContainsPosition(Position position)
        {
            //return (Math.Sqrt((position.X - Center.X) * (position.X - Center.X) +
            //                  (position.Z - Center.Z) * (position.Z - Center.Z)) < Radius);
            // wrote following to match EllipsoidTissueRegion because it seems to work better than above
            double inside = Math.Sqrt((position.X - Center.X) * (position.X - Center.X) +
                                      (position.Z - Center.Z) * (position.Z - Center.Z));

            //if (inside < 0.9999999)
            if (inside < 0.9999999999 * Radius)
            {
                return true;
            }
            //else if (inside > 1.0000001)
            else if (inside > 1.00000000001 * Radius)
            {
                return false;
            }
            else  // on boundary means cylinder contains position
            {
                _onBoundary = true;
                //return false; // ckh try 8/21/11 
                return true;  // ckh 2/28/19 this has to return true or unit tests fail
            }
        }
        /// <summary>
        /// Method to determine if photon on boundary of infinite cylinder.
        /// Currently OnBoundary of an inclusion region isn't called by any code ckh 3/5/19.
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>boolean</returns>
        public bool OnBoundary(Position position)
        {
            //_onBoundary = false;
            //var surfaceEqn = Math.Sqrt((position.X - Center.X) * (position.X - Center.X) +
            //                           (position.Z - Center.Z) * (position.Z - Center.Z));
            //_onBoundary = (Math.Abs(surfaceEqn - Radius) < 1e-7); // 
            //return _onBoundary ;
            return !ContainsPosition(position) && _onBoundary; // match with EllipsoidTissueRegion
        }
        /// <summary>
        /// method to determine normal to surface at given position. Note this returns outward facing normal.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>Direction</returns>
        public Direction SurfaceNormal(Position position)
        {
            var norm = Math.Sqrt(4 * (position.X - Center.X) * (position.X - Center.X) +
                                 4 * (position.Z - Center.Z) * (position.Z - Center.Z));
            return new Direction(
                2 * (position.X - Center.X) / norm,
                0,
                2 * (position.Z - Center.Z) / norm);
            //throw new NotImplementedException();
        }
        /// <summary>
        /// method to determine if photon ray (or track) will intersect boundary of cylinder
        /// equations to determine intersection are derived by parameterizing ray from p1 to p2
        /// as p2=p1+[dx dy dz]t t in [0,1] where dx=p2.x-p1.x dy=p2.y-p1.y dz=p2.z-p2.z
        /// and substituting into ellipsoid equations and solving quadratic in t, i.e. t1, t2
        /// t1,t2 less than 0 or t1,t2 greater than 1 => no intersection
        /// 0 less than t1 less than 1 => one intersection
        /// 0 less than t2 less than 1 => one intersections, if above line true too => two intersections
        /// </summary>
        /// <param name="photon">photon position, direction, etc.</param>
        /// <param name="distanceToBoundary">distance to boundary</param>
        /// <returns>boolean</returns>
        public bool RayIntersectBoundary(Photon photon, out double distanceToBoundary)
        {
            distanceToBoundary = double.PositiveInfinity;
            _onBoundary = false; // reset _onBoundary
            double root1, root2, xto, yto, zto;
            double root = 0;
            var dp = photon.DP;
            var p1 = dp.Position;
            var d1 = dp.Direction;

            // determine location of end of ray
            var p2 = new Position(p1.X + d1.Ux * photon.S, 
                                  p1.Y + d1.Uy * photon.S, 
                                  p1.Z + d1.Uz * photon.S);

            bool one_in = this.ContainsPosition(p1);
            bool two_in = this.ContainsPosition(p2);
            //if ((Math.Abs(p1.X)<2)&& one_in)
            //{
            //    Console.WriteLine(String.Format("p1.x,y,z={0:F}, {1:F}, {2:F}, in={3}",p1.X,p1.Y,p1.Z,one_in));
            //    Console.WriteLine("****");
            //}

            // check if ray within cylinder
            if ((one_in || _onBoundary) && two_in)
            {
                return false;
            }
            _onBoundary = false; // reset flag

            double dx = (p2.X - p1.X);
            double dy = (p2.Y - p1.Y);
            double dz = (p2.Z - p1.Z);

            double A = dx * dx + dz * dz;

            double B =
                2 * (p1.X - Center.X) * dx + 
                2 * (p1.Z - Center.Z) * dz;

            double C =
                (p1.X - Center.X) * (p1.X - Center.X)  +
                (p1.Z - Center.Z) * (p1.Z - Center.Z) - Radius * Radius;

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
                    case 2:  /* went through cylinder: must stop at nearest intersection */
                        //*which is nearest?*/
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

            /* roots imaginary -> no intersection */
            return false;
        }  
    }
}
