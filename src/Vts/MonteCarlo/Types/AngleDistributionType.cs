namespace Vts.MonteCarlo
{
    /// <summary>
    /// Phase function type used with the Monte Carlo code
    /// </summary>
    public enum MuellerMatrixType
    {
        /// <summary>
        /// Mueller Matrix for Mie Scatterer
        /// </summary>
        Mie,
        /// <summary>
        /// Meuller Matrix obtained from T-matrix method for axially symmetric scatterers
        /// </summary>
        TMatrix,
        /// <summary>
        /// Meuller Matrix for linear vertial polarizer. (Default constructor for Meuller Matrix class.)
        /// </summary>
        LinearVerticalPolarizer,
        /// <summary>
        /// Meuller Matrix for an arbitrary scatterer
        /// </summary>
        General,
    }
}