using System;



namespace Vts.FemModeling.MGRTE._2D{
    /// <summary>
    /// Instantiates appropriate ITissue given ITissueInput.
    /// </summary>
    public static class TissueFactory
    {
        // todo: revisit to make signatures here and in Tissue/TissueInput class signatures strongly typed
        /// <summary>
        /// Method to return ITissue given inputs
        /// </summary>
        /// <param name="ti">ITissueInput</param>
        /// <returns>ITissue</returns>
        public static ITissue GetTissue(ITissueInput ti)
        {
            ITissue t = null;
            var multiLayerTissueInput = (MultiLayerTissueInput)ti;
            if (multiLayerTissueInput != null)
            {
                t = (ITissue) new MultiLayerTissue(multiLayerTissueInput.Regions);
            }
            
            if (t == null)
                throw new ArgumentException("Problem generating ITissue instance. " +
                                            "Check that TissueInput, ti, has a matching ITissue definition.");

            return t;
        }
    }
}
