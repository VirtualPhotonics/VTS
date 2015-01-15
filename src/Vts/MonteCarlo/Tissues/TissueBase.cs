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
        /// Required method to initialiize the corresponding ITissue
        /// </summary>
        /// <param name="tissue"></param>
        public void Initialize(
            AbsorptionWeightingType awt = AbsorptionWeightingType.Discrete, 
            PhaseFunctionType pft = PhaseFunctionType.HenyeyGreenstein,
            double russianRouletteWeightThreshold = 0.0)
        {
            AbsorptionWeightingType = awt;
            PhaseFunctionType = pft;
            RussianRouletteWeightThreshold = russianRouletteWeightThreshold;

            RegionScatterLengths = Regions.Select(region => region.RegionOP.GetScatterLength(awt)).ToArray();
        }
    }

}
