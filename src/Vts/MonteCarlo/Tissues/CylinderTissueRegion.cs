using System;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueRegion.  Defines cylindrical region with dimensions
    /// Center, Radius and Height.
    /// </summary>
    public class CylinderTissueRegion : ITissueRegion
    {
        /// <summary>
        /// </summary>
        /// <param name="center">center position</param>
        /// <param name="radius">radius in x-y plane</param>
        /// <param name="height">height along z axis</param>
        /// <param name="op">optical properties of cylinder</param>
        public CylinderTissueRegion(Position center, double radius, double height, OpticalProperties op, string phaseFunctionKey) 
        {
            TissueRegionType = "Cylinder";
            Center = center;
            Radius = radius;
            Height = height;
            RegionOP = op;
            PhaseFunctionKey = phaseFunctionKey;
        }

        /// <summary>
        /// default constructor
        /// </summary>
        public CylinderTissueRegion() : this(
            new Position(0, 0, 5), 
            1, 
            5, 
            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
            "HenyeyGreensteinKey1") { }

        /// <summary>
        ///  tissue region identifier
        /// </summary>
        public string TissueRegionType { get; set; }

        /// <summary>
        /// center of cyliner
        /// </summary>
        public Position Center { get; set; }
        /// <summary>
        /// radius of cylinder
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// height of cylinder
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// optical properties of cylinder
        /// </summary>
        public OpticalProperties RegionOP { get; set; }
        /// <summary>
        /// key for the <string, IPhaseFunctionInput> dictionary in a class that implements ITissueInput
        /// </summary>
        public string PhaseFunctionKey { get; set; }
        /*/// <summary>
        /// Input data for phase function
        /// </summary>
        public IPhaseFunctionInput PhaseFunctionInput { get; set; }*/

        /// <summary>
        /// method to determine if photon position within or on cylinder
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>boolean</returns>
        public bool ContainsPosition(Position position)
        {
            if (((Math.Sqrt(position.X * position.X + position.Y * position.Y) <= Radius)) &&
                (position.Z <= Center.Z + Height) &&
                (position.Z >= Center.Z - Height))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Method to determine if photon on boundary of cylinder.
        /// Currently OnBoundary of an inclusion region isn't called by any code ckh 3/5/19.
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns>boolean</returns>
        public bool OnBoundary(Position position)
        {
            return ((position.Z == Center.Z + Height) || (position.Z == Center.Z - Height)) &&
                Math.Sqrt(position.X * position.X + position.Y * position.Y) == Radius;
        }

        /// <summary>
        /// method to determine if photon ray (or track) will intersect boundary of cylinder
        /// </summary>
        /// <param name="photon">photon position, direction, etc.</param>
        /// <param name="distanceToBoundary">distance to boundary</param>
        /// <returns>boolean</returns>
        public bool RayIntersectBoundary(Photon photon, out double distanceToBoundary)
        {
            throw new NotImplementedException();
        }

        //public bool RayIntersectBoundary(Photon photptr)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// method to determine normal to surface at given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns>Direction</returns>
        public Direction SurfaceNormal(Position position)
        {
            throw new NotImplementedException();
        }
    }
}
