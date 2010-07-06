using System;

namespace Vts.Modeling.ForwardSolvers
{
    public class SFDDiffusionForwardSolver
    {


        public static double StationaryOneDimensionalSpatialFrequencyFluence(
            DiffusionParameters dp, double fx, double z)
        {
            var mueffPrime = Math.Sqrt(dp.mueff * dp.mueff + 4.0 * Math.PI * Math.PI * fx * fx);

            return dp.musTilde / dp.D / (mueffPrime * mueffPrime - dp.mutTilde * dp.mutTilde) * (
                Math.Exp(-dp.mutTilde * z) -
                (1.0 + dp.mutTilde * dp.zb) / (1.0 + mueffPrime * dp.zb) *
                Math.Exp(-mueffPrime * z));
        }

        // Unsure of the boundary condition in literature to the specific problem?
        public static double StationaryOneDimensionalSpatialFrequencyZFlux(
            DiffusionParameters dp, double fx, double z)
        {
            var mueffPrime = Math.Sqrt(dp.mueff * dp.mueff + 4.0 * Math.PI * Math.PI * fx * fx);
            // need to check that this is the "real" flux and not the assumed negative value...
            return dp.musTilde / (mueffPrime * mueffPrime - dp.mutTilde * dp.mutTilde)
                * (dp.mutTilde * Math.Exp(-dp.mutTilde * z) -
                   mueffPrime * (1.0 + dp.mutTilde * dp.zb) / (1.0 + mueffPrime * dp.zb) *
                   Math.Exp(-mueffPrime * z));
        }

    }
}
