using System;
using System.Runtime.Serialization;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueRegion.  Defines Cartesian coordinate voxel with
    /// x,y,z ranges.
    /// </summary>
    public class VoxelRegion : ITissueRegion
    {
        /// <summary>
        /// constructor for voxel region
        /// </summary>
        /// <param name="x">x range of voxel</param>
        /// <param name="y">y range of voxel</param>
        /// <param name="z">z range of voxel</param>
        /// <param name="op">optical properties of voxel</param>
        /// <param name="awt">absorption weighting type of voxel</param>
        public VoxelRegion(DoubleRange x, DoubleRange y, DoubleRange z, OpticalProperties op,
                           AbsorptionWeightingType awt)
        {
            TissueRegionType = TissueRegionType.Voxel;
            X = x;
            Y = y;
            Z = z;
            RegionOP = op;
        }

        /// <summary>
        /// default constructor
        /// </summary>
        public VoxelRegion() : this(
            new DoubleRange(-10.0, 10),
            new DoubleRange(-10.0, 10),
            new DoubleRange(0.0, 10),
            new OpticalProperties(0.01, 1.0, 0.8, 1.4), AbsorptionWeightingType.Discrete)
        {
        }

        /// <summary>
        /// tissue region identifier
        /// </summary>
        public TissueRegionType TissueRegionType { get; set; }

        /// <summary>
        /// x range of voxel
        /// </summary>
        public DoubleRange X { get; set; }

        /// <summary>
        /// y range of voxel
        /// </summary>
        public DoubleRange Y { get; set; }

        /// <summary>
        /// z range of voxel
        /// </summary>
        public DoubleRange Z { get; set; }

        /// <summary>
        /// optical properties of voxel
        /// </summary>
        public OpticalProperties RegionOP { get; set; }

        /// <summary>
        /// center of voxel
        /// </summary>
        [IgnoreDataMember]
        public Position Center
        {
            get
            {
                return new Position(
                    (X.Start + X.Stop)/2,
                    (Y.Start + Y.Stop)/2,
                    (Z.Start + Z.Stop)/2);
            }
            set
            {
                var oldCenter = Center;
                var newCenter = value;

                var dx = newCenter.X - oldCenter.X;
                var dy = newCenter.Y - oldCenter.Y;
                var dz = newCenter.Z - oldCenter.Z;

                X.Start += dx;
                X.Stop += dx;

                Y.Start += dy;
                Y.Stop += dy;

                Z.Start += dz;
                Z.Stop += dz;
            }
        }

        /// <summary>
        /// Checks if the specified photon will intersect the voxel boundary
        /// </summary>
        /// <param name="photon">Photon to check for intersection (including Position, Direction, and S)</param>
        /// <param name="distanceToBoundary">The distance to the next boundary</param>
        /// <returns>True if photon will intersect the voxel boundary, false otherwise</returns>
        public bool RayIntersectBoundary(Photon photon, out double distanceToBoundary)
        {
            throw new NotImplementedException("This code is under construction");

            // the added code below is still under construction
            //distanceToBoundary = double.PositiveInfinity;
            //double root1, root2, xto, yto, zto;
            //double root = 0;
            //var dp = photon.DP;
            //var p1 = dp.Position;
            //var d1 = dp.Direction;

            //// determine location of end of ray
            //var p2 = new Position(p1.X + d1.Ux*photon.S, p1.Y + d1.Uy*photon.S, p1.Z + d1.Uz*photon.S);

            //bool one_in = this.ContainsPosition(p1);
            //bool two_in = this.ContainsPosition(p2);

            //// check if ray within voxel 
            //if (one_in && two_in)
            //{
            //    return false;
            //}

            //double rX, rY, rZ, xint, yint, zint;

            //double trackLength = Math.Sqrt((p2.X - p1.X)*(p2.X - p1.X) +
            //                               (p2.Y - p1.Y)*(p2.Y - p1.Y) +
            //                               (p2.Z - p1.Z)*(p2.Z - p1.Z));

            //if ((!one_in && two_in) || (one_in && !two_in)) // one in and one out so entering or exiting
            //{
            //    /* NOTE code not optimal yet */
            //    if (p1.Z < Z.Start) // from above
            //    {
            //        rZ = (Z.Start - p1.Z)/(p2.Z - p1.Z); /* determine intersect with plane */
            //        xint = p1.X + (p2.X - p1.X)*rZ;
            //        yint = p1.X + (p2.X - p1.X)*rZ;
            //    }
            //    else if (p1.Z > Z.Stop) // from below
            //    {
            //        rZ = (Z.Stop - p1.Z)/(p2.Z - p1.Z);
            //        xint = p1.X + (p2.X - p1.X)*rZ;
            //        yint = p1.X + (p2.X - p1.X)*rZ;
            //    }
            //    else if (p1.X < X.Start) // from left
            //    {
            //        rX = (X.Start - p1.X)/(p2.X - p1.X);
            //        yint = p1.X + (p2.X - p1.X)*rX;
            //        zint = p1.Z + (p2.Z - p1.Z)*rX;
            //    }
            //    else if (p1.X > X.Stop) // from right
            //    {
            //        rX = (X.Stop - p1.X)/(p2.X - p1.X);
            //        yint = p1.X + (p2.X - p1.X)*rX;
            //        zint = p1.Z + (p2.Z - p1.Z)*rX;
            //    }
            //    else if (p1.X < Y.Start) // from back
            //    {
            //        rY = (Y.Start - p1.X)/(p2.X - p1.X);
            //        xint = p1.X + (p2.X - p1.X)*rY;
            //        zint = p1.Z + (p2.Z - p1.Z)*rY;
            //    }
            //    else if (p1.X > Y.Stop) // from front
            //    {
            //        rY = (Y.Stop - p1.X)/(p2.X - p1.X); /* from front */
            //        xint = p1.X + (p2.X - p1.X)*rY;
            //        zint = p1.Z + (p2.Z - p1.Z)*rY;
            //    }
            //}
            
            //if (!one_in && !two_in)/* both out: check if thru cube */
            //{
            //    if ((p1.Z < Z.Start) && (p2.Z > Z.Stop)) // from above
            //    {
            //        rZ = (Z.Start - p1.Z)/(p2.Z - p1.Z);
            //        xint = p1.X + (p2.X - p1.X)*rZ;
            //        yint = p1.X + (p2.X - p1.X)*rZ;
            //    }
            //    else if ((p1.Z > Z.Stop) && (p2.Z < Z.Start))  // from below
            //    {
            //        rZ = (Z.Stop - p1.Z)/(p2.Z - p1.Z);
            //        xint = p1.X + (p2.X - p1.X)*rZ;
            //        yint = p1.X + (p2.X - p1.X)*rZ;
            //    }
            //    else if ((p1.X < X.Start) && (p2.X > X.Start)) // from left
            //    {
            //        rX = (X.Start - p1.X)/(p2.X - p1.X);
            //        yint = p1.X + (p2.X - p1.X)*rX;
            //        zint = p1.Z + (p2.Z - p1.Z)*rX;
            //    }
            //    else if ((p1.X > X.Stop) && (p2.X < X.Stop)) // from right
            //    {
            //        rX = (X.Stop - p1.X)/(p2.X - p1.X);
            //        yint = p1.X + (p2.X - p1.X)*rX;
            //        zint = p1.Z + (p2.Z - p1.Z)*rX;
            //    }                   
            //    else if ((p1.X < Y.Start) && (p2.X > Y.Start)) // from back
            //    {
            //        rY = (Y.Start - p1.X)/(p2.X - p1.X);
            //        xint = p1.X + (p2.X - p1.X)*rY;
            //        zint = p1.Z + (p2.Z - p1.Z)*rY;
            //    }
            //    else if ((p1.X > Y.Stop) && (p2.X < Y.Stop))  // from front
            //    {
            //        rY = (Y.Stop - p1.X)/(p2.X - p1.X);
            //        xint = p1.X + (p2.X - p1.X)*rY;
            //        zint = p1.Z + (p2.Z - p1.Z)*rY;
            //    }
            //    else if ((p1.Z > Z.Start) && (p2.Z < Z.Start)) // from above
            //    {
            //        rZ = (Z.Start - p1.Z)/(p2.Z - p1.Z);
            //        xint = p1.X + (p2.X - p1.X)*rZ;
            //        yint = p1.X + (p2.X - p1.X)*rZ;
            //    }
            //    else if ((p1.Z < Z.Stop) && (p2.Z > Z.Stop)) // from below
            //    {
            //        rZ = (Z.Stop - p1.Z)/(p2.Z - p1.Z);
            //        xint = p1.X + (p2.X - p1.X)*rZ;
            //        yint = p1.X + (p2.X - p1.X)*rZ;
            //    }
            //    else if ((p1.X > X.Start) && (p2.X < X.Start)) // from left
            //    {
            //        rX = (X.Start - p1.X)/(p2.X - p1.X);
            //        yint = p1.X + (p2.X - p1.X)*rX;
            //        zint = p1.Z + (p2.Z - p1.Z)*rX;
            //    }
            //    else if ((p1.X < X.Stop) && (p2.X > X.Stop)) // from right
            //    {
            //        rX = (X.Stop - p1.X)/(p2.X - p1.X);
            //        yint = p1.X + (p2.X - p1.X)*rX;
            //        zint = p1.Z + (p2.Z - p1.Z)*rX;\
            //    }
            //    else if ((p1.X > Y.Start) && (p2.X < Y.Start)) // from back
            //    {
            //        rY = (Y.Start - p1.X)/(p2.X - p1.X);
            //        xint = p1.X + (p2.X - p1.X)*rY;
            //        zint = p1.Z + (p2.Z - p1.Z)*rY;
            //    }
            //    else if ((p1.X < Y.Stop) && (p2.X > Y.Stop)) // from front
            //    {
            //        rY = (Y.Stop - p1.X)/(p2.X - p1.X);
            //        xint = p1.X + (p2.X - p1.X)*rY;
            //        zint = p1.Z + (p2.Z - p1.Z)*rY;
            //    }
            //}
        }

        /// <summary>
        /// method to determine if photon at designated position resides within voxel
        /// </summary>
        /// <param name="position">position of photon</param>
        /// <returns>boolean</returns>
        public bool ContainsPosition(Position position)
        {
            // inclusion defined in half-open interval [start,stop) so that continuum of voxels do not overlap
            return position.X >= X.Start && position.X < X.Stop &&
                   position.Y >= Y.Start && position.Y < Y.Stop &&
                   position.Z >= Z.Start && position.Z < Z.Stop;
        }

        /// <summary>
        /// method to determine if photon on boundary of voxel
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>boolean</returns>
        public bool OnBoundary(Position position)
        {
            return ((position.X == X.Start) || (position.X == X.Stop) && 
                        position.Y >= Y.Start && position.Y <= Y.Stop && position.Z >= Z.Start && position.Z <= Z.Stop) ||
                   ((position.Y == Y.Start) || (position.Y == Y.Stop) &&
                        position.X >= X.Start && position.X <= X.Stop && position.Z >= Z.Start && position.Z <= Z.Stop) ||
                   ((position.Z == Z.Start) || (position.Z == Z.Stop) &&
                        position.X >= X.Start && position.X <= X.Stop && position.Y >= Y.Start && position.Y <= Y.Stop); 
        }
    }
}
