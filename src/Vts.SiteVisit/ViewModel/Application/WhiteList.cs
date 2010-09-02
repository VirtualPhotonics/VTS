#if WHITELIST
namespace Vts.SiteVisit.ViewModel.Application
{
    public static class WhiteList
    {
        public static ForwardSolverType[] ForwardSolverTypes
        {
            get
            {
                return new[]
                {
                    ForwardSolverType.PointSourceSDA,
                    ForwardSolverType.DistributedPointSourceSDA,
                    ForwardSolverType.DistributedGaussianSourceSDA,
                    ForwardSolverType.MonteCarlo,
                    ForwardSolverType.Nurbs
                };
            }
        }

        public static ScatteringType[] ScatteringTypes
        {
            get
            {
                return new[]
                {
                    ScatteringType.PowerLaw,
                    ScatteringType.Intralipid
                };
            }
        }
    }
}
#endif