using System;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Helper methods for  cylindrical tissue region methods.  For example, ray intersect infinite cylinder of given axis alignment
    /// and ray intersect finite cylinder.  Currently this method only makes sense with our current tissue classes, if axis alignment
    /// is along x- or y- axes because if along z-axis will intersect tissue surface and bottom.
    /// </summary>
    public abstract class CylinderTissueRegionToolbox 
    {
        /// <summary>
        /// method to determine if photon ray (or track) will intersect boundary of cylinder
        /// equations to determine intersection are derived by parameterizing ray from p1 to p2
        /// as p2=p1+[dx dy dz]t t in [0,1] where dx=p2.x-p1.x dy=p2.y-p1.y dz=p2.z-p2.z
        /// and substituting into cylinder equations and solving quadratic in t, i.e. t1, t2
        /// t1,t2 less than 0 or t1,t2 greater than 1 => no intersection
        /// 0 less than t1 less than 1 => one intersection
        /// 0 less than t2 less than 1 => one intersections, if above line true too => two intersections
        /// </summary>
        /// <param name="p1">ray starting position</param>
        /// <param name="p2">ray ending position</param>
        /// <param name="oneIn">Boolean indicating whether p1 is inside infinite cylinder</param>
        /// <param name="axis">axis of cylinder: CylinderTissueRegionAxisType.X, Y or Z</param>
        /// <param name="center">position of center of cylinder</param>
        /// <param name="radius">radius of cylinder</param>
        /// <param name="distanceToBoundary">return: distance to boundary, infinity if no intersection</param>
        /// <returns>Boolean: true if intersection, false if on boundary of cylinder</returns>
        public static bool RayIntersectInfiniteCylinder(Position p1, Position p2, bool oneIn, CylinderTissueRegionAxisType axis, Position center, double radius, 
            out double distanceToBoundary)
        {
            distanceToBoundary = double.PositiveInfinity;
            double root = 0;
            double a, b, c;
            var dx = (p2.X - p1.X);
            var dy = (p2.Y - p1.Y);
            var dz = (p2.Z - p1.Z);
            switch (axis)
            {
                case CylinderTissueRegionAxisType.X:
                    a = dy * dy + dz * dz;
                    b = 2 * (p1.Y - center.Y) * dy +
                        2 * (p1.Z - center.Z) * dz;
                    c = (p1.Y - center.Y) * (p1.Y - center.Y) +
                        (p1.Z - center.Z) * (p1.Z - center.Z) - radius * radius;
                    break;
                case CylinderTissueRegionAxisType.Z:
                    a = dx * dx + dy * dy;
                    b = 2 * (p1.X - center.X) * dx +
                        2 * (p1.Y - center.Y) * dy;
                    c = (p1.X - center.X) * (p1.X - center.X) +
                        (p1.Y - center.Y) * (p1.Y - center.Y) - radius * radius;
                    break;
                case CylinderTissueRegionAxisType.Y:
                default:
                    a = dx * dx + dz * dz;
                    b = 2 * (p1.X - center.X) * dx +
                        2 * (p1.Z - center.Z) * dz;
                    c = (p1.X - center.X) * (p1.X - center.X) +
                        (p1.Z - center.Z) * (p1.Z - center.Z) - radius * radius;
                    break;
            }

            var rootTerm = b * b - 4 * a * c;

            if (rootTerm > 0)  // roots are real 
            {
                var rootTermSqrt = Math.Sqrt(rootTerm);
                var root1 = (-b - rootTermSqrt) / (2 * a);
                var root2 = (-b + rootTermSqrt) / (2 * a);

                var numberOfIntersections = 0; //number of intersections

                if (root1 < 1.0 && root1 > 0.0)
                {
                    numberOfIntersections += 1;
                    root = root1;
                }

                if (root2 < 1.0 && root2 > 0.0)
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
                        if (!oneIn && Math.Abs(root) < 1e-7)
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
                        return distanceToBoundary >= 1e-11;

                    case 2:  /* went through cylinder: must stop at nearest intersection */
                        //*which is nearest?*/
                        if (oneIn)
                        {
                            root = root1 > root2 ? root1 : root2;
                        }
                        else
                        {
                            root = root1 < root2 ? root1 : root2;
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

            } /* bb-4ac>0 */

            /* roots imaginary -> no intersection */
            return false;
        }  
    }
}
