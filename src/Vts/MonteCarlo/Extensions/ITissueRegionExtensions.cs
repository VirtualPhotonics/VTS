namespace Vts.MonteCarlo.Extensions
{
    /// <summary>
    /// extension methods for ITissueRegion classes
    /// </summary>
    public static class ITissueRegionExtensions
    {
        /// <summary>
        /// Method to determine if tissue region is air or not.
        /// Updated so that glass OPs (n=1.5, mus=1E-10) not included
        /// </summary>
        /// <param name="region">tissue region</param>
        /// <returns>boolean</returns>
        public static bool IsAir(this ITissueRegion region)
        {
            return region.RegionOP.Mua == 0D && region.RegionOP.Mus <= 1E-10 &&
                   region.RegionOP.N == 1.0;
        }
    }
}