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
                    ForwardSolverType.PointSDA,
                    ForwardSolverType.DistributedPointSDA,
                    ForwardSolverType.DistributedGaussianSDA,
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