namespace Vts.FemModeling.MGRTE._2D
{
    public static class IInclusionRegionExtensions
    {
        /// <summary>
        /// Method to determine if inclusion region is air or not
        /// </summary>
        /// <param name="region">inclusion region</param>
        /// <returns>boolean</returns>
        public static bool IsAir(this IInclusionRegion region)
        {
            return region.RegionOP.Mua == 0D && region.RegionOP.Mus <= 1E-10;
        }
    }
}