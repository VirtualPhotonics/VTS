using System;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueRegion.  Defines circular region at surface of tissue
    /// </summary>
    public class SurfaceFiberTissueRegion : ITissueRegion
    {
        private bool _onBoundary = false;

        /// <summary>
        /// SurfaceFiberTissueRegion assumes SurfaceFiber axis is parallel with z-axis
        /// </summary>
        /// <param name="center">center position</param>
        /// <param name="radius">radius in x-y plane</param>
        /// <param name="op">optical properties of SurfaceFiber</param>
        public SurfaceFiberTissueRegion(Position center, double radius, OpticalProperties op, string phaseFunctionKey) 
        {
            TissueRegionType = "SurfaceFiber";
            Center = center;
            Radius = radius;
            RegionOP = op;
            PhaseFunctionKey = phaseFunctionKey;
        }
        /// <summary>
        /// default constructor
        /// </summary>
        public SurfaceFiberTissueRegion() : this(new Position(0, 0, 5), 1,
            new OpticalProperties(0.01, 1.0, 0.8, 1.4), "HenyeyGreensteinKey1") {}

        /// <summary>
        /// tissue region identifier
        /// </summary>
        public string TissueRegionType { get; set; }

        /// <summary>
        /// center of SurfaceFiber
        /// </summary>
        public Position Center { get; set; }
        /// <summary>
        /// radius of SurfaceFiber
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// optical properties of SurfaceFiber
        /// </summary>
        public OpticalProperties RegionOP { get; set; }
        /// <summary>
        /// phase function key
        /// </summary>
        public string PhaseFunctionKey { get; set; }

        /// <summary>
        /// Method to determine if photon position within or on SurfaceFiber.  This works if height=0
        /// as long as Center.Z=0;
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>boolean</returns>
        public bool ContainsPosition(Position position)
        {
            // check axial extent first
            if (Math.Abs(position.Z) < 1e-6)
            {
                double inside = ((position.X - Center.X) * (position.X - Center.X) + 
                                 (position.Y - Center.Y) * (position.Y - Center.Y)) / (Radius * Radius);

                if (inside < 0.9999999999) // prior check 0.9999999
                {
                    return true;
                }
                else if (inside > 1.00000000001) // prior check  1.0000001
                {
                    return false;
                }
                else  // on boundary means SurfaceFiber contains position
                {
                    _onBoundary = true;
                    return true;  // ckh 2/28/19 this has to return true 
                }
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// Method to determine if photon on boundary of SurfaceFiber.
        /// Currently OnBoundary of an inclusion region isn't called by any code ckh 3/5/19.
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>boolean</returns>
        public bool OnBoundary(Position position)
        {
            //an option to the following would be to:
            //return ((position.Z == Center.Z + Height) || (position.Z == Center.Z - Height)) &&
            //    Math.Sqrt(position.X * position.X + position.Y * position.Y) == Radius
            return !ContainsPosition(position) && _onBoundary; // match with EllipsoidTissueRegion
        }

        /// <summary>
        /// method to determine if photon track intersects boundary
        /// </summary>
        /// <param name="photon">photon</param>
        /// <param name="distanceToBoundary">if intersection, distance to intersection</param>
        /// <returns>boolean true=intersection, false=no intersection</returns>
        public bool RayIntersectBoundary(Photon photon, out double distanceToBoundary)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// method to determine normal to surface at given position
        /// </summary>
        /// <param name="position">position to calculate normal</param>
        /// <returns>Direction</returns>
        public Direction SurfaceNormal(Position position)
        {
            throw new NotImplementedException();
        }
    }
}
