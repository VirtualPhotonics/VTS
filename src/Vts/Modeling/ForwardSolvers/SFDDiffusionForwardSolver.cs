using System;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// Class containing diffusion approximation solutions to the RTE in the
    /// spatial frequency domain.
    /// </summary>
    public class SFDDiffusionForwardSolver
    {

        /// <summary>
        /// Evaluate the time-independent, one dimensional spatial frequency, depth resolved
        /// fluence.
        /// </summary>
        /// <param name="dp">DiffusionParameters object</param>
        /// <param name="fx">spatial frequency</param>
        /// <param name="z">depth</param>
        /// <returns>fluence</returns>
        public static double StationaryOneDimensionalSpatialFrequencyFluence(
            DiffusionParameters dp, double fx, double z)
        {
            var mueffPrime = Math.Sqrt(dp.mueff * dp.mueff + 4.0 * Math.PI * Math.PI * fx * fx);

            return dp.musTilde / dp.D / (mueffPrime * mueffPrime - dp.mutTilde * dp.mutTilde) * (
                Math.Exp(-dp.mutTilde * z) -
                (1.0 + dp.mutTilde * dp.zb) / (1.0 + mueffPrime * dp.zb) *
                Math.Exp(-mueffPrime * z));
        }

        /// <summary>
        /// Evaluates the depth resolved diffuse spatial frequency flux in the z-direction.
        /// </summary>
        /// <param name="dp">DiffusionParameters object</param>
        /// <param name="fx">spatial frequency</param>
        /// <param name="z">depth</param>
        /// <returns>z-flux</returns>
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
