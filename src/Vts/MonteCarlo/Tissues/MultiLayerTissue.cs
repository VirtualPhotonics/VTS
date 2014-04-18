using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissue.  Defines tissue geometries comprised of layers
    /// (including homogenous with air layers above and below).  Layers are infinite along
    /// x- and y- axes.
    /// </summary>
    public class MultiLayerTissue : TissueBase
    {
        private IList<LayerRegion> _layerRegions;

        /// <summary>
        /// Creates an instance of a MultiLayerTissue
        /// </summary>
        /// <param name="regions">list of tissue regions comprising tissue</param>
        /// <param name="absorptionWeightingType">absorption weighting type</param>
        /// <param name="phaseFunctionType">phase function type</param>
        /// <param name="russianRouletteWeightThreshold">photon weight threshold to turn on Russian Roulette</param>
        /// <remarks>air above and below tissue needs to be specified for a slab geometry</remarks>
        public MultiLayerTissue(
            IList<ITissueRegion> regions, 
            AbsorptionWeightingType absorptionWeightingType, 
            PhaseFunctionType phaseFunctionType,
            double russianRouletteWeightThreshold)
            : base(regions, absorptionWeightingType, phaseFunctionType,russianRouletteWeightThreshold)
        {
            _layerRegions = regions.Select(region => (LayerRegion) region).ToArray();
        }
        
        /// <summary>
        /// Creates an instance of a MultiLayerTissue based on an input data class 
        /// </summary>
        /// <param name="input">multi-layer tissue input</param>
        /// <param name="absorptionWeightingType">absorption weighting type</param>
        /// <param name="phaseFunctionType">phase function type</param>
        /// <param name="russianRouletteWeightThreshold">russian roulette weight threshold</param>
        /// <remarks>air above and below tissue needs to be specified for a slab geometry</remarks>
        public MultiLayerTissue(
            MultiLayerTissueInput input, 
            AbsorptionWeightingType absorptionWeightingType, 
            PhaseFunctionType phaseFunctionType,
            double russianRouletteWeightThreshold)
            : this(input.Regions, absorptionWeightingType, phaseFunctionType, russianRouletteWeightThreshold)
        {
        }

        /// <summary>
        /// Creates a default instance of a MultiLayerTissue based on a homogeneous medium slab geometry
        /// and discrete absorption weighting
        /// </summary>
        public MultiLayerTissue() 
            : this(new MultiLayerTissueInput().Regions, AbsorptionWeightingType.Discrete, PhaseFunctionType.HenyeyGreenstein, 0.0)
        {
        }

        /// <summary>
        /// method to determine region index of region photon is currently in
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override int GetRegionIndex(Position position)
        {
            // use ITissueRegion interface method ContainsPosition for LayerRegion to determine
            // which region photon resides

            int index = -1;
            for (int i = 0; i < _layerRegions.Count(); i++)
            {
                if (_layerRegions[i].ContainsPosition(position))
                {
                    index = i;
                }
            }
            return index;
        }
        
        /// <summary>
        /// Finds the distance to the next boundary and independent of hitting it
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
            // check if in embedded tissue region ckh fix 8/10/11
            LayerRegion currentRegion = _layerRegions[1];
            if (currentRegionIndex < _layerRegions.Count)
            {
                currentRegion = _layerRegions[currentRegionIndex];
            }

            // calculate distance to boundary based on z-projection of photon trajectory
            double distanceToBoundary =
                goingUp
                    ? (currentRegion.ZRange.Start - photon.DP.Position.Z) / photon.DP.Direction.Uz
                    : (currentRegion.ZRange.Stop - photon.DP.Position.Z) / photon.DP.Direction.Uz;


            return distanceToBoundary;
        }
        /// <summary>
        /// method to determine if on boundary of tissue, i.e. at tissue/air interface
        /// </summary>
        /// <param name="position">photon position</param>
        /// <returns></returns>
        public override bool OnDomainBoundary(Position position)
        {
            // this code assumes that the first and last layer is air
            return 
                position.Z < 1e-10 ||
                (Math.Abs(position.Z - (_layerRegions.Last()).ZRange.Start) < 1e-10);
        }
        /// <summary>
        /// method to determine index of region photon is about to enter
        /// </summary>
        /// <param name="photon">photon info including position and direction</param>
        /// <returns>region index</returns>
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
        /// <summary>
        /// method to determine photon state type of photon exiting tissue boundary
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override PhotonStateType GetPhotonDataPointStateOnExit(Position position)
        {
            if (position.Z < 1e-10)
            {
                return PhotonStateType.PseudoReflectedTissueBoundary;
            }
            
            return PhotonStateType.PseudoTransmittedTissueBoundary;
        }
        /// <summary>
        /// method to determine direction of reflected photon
        /// </summary>
        /// <param name="positionCurrent"></param>
        /// <param name="directionCurrent"></param>
        /// <returns></returns>
        public override Direction GetReflectedDirection(
            Position positionCurrent, 
            Direction directionCurrent)
        {
            return new Direction(
                directionCurrent.Ux,
                directionCurrent.Uy,
                -directionCurrent.Uz);
        }
        /// <summary>
        /// method to determine refracted direction of photon
        /// </summary>
        /// <param name="positionCurrent">current photon position</param>
        /// <param name="directionCurrent">current photon direction</param>
        /// <param name="nCurrent">refractive index of current region</param>
        /// <param name="nNext">refractive index of next region</param>
        /// <param name="cosThetaSnell">cos(theta) resulting from Snell's law</param>
        /// <returns>direction</returns>
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
        /// <summary>
        /// method to get angle between photons current direction and boundary normal
        /// </summary>
        /// <param name="photon"></param>
        /// <returns></returns>
        public override double GetAngleRelativeToBoundaryNormal(Photon photon)
        {
            return Math.Abs(photon.DP.Direction.Uz); // abs will work for upward normal and downward normal
        }
    }
}
