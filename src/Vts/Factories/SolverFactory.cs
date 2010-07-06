using Vts.Modeling;
using Vts.Modeling.ForwardSolvers;
using Vts.Modeling.Optimizers;
using Vts.SpectralMapping;

namespace Vts.Factories
{
    public class SolverFactory
    {
        public static IForwardSolver GetForwardSolver(ForwardSolverType type)
        {
            switch (type)
            {
                case ForwardSolverType.DistributedPointSDA:
                default:
                    return new DistributedPointSourceDiffusionForwardSolver();
                case ForwardSolverType.PointSDA:
                    return new PointSourceDiffusionForwardSolver();
                case ForwardSolverType.DistributedGaussianSDA:
                    return new DistributedGaussianSourceDiffusionForwardSolver();
                case ForwardSolverType.MonteCarlo:
                    return new MonteCarloForwardSolver();
                case ForwardSolverType.DeltaPOne:
                    return new DeltaPOneForwardSolver();
                case ForwardSolverType.Nurbs:
                    return new NurbsForwardSolver();
                case ForwardSolverType.pMC:
                    return new pMCForwardSolver();
            }
        }

        public static IScatterer GetScattererType(ScatteringType type)
        {
            switch (type)
            {
                case ScatteringType.PowerLaw:
                default:
                    return PowerLawScatterer.Create(TissueType.Skin);
                case ScatteringType.Mie:
                    return MieScatterer.Create(MieScattererType.PolystyreneSphereSuspension);
                case ScatteringType.Intralipid:
                    return new IntralipidScatterer(0.01);
            }
        }

        public static IOptimizer GetOptimizer(OptimizerType type)
        {
            switch (type)
            {
                default:
                case OptimizerType.MPFitLevenbergMarquardt:
                    return new MPFitLevenbergMarquardtOptimizer();
            }
        }
    }
}
