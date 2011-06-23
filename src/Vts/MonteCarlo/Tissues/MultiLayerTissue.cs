using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissue.  Defines tissue geometries comprised of layers
    /// (including homogenous with air layers above and below).
    /// </summary>
    public class MultiLayerTissue : TissueBase
    {
        /// <summary>
        /// Creates an instance of a MultiLayerTissue
        /// </summary>
        /// <param name="regions"></param>
        /// <param name="absorptionWeightingType"></param>
        /// <remarks>air above and below tissue needs to be specified for a slab geometry</remarks>
        public MultiLayerTissue(IList<ITissueRegion> regions, AbsorptionWeightingType absorptionWeightingType, PhaseFunctionType phaseFunctionType)
            : base(regions, absorptionWeightingType, phaseFunctionType)
        {
        }

        /// <summary>
        /// Creates an instance of a MultiLayerTissue based on an input data class 
        /// </summary>
        /// <param name="input"></param>
        /// <remarks>air above and below tissue needs to be specified for a slab geometry</remarks>
        public MultiLayerTissue(MultiLayerTissueInput input, AbsorptionWeightingType absorptionWeightingType, PhaseFunctionType phaseFunctionType)
            : this(input.Regions, absorptionWeightingType, phaseFunctionType)
        {
        }

        /// <summary>
        /// Creates a default instance of a MultiLayerTissue based on a homogeneous medium slab geometry
        /// and discrete absorption weighting
        /// </summary>
        public MultiLayerTissue() 
            : this(new MultiLayerTissueInput(), AbsorptionWeightingType.Discrete, PhaseFunctionType.HenyeyGreenstein)
        {
        }

        public override int GetRegionIndex(Position position)
        {
            // this method finds the first layer satisfying the criteria below:
            Func<LayerRegion, bool> containsPosition =
                region => 
                    position.Z >= region.ZRange.Start && 
                    position.Z < region.ZRange.Stop;
           
            // this is the long method but it works
            int index = -99;
            for (int i = 0; i < Regions.Count(); i++)
            {
                if (containsPosition((LayerRegion)Regions[i]))
                {
                    index = i;
                }
            }
            return index;
        }
        
        /// <summary>
        /// Finds the distance to the next boundary and subsequently 
        /// calls photon.HitBoundaryAndAdjustTrackLength()
        /// </summary>
        /// <param name="photon"></param>
        public override double GetDistanceToBoundary(Photon photon)
        {
            if (photon.DP.Direction.Uz == 0.0)
            {
                return double.PositiveInfinity;
            }

            // going "up" in negative z-direction
            bool goingUp = photon.DP.Direction.Uz < 0.0;

            // get current and adjacent regions
            int currentRegionIndex = photon.CurrentRegionIndex;
            LayerRegion currentRegion = (LayerRegion)Regions[currentRegionIndex];

            // calculate distance to boundary based on z-projection of photon trajectory
            double distanceToBoundary =
                goingUp
                    ? (currentRegion.ZRange.Start - photon.DP.Position.Z) / photon.DP.Direction.Uz
                    : (currentRegion.ZRange.Stop - photon.DP.Position.Z) / photon.DP.Direction.Uz;


            return distanceToBoundary;
        }

        public override bool OnDomainBoundary(Photon photon)
        {
            // this code assumes that the first and last layer is air
            return 
                photon.DP.Position.Z < 1e-10 ||
                (Math.Abs(photon.DP.Position.Z - ((LayerRegion)Regions[Regions.Count() - 2]).ZRange.Stop) < 1e-10);
        }

        public override int GetNeighborRegionIndex(Photon photon)
        {
            if (photon.DP.Direction.Uz == 0.0)
            {
                throw new Exception("GetNeighborRegionIndex called and Photon not on boundary");
            }

            if (photon.DP.Direction.Uz > 0.0)
            {
                return Math.Min(photon.CurrentRegionIndex + 1, Regions.Count - 1);
            }
                
            return Math.Max(photon.CurrentRegionIndex - 1, 0);
        }

        public override PhotonStateType GetPhotonDataPointStateOnExit(Position position)
        {
            if (position.Z < 1e-10)
            {
                return PhotonStateType.PseudoTransmissionDomainTopBoundary;
            }
            
            return PhotonStateType.PseudoTransmissionDomainBottomBoundary;
        }

        public override Direction GetReflectedDirection(
            Position positionCurrent, 
            Direction directionCurrent)
        {
            return new Direction(
                directionCurrent.Ux,
                directionCurrent.Uy,
                -directionCurrent.Uz);
        }

        public override Direction GetRefractedDirection(
            Position positionCurrent, 
            Direction directionCurrent, 
            double nCurrent, 
            double nNext, 
            double cosThetaSnell)
        {
            if (directionCurrent.Uz > 0)
                return new Direction(
                    directionCurrent.Ux * nCurrent / nNext,
                    directionCurrent.Uy * nCurrent / nNext,
                    cosThetaSnell);
            else
                return new Direction(
                    directionCurrent.Ux * nCurrent / nNext,
                    directionCurrent.Uy * nCurrent / nNext,
                    -cosThetaSnell);
        }

        public override double GetAngleRelativeToBoundaryNormal(Photon photon)
        {
            return Math.Abs(photon.DP.Direction.Uz); // abs will work for upward normal and downward normal
        }
    }
}
