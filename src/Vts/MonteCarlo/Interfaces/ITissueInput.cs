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
        /// Required factory method to create the corresponding 
        /// ITissue based on the ITissueInput data
        /// </summary>
        /// <returns></returns>
        ITissue CreateTissue();  
    }
}
