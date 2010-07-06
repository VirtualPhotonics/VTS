using Vts.Common;
using Vts.MonteCarlo.Factories;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueRegion.  Defines ellipsiod given Center, and Axis
    /// radii along x,y,z axis.
    /// </summary>
    public class EllipsoidRegion : ITissueRegion
    {
        private OpticalProperties _RegionOP;
        private double _ScatterLength;
        private static Position _Center;
        private static double _Dx;
        private static double _Dy;
        private static double _Dz;

        public EllipsoidRegion(Position center, double radiusX, double radiusY, double radiusZ,
            OpticalProperties op, AbsorptionWeightingType awt)
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
            _ScatterLength = op.GetScatterLength(awt);
        }
        public EllipsoidRegion() : this (new Position(0, 0, 1), 0.5, 0.5, 0.5,
            new OpticalProperties(0.05, 1.0, 0.8, 1.4), AbsorptionWeightingType.Discrete) {}

        #region Properties
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
        public double ScatterLength
        {
            get { return _ScatterLength; }
            set { _ScatterLength = value; }
        }
        #endregion

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
                    //return 1;
                    return true;
                }
                else if (inside > 1.0000001)
                {
                    //return 0;
                    return false;
                }
                else
                {//if (inside == 1.0)
                    //return 3;
                    return true;
                }
            
        }
        //void CrossEllip(Photon photptr)   //---new
        ///*based on CrossRegion, but without Fresnel (n doesn't change)=>no reflections at the boundary.*/
        //{
        //    int CurrentRegionIndex = photptr.CurrentRegionIndex;
        //    if (CurrentRegionIndex == 2)
        //        photptr.CurrentRegionIndex--;
        //    else
        //        photptr.CurrentRegionIndex++;
        //}

        # region ISource Members

        //public bool RayIntersectEllipse(Photon photptr, ref double distanceToBoundary)
        ///* returns	2 if hit the ellipsoid from outside, 
        //            3 if didn't hit nothing but ray is in the ellipsoid, 
        //            4 if hit ellip from inside, 
        //            0 else */
        //{
        //    double root1, root2, xto, yto, zto;
        //    double root = 0;
        //    bool one_in, two_in;
        //    //int numint;
        //    //short hit = 0;
        //    bool hit = false;
        //    var p1 = photptr.DP.Position;
        //    // comment for compile
        //    var p2 = new Position(); // work around for compile
        //    //Position p2 = photptr.GetNewPosition(photptr.DP.PAndD, photptr.S);
        //    double dbound;  /* distance to boundary */
        //    double s = photptr.S;
        //    int CurrentRegionIndexIndex = photptr.CurrentRegionIndex;
            //double mua = photptr.Tissptr.Regions[CurrentRegionIndexIndex].RegionOP.Mua;
            //double mus = photptr.Tissptr.Regions[CurrentRegionIndexIndex].RegionOP.Mus; 
            // check of slab edges should be in VoxelRegion
            //if (p1.Z < 0 || p2.Z > photptr.Tissptr.Regions[1].zEnd)
            //{  // if hits upper or lower boundary (with air)
            //    hit = HitLayer(tissptr, photptr);
            //}
            //else
        //    {

        //        /* determine intersection with ellipsoid*/
        //        one_in = InRegion(p1);//is (x1,y1,z1)in the ellip? 0=no, 1=yes, 3=on the boundary, 2=ellip doesn't exist
        //        two_in = InRegion(p2);

        //        //if ((one_in == 1 || one_in == 2 || one_in == 3) && (two_in == 1 || two_in == 2))  /* ray within ellipsoid */
        //        if ((one_in && two_in))
        //        {
        //            //hit = 3;
        //            hit = false;
        //        }

        //        else   //ray may cross the ellipsoid once, twice or never. 
        //        //ph MUST stop at first intersection. finds number of intersections numint
        //        {
        //            // sanity check dimensions of ellipsoid 
        //            //if ((Dx == 0.0) ||
        //            //    (tissptr.ellip_rad_y == 0.0) ||
        //            //    (tissptr.ellip_rad_z == 0.0))
        //            //{
        //            //    Console.WriteLine("PERT ERROR: one ellipsoid radial dimension = 0.0\n");
        //            //}
        //            //else
        //            {// finds intersections
        //                double area_x = _Dx * _Dx;
        //                double area_y = _Dy * _Dy;
        //                double area_z = _Dz * _Dz;

        //                double dx = (p2.X - p1.X);
        //                double dy = (p2.Y - p1.Y);
        //                double dz = (p2.Z - p1.Z);

        //                double dxSquared = dx * dx;
        //                double dySquared = dy * dy;
        //                double dzSquared = dz * dz;

        //                double xOffset = p1.X - _Dx;
        //                double yOffset = p1.Y - _Dy;
        //                double zOffset = p1.Z - _Dz;

        //                double A =
        //                    dxSquared / area_x +
        //                    dySquared / area_y +
        //                    dzSquared / area_z;

        //                double B =
        //                    2 * dx * xOffset / area_x +
        //                    2 * dy * yOffset / area_y +
        //                    2 * dz * zOffset / area_z;

        //                double C =
        //                    xOffset * xOffset / area_x +
        //                    yOffset * yOffset / area_y +
        //                    zOffset * zOffset / area_z - 1.0;

        //                double rootTerm = B * B - 4 * A * C;
        //                if (rootTerm > 0)  // roots are real 
        //                {
        //                    double rootTermSqrt = Math.Sqrt(rootTerm);
        //                    root1 = (-B - rootTermSqrt) / (2 * A);
        //                    root2 = (-B + rootTermSqrt) / (2 * A);

        //                    int numint = 0;             //number of intersections
        //                    if ((root1 < 1) && (root1 > 0))
        //                    {
        //                        numint += 1;
        //                        root = root1;
        //                    }
        //                    if ((root2 < 1) && (root2 > 0))
        //                    {
        //                        numint += 1;
        //                        root = root2;
        //                    }
        //                    switch (numint)
        //                    {
        //                        case 0: /* roots real but no intersection */
        //                            //hit = 0;
        //                            hit = false;
        //                            break;
        //                        case 1:
        //                            //if ((one_in == 3) && (Math.Abs(root) < 1e-7)) //CKH FIX 11/11
        //                            if ( (!one_in) && (Math.Abs(root) < 1e-7))
        //                                //hit = 0;
        //                                hit = false;
        //                            else
        //                            {
        //                                //if (one_in == 1)
        //                                if (one_in)
        //                                {
        //                                    //hit = 4;       //exiting the ellipse 
        //                                    hit = true;
        //                                }
        //                                else
        //                                {
        //                                    //hit = 2;       //entering it 
        //                                    hit = true;
        //                                }

        //                                /*entering or exiting ellipsoid. It's the same*/
        //                                xto = p1.X + root * dx;
        //                                yto = p1.Y + root * dy;
        //                                zto = p1.Z + root * dz;

        //                                /*distance to the boundary*/
        //                                dbound = Math.Sqrt((xto - p1.X) * (xto - p1.X) +
        //                                      (yto - p1.Y) * (yto - p1.Y) +
        //                                      (zto - p1.Z) * (zto - p1.Z));

        //                                photptr.HitBoundary = true;
        //                                //photptr.SLeft = (photptr.S - dbound) * (mua + mus);  // CHECK FOR CAW
        //                                photptr.SLeft = (photptr.S - dbound) * photptr.Tissptr.Regions[photptr.CurrentRegionIndex].ScatterLength;
        //                                photptr.S = dbound;

        //                            }
        //                            break;

        //                        case 2:  /* went through ellipsoid: must stop at nearest intersection */
        //                            /*which is nearest?*/
        //                            //if (one_in == 3)
        //                            if (one_in)
        //                            {
        //                                if (root1 > root2) //CKH FIX 11/11
        //                                    root = root1;
        //                                else root = root2;
        //                            }
        //                            else
        //                            {
        //                                if (root1 < root2) //CKH FIX 11/11
        //                                    root = root1;
        //                                else root = root2;
        //                            }
        //                            xto = p1.X + root * (p2.X - p1.X);
        //                            yto = p1.Y + root * (p2.Y - p1.Y);
        //                            zto = p1.Z + root * (p2.Z - p1.Z);
        //                            /*distance to the nearest boundary*/
        //                            dbound = Math.Sqrt((xto - p1.X) * (xto - p1.X) +
        //                                    (yto - p1.Y) * (yto - p1.Y) +
        //                                    (zto - p1.Y) * (zto - p1.Y));
        //                            photptr.HitBoundary = true;
        //                            photptr.SLeft = (photptr.S - dbound) * (mua + mus);  // Check for CAW
        //                            photptr.S = dbound;

        //                            //hit = 2;
        //                            hit = true;
        //                            break;

        //                    } /* end switch */

        //                } /* BB-4AC>0 */
        //                else /* roots imaginary -> no intersection */
        //                {
        //                    //hit = 0;
        //                    hit = false;
        //                }
        //            } /* check on radial axis of ellipsoid */
        //        } /* intersection with ellipsoid */
        //    }
        //    return hit;
        //}
        #endregion
    }

}
