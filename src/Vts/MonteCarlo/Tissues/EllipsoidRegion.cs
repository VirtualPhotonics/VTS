using System;
using Vts.Common;
using Vts.Extensions;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueRegion.  Defines ellipsiod given Center, and Axis
    /// radii along x,y,z axis.
    /// </summary>
    public class EllipsoidRegion : ITissueRegion
    {
        private bool _onBoundary = false;

        public EllipsoidRegion(Position center, double radiusX, double radiusY, double radiusZ,
            OpticalProperties op)
        {
            RegionOP = op;
            Center = center;
            Dx = radiusX;
            Dy = radiusY;
            Dz = radiusZ;
        }

        public EllipsoidRegion() : this (new Position(0, 0, 1), 0.5, 0.5, 0.5,
            new OpticalProperties(0.05, 1.0, 0.8, 1.4)) {}

        public OpticalProperties RegionOP { get; set; }
        public Position Center { get; set; }
        public double Dx { get; set; }
        public double Dy { get; set; }
        public double Dz { get; set; }

        public bool ContainsPosition(Position position)
        {
                double inside = (position.X - Center.X) * (position.X - Center.X) /
                          (Dx * Dx) +
                    (position.Y - Center.Y) * (position.Y - Center.Y) /
                          (Dy * Dy) +
                    (position.Z - Center.Z) * (position.Z - Center.Z) /
                          (Dz * Dz);

                //if (inside < 0.9999999)
                if (inside < 0.9999999999)
                {
                    return true;
                }
                //else if (inside > 1.0000001)
                else if (inside > 1.00000000001)
                {
                    return false;
                }
                else  // on boundary
                {
                    _onBoundary = true;
                    //return false; // ckh try 8/21/11
                    return true;
                }
        }

        public bool OnBoundary(Position position)
        {
            return !ContainsPosition(position) && _onBoundary;
        }
        
        public bool RayIntersectBoundary(Photon photon, out double distanceToBoundary)
        {
            distanceToBoundary = double.PositiveInfinity;
            double root1, root2, xto, yto, zto;
            double root = 0;
            var dp = photon.DP;
            var p1 = dp.Position;
            var d1 = dp.Direction;

            // determine location of end of ray
            var p2 = new Position(p1.X + d1.Ux*photon.S, p1.Y + d1.Uy*photon.S, p1.Z + d1.Uz*photon.S);

            bool one_in = this.ContainsPosition(p1);
            bool two_in = this.ContainsPosition(p2);

            // check if ray within ellipsoid 
            if ( (one_in || _onBoundary) && two_in )
            {
                return false;
            }
            _onBoundary = false; // reset flag
            
            double area_x = Dx * Dx;
            double area_y = Dy * Dy;
            double area_z = Dz * Dz;

            double dx = (p2.X - p1.X);
            double dy = (p2.Y - p1.Y);
            double dz = (p2.Z - p1.Z);

            double dxSquared = dx * dx;
            double dySquared = dy * dy;
            double dzSquared = dz * dz;

            double xOffset = p1.X - Center.X;
            double yOffset = p1.Y - Center.Y;
            double zOffset = p1.Z - Center.Z;

            double A =
                dxSquared / area_x +
                dySquared / area_y +
                dzSquared / area_z;

            double B =
                2 * dx * xOffset / area_x +
                2 * dy * yOffset / area_y +
                2 * dz * zOffset / area_z;

            double C =
                xOffset * xOffset / area_x +
                yOffset * yOffset / area_y +
                zOffset * zOffset / area_z - 1.0;

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
                        //if ((one_in == 3) && (Math.Abs(root) < 1e-7)) //CKH FIX 11/11
                        if ((!one_in) && (Math.Abs(root) < 1e-7))
                        {
                            return false;
                        }

                        /*entering or exiting ellipsoid. It's the same*/
                        xto = p1.X + root * dx;
                        yto = p1.Y + root * dy;
                        zto = p1.Z + root * dz;

                        /*distance to the boundary*/
                        distanceToBoundary = Math.Sqrt((xto - p1.X) * (xto - p1.X) +
                                (yto - p1.Y) * (yto - p1.Y) +
                                (zto - p1.Z) * (zto - p1.Z));

                        // ckh fix 8/25/11: check if on boundary of ellipsoid
                        if (distanceToBoundary < 1e-11)
                        {
                            return false;
                        }

                        return true;
                    case 2:  /* went through ellipsoid: must stop at nearest intersection */
                        /*which is nearest?*/
                        //if (one_in == 3)
                        if (one_in)
                        {
                            if (root1 > root2) //CKH FIX 11/11
                                root = root1;
                            else root = root2;
                        }
                        else
                        {
                            if (root1 < root2) //CKH FIX 11/11
                                root = root1;
                            else root = root2;
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

            } /* BB-4AC>0 */
            
            /* roots imaginary -> no intersection */
            return false;
        }

        //public bool RayIntersectBoundary(Photon photon)
        //{
        //    double root1, root2;
        //    double root = 0;
        //    var dp = photon.DP;
        //    var p1 = dp.Position;
        //    var d1 = dp.Direction;

        //    // DC - CKH: correct??
        //    var p2 = new Position(p1.X + d1.Ux * photon.S, p1.Y + d1.Uy * photon.S, p1.Z + d1.Uz * photon.S);

        //    //bool one_in = this.ContainsPosition(p1);

        //    double area_x = Dx * Dx;
        //    double area_y = Dy * Dy;
        //    double area_z = Dz * Dz;

        //    double dx = (p2.X - p1.X);
        //    double dy = (p2.Y - p1.Y);
        //    double dz = (p2.Z - p1.Z);

        //    double dxSquared = dx * dx;
        //    double dySquared = dy * dy;
        //    double dzSquared = dz * dz;

        //    double xOffset = p1.X - Center.X;
        //    double yOffset = p1.Y - Center.Y;
        //    double zOffset = p1.Z - Center.Z;

        //    double A =
        //        dxSquared / area_x +
        //        dySquared / area_y +
        //        dzSquared / area_z;

        //    double B =
        //        2 * dx * xOffset / area_x +
        //        2 * dy * yOffset / area_y +
        //        2 * dz * zOffset / area_z;

        //    double C =
        //        xOffset * xOffset / area_x +
        //        yOffset * yOffset / area_y +
        //        zOffset * zOffset / area_z - 1.0;

        //    double rootTerm = B * B - 4 * A * C;

        //    if (rootTerm > 0)  // roots are real 
        //    {
        //        double rootTermSqrt = Math.Sqrt(rootTerm);
        //        root1 = (-B - rootTermSqrt) / (2 * A);
        //        root2 = (-B + rootTermSqrt) / (2 * A);

        //        int numint = 0; //number of intersections

        //        if ((root1 < 1) && (root1 > 0))
        //        {
        //            numint += 1;
        //            root = root1;
        //        }

        //        if ((root2 < 1) && (root2 > 0))
        //        {
        //            numint += 1;
        //            root = root2;
        //        }

        //        switch (numint)
        //        {
        //            case 0: /* roots real but no intersection */
        //                return false;
        //            case 2:  /* went through ellipsoid: must stop at nearest intersection */
        //                return true;
        //            case 1:
        //                //if ((one_in == 3) && (Math.Abs(root) < 1e-7)) //CKH FIX 11/11
        //                if ((!this.ContainsPosition(p1)) && (Math.Abs(root) < 1e-7))
        //                {
        //                    return false;
        //                }
        //                return true;
        //        } /* end switch */
        //    } /* BB-4AC>0 */

        //    /* roots imaginary -> no intersection */
        //    return false;
        //}

        //void CrossEllip(Photon photptr)   //---new
        //*based on CrossRegion, but without Fresnel (n doesn't change)=>no reflections at the boundary.*/
        //{
        //    int CurrentRegionIndex = photptr.CurrentRegionIndex;
        //    if (CurrentRegionIndex == 2)
        //        photptr.CurrentRegionIndex--;
        //    else
        //        photptr.CurrentRegionIndex++;
        //}

    }

}
