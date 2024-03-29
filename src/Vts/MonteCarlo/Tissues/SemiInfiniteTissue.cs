using System;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to SemiInfiniteTissueInput class.
    /// </summary>
    public class SemiInfiniteTissueInput : TissueInput, ITissueInput
    {

        /// <summary>
        /// constructor for Semi-infinite tissue input
        /// </summary>
        /// <param name="region">tissue region info</param>
        public SemiInfiniteTissueInput(ITissueRegion region)
        {
            TissueType = "SemiInfinite";
            Regions = new[] { region };
        }

        /// <summary>
        /// SemiInfiniteTissueInput default constructor provides homogeneous tissue
        /// </summary>
        public SemiInfiniteTissueInput()
            : this(new SemiInfiniteTissueRegion(new OpticalProperties(0.02, 1.0, 0.8, 1.4)))
        {
        }

        /// <summary>
        /// list of tissue regions comprising tissue
        /// </summary>
        public ITissueRegion[] Regions { get; set; }

        /// <summary>
        /// Required factory method to create the corresponding 
        /// ITissue based on the ITissueInput data
        /// </summary>
        /// <param name="awt">Absorption Weighting Type</param>
        /// <param name="pft">Phase Function Type</param>
        /// <param name="russianRouletteWeightThreshold">Russian Roulette Weight Threshold</param>
        /// <returns>instantiated tissue</returns>
        public ITissue CreateTissue(AbsorptionWeightingType awt, PhaseFunctionType pft, double russianRouletteWeightThreshold)
        {
            throw new NotImplementedException();
        }
    }
}
