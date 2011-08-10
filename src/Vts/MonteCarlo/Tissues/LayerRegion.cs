using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{

    /// <summary>
    /// Implements ITissueRegion.  Defines a layer infinite in extent along
    /// x,y-axes and with extent along z-axis given by ZRange.
    /// </summary>
    public class LayerRegion : ITissueRegion
    {
        public LayerRegion(DoubleRange zRange, OpticalProperties op)
        {
            ZRange = zRange;
            RegionOP = op;
        }

        public LayerRegion()
            : this(
                new DoubleRange(0.0, 10),
                new OpticalProperties(0.01, 1.0, 0.8, 1.4)) { }

        public DoubleRange ZRange { get; set; }
        public OpticalProperties RegionOP { get; set; }

        public bool ContainsPosition(Position p)
        {
            return p.Z >= ZRange.Start && p.Z < ZRange.Stop;
        }

        //public bool RayExitBoundary(Photon photptr, ref double distanceToBoundary)
        //{
        //    distanceToBoundary = 0.0;  /* distance to boundary */

        //    if (photptr.DP.Direction.Uz < 0.0)
        //        distanceToBoundary = ( Z.Start - photptr.DP.Position.Z) /
        //            photptr.DP.Direction.Uz;
        //    else if (photptr.DP.Direction.Uz > 0.0)
        //        distanceToBoundary = ( Z.Stop - photptr.DP.Position.Z) /
        //            photptr.DP.Direction.Uz;

        //    if ((photptr.DP.Direction.Uz != 0.0) && (photptr.S > distanceToBoundary))
        //    {
        //        //photptr.HitBoundary = true;
        //        ////photptr.SLeft = (photptr.S - distanceToBoundary) * (mua + mus);  // DAW
        //        //photptr.SLeft = (photptr.S - distanceToBoundary) * photptr._tissue.Regions[photptr.CurrentRegionIndex].ScatterLength;
        //        //photptr.S = distanceToBoundary;
        //        return true;
        //    }
        //    else
        //        return false;
        //}
    }
}
