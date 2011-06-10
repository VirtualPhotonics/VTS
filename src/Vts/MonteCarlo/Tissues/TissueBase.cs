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

        public double GetDistanceToClosestVirtualBoundary(Photon photon)
        {
            var distance = double.PositiveInfinity;

            //if (_virtualBoundaries != null && _virtualBoundaries.Count > 0)
            //{
            //    foreach (var virtualBoundary in _virtualBoundaries)
            //    {
            //        var distanceToVB = virtualBoundary.GetDistanceToBoundary(photon);
                                
            //        if(distanceToVB <= distance)
            //        {
            //            distance = distanceToVB;
            //        }
            //    }
            //}

            return distance;
        }

        public IList<ITissueRegion> Regions { get; protected set; }
        public IList<double> RegionScatterLengths { get; protected set; }
        public AbsorptionWeightingType AbsorptionWeightingType { get; protected set; }
        public PhaseFunctionType PhaseFunctionType { get; protected set; }

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
