using System;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.Extensions;

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
        public VoxelRegion(DoubleRange x, DoubleRange y, DoubleRange z, OpticalProperties op, AbsorptionWeightingType awt) 
        {
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
            new OpticalProperties(0.01, 1.0, 0.8, 1.4), AbsorptionWeightingType.Discrete) {}  

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
                    (X.Start + X.Stop) / 2,
                    (Y.Start + Y.Stop) / 2,
                    (Z.Start + Z.Stop) / 2);
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
        /// <param name="p">Photon to check for intersection (including Position, Direction, and S)</param>
        /// <param name="distanceToBoundary">The distance to the next boundary</param>
        /// <returns>True if photon will intersect the voxel boundary, false otherwise</returns>
        public bool RayIntersectBoundary(Photon p, out double distanceToBoundary)
        {
            throw new NotImplementedException("This code is copied from layers and doesn't account for X or Y directions yet...");

            //distanceToBoundary = double.PositiveInfinity;  /* distance to boundary */

            //if (p.DP.Direction.Uz < 0.0)
            //    distanceToBoundary = ( Z.Start - p.DP.Position.Z) /
            //        p.DP.Direction.Uz;
            //else if (p.DP.Direction.Uz > 0.0)
            //    distanceToBoundary = ( Z.Stop - p.DP.Position.Z) /
            //        p.DP.Direction.Uz;

            //if ((p.DP.Direction.Uz != 0.0) && (p.S > distanceToBoundary))
            //{
            //    //photptr.HitBoundary = true;
            //    ////photptr.SLeft = (photptr.S - distanceToBoundary) * (mua + mus);  // DAW
            //    //photptr.SLeft = (photptr.S - distanceToBoundary) * photptr._tissue.Regions[photptr.CurrentRegionIndex].ScatterLength;
            //    //photptr.S = distanceToBoundary;
            //    return true;
            //}
            //else
           //    return false;
        }

        ///// <summary>
        ///// Checks if the specified photon will intersect the voxel boundary
        ///// </summary>
        ///// <param name="p">Photon to check for intersection (including Position, Direction, and S)</param>
        ///// <returns>True if photon will intersect the voxel boundary, false otherwise</returns>
        //public bool RayIntersectBoundary(Photon p)
        //{
        //    double dummyDistance;
        //    return RayIntersectBoundary(p, out dummyDistance);
        //}

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
