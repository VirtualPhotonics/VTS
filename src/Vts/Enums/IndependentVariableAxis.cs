namespace Vts
{
    /// <summary>
    /// independent variable axis tyeps
    /// </summary>
    public enum IndependentVariableAxis
    {
        // add combinations (e.g. RhoAndFt, etc) and update ComputationFactory?
        /// <summary>
        /// source-detector separation (rho)
        /// </summary>
        Rho,
        /// <summary>
        /// time (t)
        /// </summary>
        Time,
        /// <summary>
        /// spatial-frequency (fx)
        /// </summary>
        Fx,
        /// <summary>
        /// temporal-frequency (ft)
        /// </summary>
        Ft,
        /// <summary>
        /// depth in tissue (z)
        /// </summary>
        Z,
        /// <summary>
        /// wavelength (lambda)
        /// </summary>
        Wavelength
    }
}