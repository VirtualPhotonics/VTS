using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines contract for TissueRegion classes.
    /// </summary>
    public interface ITissueRegion 
    {
        OpticalProperties RegionOP { get; }
        double ScatterLength { get; }
        bool ContainsPosition(Position position);
        ////bool RayIntersectBoundary(Photon photptr, ref double distanceToBoundary);
        //bool RayExitBoundary(Photon photptr, ref double distanceToBoundary);
    }
}