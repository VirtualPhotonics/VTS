namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for TissueInput.
    /// </summary>
    public interface ITissueInput
    {
        /// <summary>
        /// Type of tissue
        /// </summary>
        string TissueType { get; }
        
        /// <summary>
        /// List of tissue regions comprising tissue.
        /// </summary>
        ITissueRegion[] Regions { get; }
        
        /// <summary>
        ///// Required factory method to create the corresponding 
        ///// ITissue based on the ITissueInput data
        /// </summary>
        /// <param name="awt">Absorption Weighting Type</param>
        /// <param name="pft">Phase Function Type</param>
        /// <param name="russianRouletteWeightThreshold">Russian Roulette Weight Threshold</param>
        /// <returns></returns>
        ITissue CreateTissue(AbsorptionWeightingType awt, PhaseFunctionType pft, double russianRouletteWeightThreshold);  
    }
}
