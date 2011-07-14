namespace Vts.SpectralMapping
{
    /// <summary>
    /// General interface for any spectrum (ie. ChromophoreSpectrum or ScatteringSpectrum)
    /// </summary>
    public interface ISpectrum
    {
        string Name { get; set; }
        double GetSpectralValue(double wavelength);
        ChromophoreCoefficientType ChromophoreCoefficientType { get; set; }
        AbsorptionCoefficientUnit AbsorptionCoefficientUnit { get; set; }
        MolarUnit MolarUnit { get; set; }
    }
}
