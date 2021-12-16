namespace Vts.MonteCarlo
{
    /// <summary>
    /// Phase function type used within the Monte Carlo code
    /// </summary>
    public enum MeullerMatrixType
    {
        /// <summary>
        /// Mueller Matrix for Mie Scatterer
        /// </summary>
        Mie,
        /// <summary>
        /// Mueller Matrix obtained from T-matrix method for axially symmetric scatterers
        /// </summary>
        TMatrix,
        /// <summary>
        /// Mueller Matrix for a linear vertical polarizer (default constructor for the Mueller Matrix class)
        /// </summary>
        LinearVerticalPolarizer,
        /// <summary>
        /// Mueller Matrix for an arbitrary scatterer
        /// </summary>
        General,
    }
}