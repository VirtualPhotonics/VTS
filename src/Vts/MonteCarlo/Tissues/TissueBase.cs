using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.Extensions;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Base class for all tissue definitions.
    /// </summary>
    public abstract class TissueBase : ITissue
    {
        /// <summary>
        /// constructor for tissue base
        /// </summary>
        /// <param name="regions">list of tissue regions</param>
        /// <param name="absorptionWeightingType">absorption weighting type</param>
        /// <param name="phaseFunctionType">phase function type</param>
        /// <param name="russianRouletteWeightThreshold">russian roulette weight threshold</param>
        public TissueBase(IList<ITissueRegion> regions, 
            AbsorptionWeightingType absorptionWeightingType,
            PhaseFunctionType phaseFunctionType,
            double russianRouletteWeightThreshold)
        {
            Regions = regions;
            AbsorptionWeightingType = absorptionWeightingType;
            PhaseFunctionType = phaseFunctionType;
            RegionScatterLengths = regions.Select(region => region.RegionOP.GetScatterLength(absorptionWeightingType)).ToArray();
            RussianRouletteWeightThreshold = russianRouletteWeightThreshold;
        }
        /// <summary>
        /// list of tissue regions
        /// </summary>
        public IList<ITissueRegion> Regions { get; protected set; }
        /// <summary>
        /// scatter lengths of region, either 1/mut or 1/mus depending on AbsorptionWeightingTyp
        /// </summary>
        public IList<double> RegionScatterLengths { get; protected set; }
        /// <summary>
        /// type of absorption deweighting employed
        /// </summary>
        public AbsorptionWeightingType AbsorptionWeightingType { get; protected set; }
        /// <summary>
        /// type of phase function used within region
        /// </summary>
        public PhaseFunctionType PhaseFunctionType { get; protected set; }
        /// <summary>
        /// photon weight threshold, below which turns on Russian Roulette
        /// </summary>
        public double RussianRouletteWeightThreshold { get; protected set; }

        /// <summary>
        /// method to determine region index (according to list of tissue regions) of current
        /// photon position
        /// </summary>
        /// <param name="position">Position of photon</param>
        /// <returns>region index</returns>
        public abstract int GetRegionIndex(Position position);
        /// <summary>
        /// method to determine distance to tissue boundary
        /// </summary>
        /// <param name="photon">current Photon state</param>
        /// <returns>distance to closest boundary</returns>
        public abstract double GetDistanceToBoundary(Photon photon);
        /// <summary>
        /// method to determine photon's angle relative to the boundary normal
        /// </summary>
        /// <param name="photon">current Photon state</param>
        /// <returns>angle relative to normal</returns>
        public abstract double GetAngleRelativeToBoundaryNormal(Photon photon);
        /// <summary>
        /// method to determine index of region photon is about to enter 
        /// </summary>
        /// <param name="photon">current Photon state</param>
        /// <returns>index of neighbor region</returns>
        public abstract int GetNeighborRegionIndex(Photon photon);
        /// <summary>
        /// method to determine if on domain of tissue.
        /// </summary>
        /// <param name="position">Photon position</param>
        /// <returns>true if on boundary, false if not</returns>
        public abstract bool OnDomainBoundary(Position position);
        /// <summary>
        /// method to determine state of Photon upon exit from tissue domain
        /// </summary>
        /// <param name="position">Photon position</param>
        /// <returns>PhotonStateType enum</returns>
        public abstract PhotonStateType GetPhotonDataPointStateOnExit(Position position);
        /// <summary>
        /// method to determine reflected direction of photon given its current position and direction
        /// </summary> 
        /// <param name="currentPosition">current position of photon</param>
        /// <param name="currentDirection">current direction of photon</param>
        /// <returns>new direction of photon</returns>
        public abstract Direction GetReflectedDirection(Position currentPosition, Direction currentDirection);
        /// <summary>
        /// method to determine refracted direction of photon given its current position and direction
        /// </summary>
        /// <param name="currentPosition">current position of photon</param>
        /// <param name="currentDirection">current direction of photon</param>
        /// <param name="currentN">current refractive index</param>
        /// <param name="nextN">refractive index of next tissue region</param>
        /// <param name="cosThetaSnell">cos(theta) according to Snell's law of refracted angle</param>
        /// <returns>new direction of photon</returns>
        public abstract Direction GetRefractedDirection(Position currentPosition, Direction currentDirection, double currentN, double nextN, double cosThetaSnell);
    }

}
