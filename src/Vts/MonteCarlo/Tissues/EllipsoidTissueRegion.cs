using System;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueRegion.  Defines ellipsoid given Center, and Axis
    /// radii along x,y,z axis.
    /// </summary>
    public class EllipsoidTissueRegion : ITissueRegion
    {
        private bool _onBoundary;
        /// <summary>
        /// class specifies ellipsoid tissue region (x-xc)^2/a^2 + (y-yc)^2/b^2 + (z-zc)^2/c^2 = 1
        /// where center is (xc,yc,zc) and semi-axis along x-, y-, z- axes are a, b, c, respectively.
        /// </summary>
        /// <param name="center">Position (x,y,z) of the center of the ellipsoid</param>
        /// <param name="radiusX">semi-axis along x-axis</param>
        /// <param name="radiusY">semi-axis along y-axis</param>
        /// <param name="radiusZ">semi-axis along z-axis</param>
        /// <param name="op">OpticalProperties of ellipsoid</param>
        public EllipsoidTissueRegion(Position center, double radiusX, double radiusY, double radiusZ,
            OpticalProperties op)
        {
            TissueRegionType = "Ellipsoid";
            RegionOP = op;
            Center = center;
            Dx = radiusX;
            Dy = radiusY;
            Dz = radiusZ;
        }
        /// <summary>
        /// default constructor defines sphere with radius 0.5mm and center (0,0,1)
        /// </summary>
        public EllipsoidTissueRegion() : this (new Position(0, 0, 1), 0.5, 0.5, 0.5,
            new OpticalProperties(0.05, 1.0, 0.8, 1.4)) {}

        /// <summary>
        /// tissue region identifier
        /// </summary>
        public string TissueRegionType { get; set; }
        /// <summary>
        /// optical properties of ellipsoid
        /// </summary>
        public OpticalProperties RegionOP { get; set; }
        /// <summary>
        /// center of ellipsoid
        /// </summary>
        public Position Center { get; set; }
        /// <summary>
        /// distance from center to x-axis radius
        /// </summary>
        public double Dx { get; set; }
        /// <summary>
        /// distance from center to y-axis radius
        /// </summary>
        public double Dy { get; set; }
        /// <summary>
        /// distance from center to z-axis radius
        /// </summary>
        public double Dz { get; set; }
        /// <summary>
        /// method to determine if given Position lies within ellipsoid allows for floating point imprecision
        /// </summary>
        /// <param name="position">Position</param>
        /// <returns>Boolean, true if within or on, false otherwise</returns>
        public bool ContainsPosition(Position position)
        {
            var inside = (position.X - Center.X) * (position.X - Center.X) /
                            (Dx * Dx) +
                            (position.Y - Center.Y) * (position.Y - Center.Y) /
                            (Dy * Dy) +
                            (position.Z - Center.Z) * (position.Z - Center.Z) /
                            (Dz * Dz);

            switch (inside)
            {
                case < 0.9999999999:
                    return true; // previous check  0.9999999
                case > 1.00000000001:
                    return false; // previous check 1.0000001
                default:
                    // on boundary means ellipsoid contains position
                    _onBoundary = true;
                    return true;  // ckh 2/28/19 this has to return true or unit tests fail => contains if on ellipsoid
            }
        }
        /// <summary>
        /// Method to determine if given Position lies on boundary of ellipsoid.
        /// Currently OnBoundary of an inclusion region isn't called by any code ckh 3/5/19.
        /// </summary>
        /// <param name="position">position to be checked</param>
        /// <returns>true if on boundary, false otherwise</returns>
        public bool OnBoundary(Position position)
        {
            return !ContainsPosition(position) && _onBoundary;
        }
        /// <summary>
        /// method to determine normal to surface at given position. Note this returns outward facing normal.
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>Direction normal to surface at position</returns>
        public Direction SurfaceNormal(Position position)
        {
            var newX = 2 * (position.X - Center.X) / (Dx * Dx);
            var newY = 2 * (position.Y - Center.Y) / (Dy * Dy);
            var newZ = 2 * (position.Z - Center.Z) / (Dz * Dz);
            var norm = Math.Sqrt(newX * newX + newY * newY + newZ * newZ);
            return new Direction(newX / norm, newY / norm, newZ / norm);
        }
        /// <summary>
        /// method to determine if photon track or ray intersects boundary of ellipsoid
        /// equations to determine intersection are derived by parameterizing ray from p1 to p2
        /// as p2=p1+[dx dy dz]t t in [0,1] where dx=p2.x-p1.x dy=p2.y-p1.y dz=p2.z-p2.z
        /// and substituting into ellipsoid equations and solving quadratic in t, i.e. t1, t2
        /// t1,t2 less than 0 or t1,t2 greater than 1 => no intersection
        /// 0 less than t1 less than 1 => one intersection
        /// 0 less than t2 less than 1 => one intersections, if above line true too => two intersections
        /// </summary>
        /// <param name="photon">Photon</param>
        /// <param name="distanceToBoundary">return: distance to boundary, infinity if no intersection</param>
        /// <returns>Boolean true if intersection, false otherwise</returns>
        public bool RayIntersectBoundary(Photon photon, out double distanceToBoundary)
        {
            distanceToBoundary = double.PositiveInfinity;
            _onBoundary = false; // reset _onBoundary
            var root = 0.0;
            var dp = photon.DP;
            var p1 = dp.Position;
            var d1 = dp.Direction;

            // determine location of end of ray
            var p2 = new Position(p1.X + d1.Ux*photon.S, p1.Y + d1.Uy*photon.S, p1.Z + d1.Uz*photon.S);

            var oneIn = this.ContainsPosition(p1);
            var twoIn = this.ContainsPosition(p2);

            // check if ray within ellipsoid 
            if ( (oneIn || _onBoundary) && twoIn ) return false;
            _onBoundary = false; // reset flag
            
            var areaX = Dx * Dx;
            var areaY = Dy * Dy;
            var areaZ = Dz * Dz;

            var dx = (p2.X - p1.X);
            var dy = (p2.Y - p1.Y);
            var dz = (p2.Z - p1.Z);

            var dxSquared = dx * dx;
            var dySquared = dy * dy;
            var dzSquared = dz * dz;

            var xOffset = p1.X - Center.X;
            var yOffset = p1.Y - Center.Y;
            var zOffset = p1.Z - Center.Z;

            var a =
                dxSquared / areaX +
                dySquared / areaY +
                dzSquared / areaZ;

            var b =
                2 * dx * xOffset / areaX +
                2 * dy * yOffset / areaY +
                2 * dz * zOffset / areaZ;

            var c =
                xOffset * xOffset / areaX +
                yOffset * yOffset / areaY +
                zOffset * zOffset / areaZ - 1.0;

            var rootTerm = b * b - 4 * a * c;

            if (rootTerm <= 0) return false; // roots are real 
            var rootTermSqrt = Math.Sqrt(rootTerm);
            var root1 = (-b - rootTermSqrt) / (2 * a);
            var root2 = (-b + rootTermSqrt) / (2 * a);

            var numberOfIntersections = 0; //number of intersections

            if (root1 is < 1 and > 0)
            {
                numberOfIntersections += 1;
                root = root1;
            }

            if (root2 is < 1 and > 0)
            {
                numberOfIntersections += 1;
                root = root2;
            }

            double xto;
            double yto;
            double zto;
            switch (numberOfIntersections)
            {
                case 0: /* roots real but no intersection */
                    return false;
                case 1:
                    if (!oneIn && Math.Abs(root) < 1e-7)  return false;

                    /*entering or exiting ellipsoid. It's the same*/
                    xto = p1.X + root * dx;
                    yto = p1.Y + root * dy;
                    zto = p1.Z + root * dz;

                    /*distance to the boundary*/
                    distanceToBoundary = Math.Sqrt((xto - p1.X) * (xto - p1.X) +
                                                   (yto - p1.Y) * (yto - p1.Y) +
                                                   (zto - p1.Z) * (zto - p1.Z));

                    // ckh fix 8/25/11: check if on boundary of ellipsoid
                    return distanceToBoundary >= 1e-11;

                case 2:  /* went through ellipsoid: must stop at nearest intersection */
                    /*which is nearest?*/
                    if (oneIn)
                    {
                        root = root1 > root2 ? root1 : root2; //CKH FIX 11/11
                    }
                    else
                    {
                        root = root1 < root2 ? root1 : root2; //CKH FIX 11/11
                    }
                    xto = p1.X + root * (p2.X - p1.X);
                    yto = p1.Y + root * (p2.Y - p1.Y);
                    zto = p1.Z + root * (p2.Z - p1.Z);

                    /*distance to the nearest boundary*/
                    distanceToBoundary = Math.Sqrt((xto - p1.X) * (xto - p1.X) +
                                                   (yto - p1.Y) * (yto - p1.Y) +
                                                   (zto - p1.Z) * (zto - p1.Z));

                    return true;

            } /* end switch */

            /* bb-4ac>0 */
            /* roots imaginary -> no intersection */
            return false;
        }

    }
}
