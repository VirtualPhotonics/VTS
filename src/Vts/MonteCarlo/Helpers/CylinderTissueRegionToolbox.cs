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
            double root1, root2, xto, yto, zto;
            double root = 0;
            double A, B, C;
            double dx = (p2.X - p1.X);
            double dy = (p2.Y - p1.Y);
            double dz = (p2.Z - p1.Z);
            switch (axis)
            {
                case CylinderTissueRegionAxisType.X:
                    A = dy * dy + dz * dz;
                    B = 2 * (p1.Y - center.Y) * dy +
                        2 * (p1.Z - center.Z) * dz;
                    C = (p1.Y - center.Y) * (p1.Y - center.Y) +
                        (p1.Z - center.Z) * (p1.Z - center.Z) - radius * radius;
                    break;
                default:
                case CylinderTissueRegionAxisType.Y:
                    A = dx * dx + dz * dz;
                    B = 2 * (p1.X - center.X) * dx +
                        2 * (p1.Z - center.Z) * dz;
                    C = (p1.X - center.X) * (p1.X - center.X) +
                        (p1.Z - center.Z) * (p1.Z - center.Z) - radius * radius;
                    break;
                case CylinderTissueRegionAxisType.Z:
                    A = dx * dx + dy * dy;
                    B = 2 * (p1.X - center.X) * dx +
                        2 * (p1.Y - center.Y) * dy;
                    C = (p1.X - center.X) * (p1.X - center.X) +
                        (p1.Y - center.Y) * (p1.Y - center.Y) - radius * radius;
                    break;
            }

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
                        if ((!oneIn) && (Math.Abs(root) < 1e-7))
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
                        if (oneIn)
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
