namespace Vts.FemModeling.MGRTE._2D
{
    public static class ITissueRegionExtensions
    {
        /// <summary>
        /// Method to determine if tissue region is air or not
        /// </summary>
        /// <param name="region">tissue region</param>
        /// <returns>boolean</returns>
        public static bool IsAir(this ITissueRegion region)
        {
            return region.RegionOP.Mua == 0D && region.RegionOP.Mus <= 1E-10;
        }
    }
}