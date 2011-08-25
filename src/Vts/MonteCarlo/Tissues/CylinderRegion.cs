using System;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.Extensions;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueRegion.  Defines cylindrical region with dimensions
    /// Center, Radius and Height.
    /// </summary>
    public class CylinderRegion : ITissueRegion
    {
        /// <summary>
        /// CylinderRegion assumes cylinder axis is parallel with z-axis
        /// </summary>
        /// <param name="center">center position</param>
        /// <param name="radius">radius in x-y plane</param>
        /// <param name="height">height along z axis</param>
        /// <param name="op"></param>
        /// <param name="awt"></param>
        public CylinderRegion(Position center, double radius, double height, OpticalProperties op, AbsorptionWeightingType awt) 
        {
            Center = center;
            Radius = radius;
            Height = height;
            RegionOP = op;
        }

        public CylinderRegion() : this(new Position(0, 0, 5), 1, 5, 
            new OpticalProperties(0.01, 1.0, 0.8, 1.4), AbsorptionWeightingType.Discrete) {}

        public Position Center { get; set; }

        public double Radius { get; set; }

        public double Height { get; set; }

        public OpticalProperties RegionOP { get; set; }
        
        public bool ContainsPosition(Position position)
        {
            if (((Math.Sqrt(position.X * position.X + position.Y * position.Y) < Radius)) &&
                (position.Z < Center.Z + Height) &&
                (position.Z > Center.Z - Height))
                return true;
            else
                return false;
        }

        public bool RayIntersectBoundary(Photon photptr, out double distanceToBoundary)
        {
            throw new NotImplementedException();
        }

        public bool RayIntersectBoundary(Photon photptr)
        {
            throw new NotImplementedException();
        }
    }
}
