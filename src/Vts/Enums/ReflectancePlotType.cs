namespace Vts
{
    /// <summary>
    /// reflectance plot types
    /// </summary>
    public enum ReflectancePlotType
    {
        /// <summary>
        /// forward solver solutions plots
        /// </summary>
        ForwardSolver,
        /// <summary>
        /// measured data plot for inverse solver panel
        /// </summary>
        InverseSolverMeasuredData,
        /// <summary>
        /// forward solver at initial guess for inverse solver panel
        /// </summary>
        InverseSolverAtInitialGuess,
        /// <summary>
        /// forward solver at converged optical properties for inverse solver panel
        /// </summary>
        InverseSolverAtConvergedData,
        /// <summary>
        /// clear plot
        /// </summary>
        Clear
    }
}