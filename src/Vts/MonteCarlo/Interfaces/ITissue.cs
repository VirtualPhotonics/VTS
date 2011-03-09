using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Tissue classes in Monte Carlo simulation.
    /// </summary>
    public interface ITissue
    {
        AbsorptionWeightingType AbsorptionWeightingType { get; }
        PhaseFunctionType PhaseFunctionType { get; }
        IList<ITissueRegion> Regions { get; }
        IList<double> RegionScatterLengths { get; }
        int GetRegionIndex(Position position);
        double GetDistanceToBoundary(Photon photon);  
        double GetAngleRelativeToBoundaryNormal(Photon photon);
        int GetNeighborRegionIndex(Photon photon);  
        bool OnDomainBoundary(Photon photon);  // no boundaries, possibly Tissue kills photons per situation
        PhotonStateType GetPhotonDataPointStateOnExit(Position position);
        Direction GetReflectedDirection(Position currentPosition, Direction currentDirection);
        Direction GetRefractedDirection(Position currentPosition, Direction currentDirection, 
            double currentN, double nextN, double cosThetaSnell);
    }
}
