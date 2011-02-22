using System;
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
        private OpticalProperties _RegionOP;
        private double _ScatterLength;

        public VoxelRegion(DoubleRange x, DoubleRange y, DoubleRange z, OpticalProperties op, AbsorptionWeightingType awt) 
        {
            X = x;
            Y = y;
            Z = z;
            _RegionOP = op;
            _ScatterLength = op.GetScatterLength(awt);
        }
        public VoxelRegion() : this(
            new DoubleRange(-10.0, 10),
            new DoubleRange(-10.0, 10),
            new DoubleRange(0.0, 10),
            new OpticalProperties(0.01, 1.0, 0.8, 1.4), AbsorptionWeightingType.Discrete) {}  

        # region Properties
        public DoubleRange X { get; set; }
        public DoubleRange Y { get; set; }
        public DoubleRange Z { get; set; }
        public OpticalProperties RegionOP
        {
            get { return _RegionOP; }
        }
        public double ScatterLength
        {
            get { return _ScatterLength; }
        }
        #endregion

        public bool RayIntersectBoundary(Photon photptr, ref double distanceToBoundary)
        {
            distanceToBoundary = 0.0;  /* distance to boundary */

            if (photptr.DP.Direction.Uz < 0.0)
                distanceToBoundary = ( Z.Start - photptr.DP.Position.Z) /
                    photptr.DP.Direction.Uz;
            else if (photptr.DP.Direction.Uz > 0.0)
                distanceToBoundary = ( Z.Stop - photptr.DP.Position.Z) /
                    photptr.DP.Direction.Uz;

            if ((photptr.DP.Direction.Uz != 0.0) && (photptr.S > distanceToBoundary))
            {
                //photptr.HitBoundary = true;
                ////photptr.SLeft = (photptr.S - distanceToBoundary) * (mua + mus);  // DAW
                //photptr.SLeft = (photptr.S - distanceToBoundary) * photptr._tissue.Regions[photptr.CurrentRegionIndex].ScatterLength;
                //photptr.S = distanceToBoundary;
                return true;
            }
            else
                return false;
        }

        #region ITissueRegion Members


        public bool ContainsPosition(Position position)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
