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

        public static RandomNumberGeneratorType[] RandomNumberGeneratorTypes
        {
            get
            {
                return new[]
                {
                    RandomNumberGeneratorType.MersenneTwister
                };
            }
        }

        public static AbsorptionWeightingType[] AbsorptionWeightingTypes
        {
            get
            {
                return new[]
                {
                    AbsorptionWeightingType.Analog,
                    AbsorptionWeightingType.Discrete,
                    AbsorptionWeightingType.Continuous
                };
            }
        }

        public static PhaseFunctionType[] PhaseFunctionTypes
        {
            get
            {
                return new[]
                {
                    PhaseFunctionType.HenyeyGreenstein
                };
            }
        }
    }
}
#endif