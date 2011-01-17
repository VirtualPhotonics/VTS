using System;
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
        private OpticalProperties _OP;
        private double _ScatterLength;
        private Position _Center;
        private double _Radius;
        private double _Height;
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
            _Center = center;
            _Radius = radius;
            _Height = height;
            _OP = op;
            _ScatterLength = op.GetScatterLength(awt);
        }
        public CylinderRegion() : this(new Position(0, 0, 5), 1, 5, 
            new OpticalProperties(0.01, 1.0, 0.8, 1.4), AbsorptionWeightingType.Discrete) {}  

        # region Properties
        public Position Center
        {
            get { return _Center; }
        }
        public double Radius
        {
            get { return _Radius; }
        }
        public double Height
        {
            get { return _Height; }
        }
        public OpticalProperties RegionOP
        {
            get { return _OP; }
        }
        public double ScatterLength
        {
            get { return _ScatterLength; }
        }
        #endregion
        public bool ContainsPosition(Position position)
        {
            if (((Math.Sqrt(position.X * position.X + position.Y * position.Y) < _Radius)) &&
                (position.Z < _Center.Z + _Height) &&
                (position.Z > _Center.Z - _Height))
                return true;
            else
                return false;
        }

        public bool RayIntersectBoundary(Photon photptr, ref double distanceToBoundary)
        {
            throw new NotImplementedException();
        }
    }
}
