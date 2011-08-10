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
        private OpticalProperties _RegionOP;
        private static Position _Center;
        private static double _Dx;
        private static double _Dy;
        private static double _Dz;

        public EllipsoidRegion(Position center, double radiusX, double radiusY, double radiusZ,
            OpticalProperties op)
        {
            if ((radiusX == 0.0) || (radiusY == 0.0) || (radiusZ == 0.0))
            {
                throw new System.ArgumentException("radiusX, Y or Z cannot be 0", "radiusX");
            }
            _RegionOP = new OpticalProperties(op.Mua, op.Musp, op.G, op.N);
            _Center = center;
            _Dx = radiusX;
            _Dy = radiusY;
            _Dz = radiusZ;
        }

        public EllipsoidRegion() : this (new Position(0, 0, 1), 0.5, 0.5, 0.5,
            new OpticalProperties(0.05, 1.0, 0.8, 1.4)) {}

        public OpticalProperties RegionOP
        {
            get { return _RegionOP; }
        }
        public Position Center
        {
            get { return _Center; }
            set { _Center = value; }
        }
        public double Dx
        {
            get { return _Dx; }
            set { _Dx = value; }
        }
        public double Dy
        {
            get { return _Dy; }
            set { _Dy = value; }
        }
        public double Dz
        {
            get { return _Dz; }
            set { _Dz = value; }
        }

        public bool ContainsPosition(Position position)
        {
                double inside = (position.X - _Center.X) * (position.X - _Center.X) /
                          (_Dx * _Dx) +
                    (position.Y - _Center.Y) * (position.Y - _Center.Y) /
                          (_Dy * _Dy) +
                    (position.Z - _Center.Z) * (position.Z - _Center.Z) /
                          (_Dz * _Dz);

                if (inside < 0.9999999)
                {
                    return true;
                }
                else if (inside > 1.0000001)
                {
                    return false;
                }
                else
                {
                    return true;
                }
        }
        
        public bool RayIntersectEllipsoid(Photon photon, out double distanceToBoundary)
        {
            distanceToBoundary = double.PositiveInfinity;
            double root1, root2, xto, yto, zto;
            double root = 0;
            var dp = photon.DP;
            var p1 = dp.Position;
            var d1 = dp.Direction;

            // DC - CKH: correct??
            var p2 = new Position(p1.X + d1.Ux*photon.S, p1.Y + d1.Uy*photon.S, p1.Z + d1.Uz*photon.S);

            bool one_in = this.ContainsPosition(p1);
            
            double area_x = _Dx * _Dx;
            double area_y = _Dy * _Dy;
            double area_z = _Dz * _Dz;

            double dx = (p2.X - p1.X);
            double dy = (p2.Y - p1.Y);
            double dz = (p2.Z - p1.Z);

            double dxSquared = dx * dx;
            double dySquared = dy * dy;
            double dzSquared = dz * dz;

            double xOffset = p1.X - _Dx;
            double yOffset = p1.Y - _Dy;
            double zOffset = p1.Z - _Dz;

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
                                (zto - p1.Y) * (zto - p1.Y));

                        return true;

                } /* end switch */

            } /* BB-4AC>0 */
            
            /* roots imaginary -> no intersection */
            return false;
        }
        public bool RayIntersectEllipsoid(Photon photon)
        {
            double root1, root2;
            double root = 0;
            var dp = photon.DP;
            var p1 = dp.Position;
            var d1 = dp.Direction;

            // DC - CKH: correct??
            var p2 = new Position(p1.X + d1.Ux * photon.S, p1.Y + d1.Uy * photon.S, p1.Z + d1.Uz * photon.S);

            //bool one_in = this.ContainsPosition(p1);

            double area_x = _Dx * _Dx;
            double area_y = _Dy * _Dy;
            double area_z = _Dz * _Dz;

            double dx = (p2.X - p1.X);
            double dy = (p2.Y - p1.Y);
            double dz = (p2.Z - p1.Z);

            double dxSquared = dx * dx;
            double dySquared = dy * dy;
            double dzSquared = dz * dz;

            double xOffset = p1.X - _Dx;
            double yOffset = p1.Y - _Dy;
            double zOffset = p1.Z - _Dz;

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
                    case 2:  /* went through ellipsoid: must stop at nearest intersection */
                        return true;
                    case 1:
                        //if ((one_in == 3) && (Math.Abs(root) < 1e-7)) //CKH FIX 11/11
                        if ((!this.ContainsPosition(p1)) && (Math.Abs(root) < 1e-7))
                        {
                            return false;
                        }
                        return true;
                } /* end switch */
            } /* BB-4AC>0 */

            /* roots imaginary -> no intersection */
            return false;
        }

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
