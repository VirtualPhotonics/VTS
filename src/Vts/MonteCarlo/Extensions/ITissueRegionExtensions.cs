namespace Vts.MonteCarlo.Extensions
{
    /// <summary>
    /// extension methods for ITissueRegion classes
    /// </summary>
    public static class ITissueRegionExtensions
    {
        /// <summary>
        /// Method to determine if tissue region is air or not.
        /// </summary>
        /// <param name="region">tissue region</param>
        /// <returns>Boolean</returns>
        public static bool IsAir(this ITissueRegion region)
        {
            return region.RegionOP.Mua == 0D && region.RegionOP.Mus <= 1E-10;
        }
    }
}