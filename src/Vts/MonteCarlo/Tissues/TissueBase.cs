using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.Extensions;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Base class for all tissue inputs
    /// </summary>
    public abstract class TissueInput
    {
        public TissueInput()
        {
            TissueType = "";
        }

        // mandatory user inputs (required for ITissueInput contract)
        public string TissueType { get; set; }
    }

    /// <summary>
    /// Base class for all tissue definitions.
    /// </summary>
    public abstract class TissueBase
    {
        /// <summary>
        /// constructor for tissue base
        /// </summary>
        /// <param name="regions">list of tissue regions</param>
        public TissueBase(IList<ITissueRegion> regions)
        {
            Regions = regions;

            // obsolete: phase function now region-specific
            // PhaseFunctionType = phaseFunctionType;

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

        ///// <summary>
        ///// Phase function used within each region
        ///// </summary>
        public IDictionary<string, IPhaseFunction> RegionPhaseFunctions { get; set; }
        /// <summary>
        /// photon weight threshold, below which turns on Russian Roulette
        /// </summary>
        public double RussianRouletteWeightThreshold { get; protected set; }

        /// <summary>
        /// Required method to initialiize the corresponding ITissue
        /// </summary>
        /// <param name="awt">absorption weighting type</param>
        /// <param name="regionPhaseFunctions">dictionary of region phase functions</param>
        /// <param name="russianRouletteWeightThreshold">threshold for Russian Roulette</param>
        /// Note: phase function inputs go through the factory to convert to phase funcions in MonteCarloSimulation
        public void Initialize(
            AbsorptionWeightingType awt = AbsorptionWeightingType.Discrete,
            IDictionary<string,IPhaseFunction> regionPhaseFunctions = null,
            double russianRouletteWeightThreshold = 0.0)
        {
            AbsorptionWeightingType = awt;
            if (regionPhaseFunctions != null)
            {
                RegionPhaseFunctions = new Dictionary<string, IPhaseFunction>();
                foreach (var phaseFunction in regionPhaseFunctions)
                {
                    RegionPhaseFunctions.Add(phaseFunction.Key, phaseFunction.Value);
                }
            }
            RussianRouletteWeightThreshold = russianRouletteWeightThreshold;

            RegionScatterLengths = Regions.Select(region => region.RegionOP.GetScatterLength(awt)).ToArray();
        }
    }
}