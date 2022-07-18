namespace Vts
{
    /// <summary>
    /// chromophore coefficient types
    /// </summary>
    public enum ChromophoreCoefficientType
    {
        /// <summary>
        /// Absorption coefficients per unit distance in [1/mm] 
        /// (i.e. 2.303 * Absorbance)
        /// </summary>
        FractionalAbsorptionCoefficient,
        /// <summary>
        /// Extinction coefficients per unit distance per concentration [(1/mm)*(1/microMoloar)]
        /// (i.e. Absorbance / [Concentration * Path-length])
        /// </summary>
        MolarAbsorptionCoefficient,
    }
}