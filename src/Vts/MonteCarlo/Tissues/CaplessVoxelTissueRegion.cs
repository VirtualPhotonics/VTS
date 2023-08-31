using System;
using System.Runtime.Serialization;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueRegion.  Defines Cartesian coordinate voxel with x,y,z ranges
    /// and NO top or bottom
    /// </summary>
    public class CaplessVoxelTissueRegion : ITissueRegion
    {
        private bool _onBoundary = false;
        /// <summary>
        /// constructor for voxel region
        /// </summary>
        /// <param name="x">x range of voxel</param>
        /// <param name="y">y range of voxel</param>
        /// <param name="z">z range of voxel</param>
        /// <param name="op">optical properties of voxel</param>
        public CaplessVoxelTissueRegion(DoubleRange x, DoubleRange y, DoubleRange z, OpticalProperties op)
        {
            TissueRegionType = "CaplessVoxel";
            X = x;
            Y = y;
            Z = z;
            RegionOP = op;
        }

        /// <summary>
        /// default constructor
        /// </summary>
        public CaplessVoxelTissueRegion() : this(
            new DoubleRange(-10.0, 10),
            new DoubleRange(-10.0, 10),
            new DoubleRange(0.0, 10),
            new OpticalProperties(0.01, 1.0, 0.8, 1.4))
        {
        }

        /// <summary>
        /// tissue region identifier
        /// </summary>
        public string TissueRegionType { get; set; }

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
            get =>
                new(
                    (X.Start + X.Stop)/2,
                    (Y.Start + Y.Stop)/2,
                    (Z.Start + Z.Stop)/2);
            set
            {
                var oldCenter = Center;

                var dx = value.X - oldCenter.X;
                var dy = value.Y - oldCenter.Y;
                var dz = value.Z - oldCenter.Z;

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
        /// EXCEPT the top and bottom
        /// </summary>
        /// <param name="photon">Photon to check for intersection (including Position, Direction, and S)</param>
        /// <param name="distanceToBoundary">The distance to the next boundary</param>
        /// <returns>True if photon will intersect the voxel boundary, false otherwise</returns>
        public bool RayIntersectBoundary(Photon photon, out double distanceToBoundary)
        {
            distanceToBoundary = double.PositiveInfinity;
            var dp = photon.DP;
            var p1 = dp.Position;
            var d1 = dp.Direction;

            // determine location of end of ray
            var p2 = new Position(p1.X + d1.Ux*photon.S, p1.Y + d1.Uy*photon.S, p1.Z + d1.Uz*photon.S);

            var oneIn = this.ContainsPosition(p1);
            var twoIn = this.ContainsPosition(p2);

            // check if ray within voxel 
            if (oneIn && twoIn)
            {
                return false;
            }
            // check if ray outside voxel
            if (!oneIn && !twoIn &&
                p1.X < X.Start && p2.X < X.Start || (p1.X > X.Stop && p2.X > X.Stop) ||
                (p1.Y < Y.Start && p2.Y < Y.Start) || (p1.Y > Y.Stop && p2.Y > Y.Stop) ||
                (p1.Z < Z.Start && p2.Z < Z.Start) || (p1.Z > Z.Stop && p2.Z > Z.Stop))
            {
                return false;
            }

            // the following code makes sure that distanceToBoundary is calculated
            // correctly if photon just slightly off boundary
            if (Math.Abs(p1.X - X.Start) < 1e-11) p1.X = X.Start;
            if (Math.Abs(p1.X - X.Stop) < 1e-11) p1.X = X.Stop; 
            if (Math.Abs(p1.Y - Y.Start) < 1e-11) p1.Y = Y.Start;
            if (Math.Abs(p1.Y - Y.Stop) < 1e-11) p1.Y = Y.Stop; 
            // no check on Z since capless
            
            // following algorithm from tavianator.com/fast-branchless-raybounding-box-intersections
            // check intersections of ray with planes that make up box
            var dmin = double.NegativeInfinity;
            var dmax = double.PositiveInfinity;
            var dist1 = (X.Start - p1.X)/d1.Ux;
            var dist2 = (X.Stop - p1.X)/d1.Ux;
            dmin = Math.Max(dmin, Math.Min(dist1, dist2));
            dmax = Math.Min(dmax, Math.Max(dist1, dist2));
            dist1 = (Y.Start - p1.Y)/d1.Uy;
            dist2 = (Y.Stop - p1.Y)/d1.Uy;
            dmin = Math.Max(dmin, Math.Min(dist1, dist2));
            dmax = Math.Min(dmax, Math.Max(dist1, dist2));
            // no check on Z since capless
            if (!(dmax >= dmin)) return false;
            double xint;
            double yint;
            double zint;
            if (dmin > 0)
            {
                xint = p1.X + d1.Ux * dmin;
                yint = p1.Y + d1.Uy * dmin;
                zint = p1.Z + d1.Uz * dmin;  
            }
            else if (dmax > 0)
            {
                xint = p1.X + d1.Ux * dmax;
                yint = p1.Y + d1.Uy * dmax;
                zint = p1.Z + d1.Uz * dmax;
            }
            else
            {
                return false;
            }

            /*distance to the boundary*/
            distanceToBoundary = Math.Sqrt((xint - p1.X) * (xint - p1.X) +
                                           (yint - p1.Y) * (yint - p1.Y) +
                                           (zint - p1.Z) * (zint - p1.Z));

            // ckh fix 9/23/15: check if on boundary of voxel
            return (distanceToBoundary >= 1e-11);
            // dmax < dmin
        }

        /// <summary>
        /// method to determine if photon at designated position resides within or on voxel
        /// </summary>
        /// <param name="position">position of photon</param>
        /// <returns>Boolean indicating whether region contains position or not</returns>
        public bool ContainsPosition(Position position)
        {
            //// inclusion defined in half-open interval [start,stop) so that continuum of voxels do not overlap
            return position.X >= X.Start && position.X <= X.Stop &&
                   position.Y >= Y.Start && position.Y <= Y.Stop &&
                   position.Z >= Z.Start && position.Z <= Z.Stop;
        }

        /// <summary>
        /// Method to determine if photon on boundary of voxel.
        /// Currently OnBoundary of an inclusion region isn't called by any code ckh 3/5/19.
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>Boolean indicating whether on boundary or not</returns>
        public bool OnBoundary(Position position)
        {
            const double tol = 1e-11;
            if (((Math.Abs(position.X - X.Start) < tol || Math.Abs(position.X - X.Stop) < tol) &&
                  position.Y >= Y.Start && position.Y <= Y.Stop &&
                  position.Z >= Z.Start && position.Z <= Z.Stop) ||
                ((Math.Abs(position.Y - Y.Start) < tol || Math.Abs(position.Y - Y.Stop) < tol) &&
                  position.X >= X.Start && position.X <= X.Stop &&
                  position.Z >= Z.Start && position.Z <= Z.Stop))
                // no check on Z since capless
            {
                _onBoundary = true;
            }
            else
            {
                _onBoundary = false;
            }
            return _onBoundary; 
        }
        /// <summary>
        /// method to determine normal to surface at given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>Direction normal to surface at position</returns>
        public Direction SurfaceNormal(Position position)
        {
            const double tol = 1e-6; // use tolerance because position will have floating point errors
            // larger tolerance works because voxel edges typically have 4-5 significant digits

            // the following code doesn't handle if on or edge corner, but may not be problem
            if (Math.Abs(position.X - X.Start) < tol)
            {
                return new Direction(-1, 0, 0);
            }

            if (Math.Abs(position.X - X.Stop) < tol)
            {
                return new Direction(1, 0, 0);
            }

            if (Math.Abs(position.Y - Y.Start) < tol)
            {
                return new Direction(0, -1, 0);
            }

            return Math.Abs(position.Y - Y.Stop) < tol ? new Direction(0, 1, 0) :
                // no check on Z since capless
                null;
        }
    }
}
