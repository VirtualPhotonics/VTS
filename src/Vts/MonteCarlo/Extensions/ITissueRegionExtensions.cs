namespace Vts.MonteCarlo.Extensions
{
    public static class ITissueRegionExtensions
    {
        public static bool IsAir(this ITissueRegion region)
        {
            return region.RegionOP.Mua == 0D && region.RegionOP.Mus <= 1E-10;
        }
    }
}