namespace Vts
{
    /// <summary>
    /// Types of Forward solvers in our GUI
    /// </summary>
    public enum ForwardSolverType
    {
        /// <summary>
        /// Standard Diffusion Approximation (SDA) with point source forward solver
        /// </summary>
        PointSourceSDA,
        /// <summary>
        /// Standard Diffusion Approximation (SDA) with distributed point source forward solver
        /// </summary>
        DistributedPointSourceSDA,
        /// <summary>
        /// Standard Diffusion Approximation (SDA) with Gaussian distributed source forward solver
        /// </summary>
        DistributedGaussianSourceSDA,
        /// <summary>
        /// delta-P1 forward solver
        /// </summary>
        DeltaPOne,
        /// <summary>
        /// scaled Monte Carlo forward solver
        /// </summary>
        MonteCarlo,
        /// <summary>
        /// scaled Monte Carlo forward solver with non-uniform rational b-splines forward solver
        /// </summary>
        Nurbs,
        /// <summary>
        /// two-layer forward solver based on standard diffusion
        /// </summary>
        TwoLayerSDA,
        //        DiscreteOrdinates
    }
}