namespace Vts.SpectralMapping
{
    /// <summary>
    /// General interface for any spectrum (ie. ChromophoreSpectrum or ScatteringSpectrum)
    /// </summary>
    public interface ISpectrum
    {
        /// <summary>
        /// Name of the spectrum
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Returns the spectral value for a specific wavelength
        /// </summary>
        /// <param name="wavelength">The wavelength value</param>
        /// <returns>A double representing the spectral value</returns>
        double GetSpectralValue(double wavelength);
        /// <summary>
        /// The chromophore coefficient type
        /// </summary>
        ChromophoreCoefficientType ChromophoreCoefficientType { get; set; }
        /// <summary>
        /// The absorption coefficient unit
        /// </summary>
        AbsorptionCoefficientUnit AbsorptionCoefficientUnit { get; set; }
        /// <summary>
        /// The molar unit
        /// </summary>
        MolarUnit MolarUnit { get; set; }
    }
}
