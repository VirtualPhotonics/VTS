using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.Extensions;

namespace Vts.MonteCarlo.Tissues
{

    public abstract class TissueBase : ITissue
    {
        public TissueBase(IList<ITissueRegion> regions, AbsorptionWeightingType absorptionWeightingType)
        {
            Regions = regions;
            AbsorptionWeightingType = absorptionWeightingType;
            RegionScatterLengths = regions.Select(region => region.RegionOP.GetScatterLength(absorptionWeightingType)).ToArray();
        }

        public IList<ITissueRegion> Regions { get; protected set; }
        public IList<double> RegionScatterLengths { get; protected set; }
        public AbsorptionWeightingType AbsorptionWeightingType { get; protected set; }

        public abstract int GetRegionIndex(Position position);
        public abstract double GetDistanceToBoundary(Photon photon);
        public abstract double GetAngleRelativeToBoundaryNormal(Photon photon);
        public abstract int GetNeighborRegionIndex(Photon photon);
        public abstract bool OnDomainBoundary(Photon photon);
        public abstract PhotonStateType GetPhotonDataPointStateOnExit(Position position);
        public abstract Direction GetReflectedDirection(Position currentPosition, Direction currentDirection);
        public abstract Direction GetRefractedDirection(Position currentPosition, Direction currentDirection, double currentN, double nextN, double cosThetaSnell);
    }

}
